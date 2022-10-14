using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ZstdNet;

namespace SmallFileStore
{
    class FileRecord
    {
        [StoreAsText]
        public enum SaveLocation
        {
            MEMORYCACHE,LOOSE, GROUP
        }


        [PrimaryKey,AutoIncrement]
        public long Id { get; set; }
        [Indexed]
        public string filename { get; set; }
        [Indexed]
        public string category { get; set; } // Serves as a kind of folder if you want it. Or different types of records. Whatever u want.
        [Indexed]
        public DateTime FileDate { get; set; }
        [Indexed]
        public SaveLocation referenceType { get; set; }
        [Indexed]
        public long referenceId { get; set; } // Either reference to cache position or loose file position
        public long offsetStart { get; set; }
        public long dataLength { get; set; }
    }

    class GroupRecord
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public byte[] compressedData { get; set; }
        public long? DictionaryId { get; set; }

    }
    class LooseRecord
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public byte[] rawData { get; set; }

    }

    class Dictionary
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        [Unique]
        public string hash { get; set; }
        public byte[] data { get; set; }
    }
    class SmallFileStore
    {
        struct CachedEntry
        {
            public string filename;
            public string category;
            public byte[] data;
            public DateTime dateAdded;
        }

        private int _recordsPerGroup = 100;
        private string _databasePath = "";
        private string _dictionaryPath = "";
        private string _currentDictionaryHash = null;
        private long? _currentDictionaryId = null;
        private CompressionOptions _compressionOptions;

        private Queue<CachedEntry> writeCache = new Queue<CachedEntry>();

        public SmallFileStore(string databasePath,string dictionaryPath=null,int recordsPerGroup = 100)
        {
            _databasePath = databasePath;
            _dictionaryPath = dictionaryPath;

            // create neccessary tables if not already existing.
            lock (_databasePath)
            {
                using (SQLiteConnection db = new SQLiteConnection(databasePath, false))
                {
                    db.CreateTable<Dictionary>();
                    db.CreateTable<FileRecord>();
                    db.CreateTable<GroupRecord>();
                   
                    

                    db.Close();
                }
            }

            if (dictionaryPath == null)
            {
                _compressionOptions = new CompressionOptions( CompressionOptions.MaxCompressionLevel);

            }
            else
            {
                byte[] dictionary = File.ReadAllBytes(dictionaryPath);

                _currentDictionaryHash = GetHashSHA1(dictionary);

                lock (_databasePath)
                {

                    // Insert dictionary into db or get hash of existing dictionary.
                    using (SQLiteConnection db = new SQLiteConnection(databasePath, false))
                    {
                        db.Get<Dictionary>(d => d.hash.Equals(_currentDictionaryHash));
                        TableQuery<Dictionary> query = db.Table<Dictionary>().Where(d => d.hash.Equals(_currentDictionaryHash));
                        
                        if (query.Count() == 0)
                        {
                            Dictionary thisNewDict = new Dictionary() { data = dictionary, hash = _currentDictionaryHash };
                            db.Insert(thisNewDict);
                            _currentDictionaryId = thisNewDict.Id;
                        }
                        else
                        {
                            _currentDictionaryId = query.FirstOrDefault().Id;
                        }

                        db.Close();
                    }


                }


                _compressionOptions = new CompressionOptions(dictionary, CompressionOptions.MaxCompressionLevel);

            }
        }

        // Text version
        public void AddFile(string filename,string textData,string category = null,DateTime? dateAdded = null)
        {

            byte[] data = Encoding.UTF8.GetBytes(textData);
            AddFile(filename, data, category, dateAdded);
        }

        // Normal add.
        public void AddFile(string filename,byte[] data,string category = null,DateTime? dateAdded = null)
        {
            if(dateAdded == null)
            {
                dateAdded = DateTime.UtcNow;
            }
            lock (writeCache)
            {
                writeCache.Enqueue(new CachedEntry() { filename = filename, category = category, dateAdded = dateAdded.Value, data = data });
            }
            _checkIfShouldWriteAndWrite();
        }

        private void _checkIfShouldWriteAndWrite()
        {
            lock (writeCache)
            {

                if (writeCache.Count >= _recordsPerGroup)
                {

                    CachedEntry[] entriesToWrite= writeCache.DequeueChunk(_recordsPerGroup).ToArray();
                    List<byte> groupData = new List<byte>();
                    List<FileRecord> fileRecords = new List<FileRecord>();

                    long offsetHere = 0;
                    foreach(CachedEntry entry in entriesToWrite)
                    {
                        FileRecord record = new FileRecord() { FileDate=entry.dateAdded, category=entry.category, filename=entry.filename, offsetStart=offsetHere,dataLength= entry.data.Length, referenceType= FileRecord.SaveLocation.GROUP };
                        groupData.AddRange(entry.data);
                        offsetHere += entry.data.Length;
                        fileRecords.Add(record);
                    }

                    GroupRecord groupRec = new GroupRecord() { DictionaryId=_currentDictionaryId};
                    Compressor compressor = new Compressor(_compressionOptions);

                    groupRec.compressedData = compressor.Wrap(groupData.ToArray());
                    lock (_databasePath)
                    {
                        using (SQLiteConnection db = new SQLiteConnection(_databasePath, false))
                        {
                            db.Insert(groupRec);
                            db.BeginTransaction();
                            foreach(FileRecord record in fileRecords)
                            {
                                record.referenceId = groupRec.Id;
                                db.Insert(record);
                            }
                            db.Commit();

                            db.Close();
                        }
                    }

                }
            }
        }

        // from: https://stackoverflow.com/a/800469
        public static string GetHashSHA1(byte[] data)
        {
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                return string.Concat(sha1.ComputeHash(data).Select(x => x.ToString("X2")));
            }
        }

        // Need to write any remaining files to the store (DB) before closing.
        ~SmallFileStore()
        {
            DumpLooseFiles();
        }

        // Only call when disposing.
        // Maybe at some point I can introduce a safety feature that backs up
        // loose files in regular intervals but rn that's too complicated.
        // For now they'll be dumped when the object closes and 
        // loaded when it opens.
        private void DumpLooseFiles()
        {
            lock (writeCache)
            {


                CachedEntry[] entriesToWrite = writeCache.DequeueChunk(writeCache.Count).ToArray();


                lock (_databasePath)
                {
                    using (SQLiteConnection db = new SQLiteConnection(_databasePath, false))
                    {
                        foreach (CachedEntry entry in entriesToWrite)
                        {
                            FileRecord record = new FileRecord() { FileDate = entry.dateAdded, category = entry.category, filename = entry.filename, offsetStart = 0, dataLength = entry.data.Length, referenceType = FileRecord.SaveLocation.LOOSE };
                            LooseRecord looseRec = new LooseRecord() { rawData = entry.data };

                            db.Insert(looseRec);
                            record.referenceId = looseRec.Id;
                            db.Insert(record);
                        }


                        db.Close();
                    }
                }


            }
        }
    }

    public static class QueueExtensions
    {
        public static IEnumerable<T> DequeueChunk<T>(this Queue<T> queue, int chunkSize)
        {
            for (int i = 0; i < chunkSize && queue.Count > 0; i++)
            {
                yield return queue.Dequeue();
            }
        }
    }
}


