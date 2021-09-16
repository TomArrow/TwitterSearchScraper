using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            _ = Task.Run(go);
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

                Parallel.ForEach(config.Sections, new ParallelOptions() { MaxDegreeOfParallelism=5}, (ConfigSection section) =>
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

                    Dispatcher.Invoke(() =>
                    {
                        searchTerms = config.getDuplicateValueArray(sectionName, "term");
                        wrongSpell = config.GetValue(sectionName, "wrongSpell", 0);
                        ignore = config.GetValue(sectionName, "ignore", 0);
                        outputDirectory = config.GetValue(sectionName, "output").Trim('/').Trim('\\');
                    });

                    if(ignore > 0) // Option to temporarily disable crawls
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
                        TwitterSearchResult result = tb.doSearch(term);
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
                    }
                });

                System.Threading.Thread.Sleep(waitBetweenCrawls*1000);
            }


        }
    }
}
