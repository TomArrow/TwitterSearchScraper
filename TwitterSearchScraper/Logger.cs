using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SQLite;

namespace TwitterSearchScraper
{

    public class LogEntry
    {
        [StoreAsText]
        public enum CompressType
        {
            RAW,
            GZIP,
            ZLIB // Not yet implemented
        }

        [PrimaryKey,AutoIncrement]
        public long id { get; set; }
        [Indexed]
        public string type { get; set; }
        public CompressType compressType { get; set; }
        public byte[] content { get; set; }
        public string contentRaw { get; set; }
        public DateTime when { get; set; }
    }

    public static class Logger
    {
        private static string logDBmutex = "abc";
        private enum SAVETYPE
        {
            SQLITE,
            TEXT,
            NONE
        }
        private const SAVETYPE saveType = SAVETYPE.SQLITE;
        public static void Log(string type,string content)
        {
            switch (saveType)
            {
                case SAVETYPE.SQLITE:
                    //LogEntry logEntry = new LogEntry() { content = compressGzip(content),compressType=LogEntry.CompressType.GZIP, type = type, when= DateTime.UtcNow };
                    LogEntry logEntry = new LogEntry() { contentRaw = content,compressType=LogEntry.CompressType.RAW, type = type, when= DateTime.UtcNow };
                    Application.Current.Dispatcher.Invoke(() => {
                        lock (logDBmutex) { 
                            var db = new SQLiteConnection("logs_"+ DateTime.UtcNow.ToString("yyyy-MM-dd") + ".db", false);
                            db.CreateTable<LogEntry>();
                            db.Insert(logEntry);
                            db.Close();
                            db.Dispose();
                        }
                        //Directory.CreateDirectory("logs");
                        //File.WriteAllText("logs\\" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + type + ".json", content);
                    });
                    break;
                case SAVETYPE.TEXT:
                    type = MakeValidFileName(type);
                    Application.Current.Dispatcher.Invoke(() => {
                        Directory.CreateDirectory("logs");
                        File.WriteAllText("logs\\" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + type + ".json", content);
                    });
                    break;
                default:
                    break;
            }
            
        }

        // from: https://stackoverflow.com/a/847251
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private static byte[] compressGzip(string input)
        {

            using (MemoryStream uncompressed = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {

                using (MemoryStream compressed = new MemoryStream())
                {
                    using (GZipStream compressor = new GZipStream(compressed, CompressionMode.Compress))
                    {
                        uncompressed.CopyTo(compressor);
                    }
                    // Get the compressed bytes only after closing the GZipStream
                    byte[] compressedBytes = compressed.ToArray();
                    return compressedBytes;
                }
            }
        }
    }
}
