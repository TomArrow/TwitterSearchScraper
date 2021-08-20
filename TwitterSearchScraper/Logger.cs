using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSearchScraper
{
    public static class Logger
    {
        public static void Log(string type,string content)
        {
            Directory.CreateDirectory("logs");
            File.WriteAllText("logs\\" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + "-"+type+".json", content);
        }
    }
}
