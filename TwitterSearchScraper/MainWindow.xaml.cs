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

            go();
        }

        public void go()
        {
            ConfigParserMod config = new ConfigParserMod("crawlConfig.ini");
            ConfigParserMod generalConfig = new ConfigParserMod("generalConfig.ini");

            string bearerToken = generalConfig.GetValue("twitter", "bearerToken");

            foreach(ConfigSection section in config.Sections)
            {
                string sectionName = section.SectionName;
                string[] searchTerms = config.getDuplicateValueArray(sectionName, "term");
                int wrongSpell = config.GetValue(sectionName, "wrongSpell", 0);
                string outputDirectory = config.GetValue(sectionName, "output");


            }

            TwitterSearch tb = new TwitterSearch(bearerToken);
            tb.doSearch("beirut");
        }
    }
}
