using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TwitterSearchScraper
{
    public static class Logger
    {
        public static void Log(string type,string content)
        {
            type = MakeValidFileName(type);
            Application.Current.Dispatcher.Invoke(()=>{
                Directory.CreateDirectory("logs");
                File.WriteAllText("logs\\" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + type + ".json", content);
            });
        }

        // from: https://stackoverflow.com/a/847251
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
