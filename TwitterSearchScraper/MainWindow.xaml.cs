using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Salaros.Configuration;
using SQLite;

namespace TwitterSearchScraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory("logs");
            Directory.CreateDirectory("backup");

            //go();



            test();

            //_ = Task.Run(go);
        }

        public void test()
        {

            //string[] filesForTesting = Directory.GetFiles("logs");
            string[] filesForTesting = Directory.GetFiles("testlogs");
            //string[] filesForTesting = Directory.GetFiles("onelog");

            var db = new SQLiteConnection("testiklus2.db",false);

            JSONModels.TwitterSearch.Tweet.createTables(db);


            db.BeginTransaction();
            foreach (string file in filesForTesting)
            {

                JsonSerializerOptions jsonOpt = new JsonSerializerOptions() { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString };
                string webcontent = File.ReadAllText(file);
                try
                {

                    JSONModels.TwitterSearch.Rootobject ro = JsonSerializer.Deserialize<JSONModels.TwitterSearch.Rootobject>(webcontent, jsonOpt);


                    if(ro.globalObjects == null || ro.globalObjects.tweets == null) 
                    {
                        continue;
                    }

                    foreach (JSONModels.TwitterSearch.Tweet tweet in ro.globalObjects.tweets.Values)
                    {
                        tweet.screenname = ro.globalObjects.users[tweet.user_id].screen_name;
                        tweet.username = ro.globalObjects.users[tweet.user_id].name;
                        //tweet.insertOrIgnoreIntoSQLite(db);
                        try
                        {
                            tweet.insertOrIgnoreIntoSQLite(db);
                            /*db.Insert(tweet,"OR IGNORE");
                            if(tweet.entities != null && tweet.entities.media != null)
                            {
                                foreach(JSONModels.TwitterSearch.Medium2 medium in tweet.entities.media)
                                {
                                    db.Insert(medium, "OR IGNORE");
                                }
                            }
                            if(tweet.extended_entities != null && tweet.extended_entities.media != null)
                            {
                                foreach(JSONModels.TwitterSearch.Medium2 medium in tweet.extended_entities.media)
                                {
                                    db.Insert(medium, "OR IGNORE");
                                }
                            }*/

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("muh error" + e.Message);
                        }
                    }
                    try
                    {


                        db.InsertAll(ro.globalObjects.users.Values, "OR IGNORE");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("muh error" + e.Message);
                    }

                } catch(Exception e)
                {
                    Console.WriteLine("muh error" + e.Message);
                }


            }

            db.Commit();
            db.Close();
            db.Dispose();
            //Console.WriteLine("abc");
        }

        // 
        private string[] createVariations(string[] inputTerms,int variationCount=1)
        {
            List<string> retVal = new List<string>();

            foreach(string term in inputTerms)
            {
                retVal.Add(term);
                for(int i = 0; i < term.Length; i++)
                {
                    // Remove versions with one character at each place removed.
                    retVal.Add(term.Substring(0,i) + term.Substring(i+1));
                }
            }

            if(variationCount == 1)
            {

                return retVal.ToArray();
            } else
            {
                return createVariations(retVal.ToArray(), variationCount - 1);
            }

        }

        public void go()
        {

            while (true)
            {

                ConfigParserMod config = new ConfigParserMod("crawlConfig.ini");
                ConfigParserMod generalConfig = new ConfigParserMod("generalConfig.ini");

                string bearerToken = generalConfig.GetValue("twitter", "bearerToken");
                int waitBetweenCrawls = generalConfig.GetValue("general", "waitBetweenCrawls", 600);
                int retryCountOnIssues = generalConfig.GetValue("general", "retryCountOnIssues", 10);

                try
                {
                    Parallel.ForEach(config.Sections, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, (ConfigSection section) =>
                       {
                           //foreach (ConfigSection section in config.Sections)
                           //{

                           //MessageBox.Show(section.SectionName);

                           string sectionName = section.SectionName;
                           if (sectionName == "__example__")
                           {
                               //continue;
                               return;
                           }
                           string[] searchTerms = new string[0];
                           int wrongSpell = 0;
                           string outputDirectory = "";
                           int ignore = 0;
                           int latest = 0;

                           Dispatcher.Invoke(() =>
                           {
                               searchTerms = config.getDuplicateValueArray(sectionName, "term");
                               wrongSpell = config.GetValue(sectionName, "wrongSpell", 0);
                               ignore = config.GetValue(sectionName, "ignore", 0);
                               latest = config.GetValue(sectionName, "latest", 0);
                               outputDirectory = config.GetValue(sectionName, "output").Trim('/').Trim('\\');
                           });

                           if (ignore > 0) // Option to temporarily disable crawls
                           {
                               return;
                           }

                           List<string> variations = new List<string>();
                           foreach (string term in searchTerms)
                           {
                               int wrongSpellLocal = wrongSpell;
                               string[] tmp = term.Split(':');
                               string termLocal = tmp[0];
                               if (tmp.Length > 1) // wrongspell number override for this searchterm
                               {
                                   int.TryParse(tmp[1], out wrongSpellLocal);
                               }
                               if (wrongSpellLocal == 0)
                               {
                                   variations.Add(termLocal);
                               }
                               else
                               {

                                   variations.AddRange(createVariations(new string[] { termLocal }, wrongSpellLocal));
                               }
                           }


                           foreach (string term in variations)
                           {

                               TwitterSearch tb = new TwitterSearch(bearerToken);
                               tb.retryCountOnIssues = retryCountOnIssues;
                               TwitterSearchResult result = tb.doSearch(term, latest > 0);
                               if (result.photos.Length > 0)
                               {
                                   File.AppendAllLines(outputDirectory + "\\" + sectionName + ".photos.txt", result.photos);
                                   File.AppendAllLines("backup\\" + sectionName + ".photos.txt", result.photos);
                               }
                               if (result.videos.Length > 0)
                               {
                                   File.AppendAllLines(outputDirectory + "\\" + sectionName + ".videos.txt", result.videos);
                                   File.AppendAllLines("backup\\" + sectionName + ".videos.txt", result.videos);
                               }
                               if (result.tweets.Count > 0 || result.users.Count > 0)
                               {
                                   string[] databases = { outputDirectory + "\\" + sectionName + ".db", "backup\\" + sectionName + ".db" };
                                   foreach (string databasePath in databases)
                                   {

                                       var db = new SQLiteConnection(databasePath, false);
                                       JSONModels.TwitterSearch.Tweet.createTables(db);

                                       //db.CreateTable<JSONModels.TwitterSearch.Tweet>();
                                       //db.CreateTable<JSONModels.TwitterSearch.User>();
                                       db.BeginTransaction();

                                       foreach (JSONModels.TwitterSearch.Tweet tweet in result.tweets.Values)
                                       {
                                           tweet.screenname = result.users[tweet.user_id].screen_name;
                                           tweet.username = result.users[tweet.user_id].name;
                                           tweet.insertOrIgnoreIntoSQLite(db);
                                           //db.Insert(tweet);
                                       }

                                       db.InsertAll(result.users.Values);

                                       db.Commit();
                                       db.Close();
                                       db.Dispose();
                                   }
                               }
                           }
                       });
                } catch(Exception e)
                {
                    
                    Logger.Log("ERROR-parallelFor",e.Message+"\n"+e.StackTrace);
                }

                System.Threading.Thread.Sleep(waitBetweenCrawls*1000);
            }


        }
    }
}
