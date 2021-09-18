using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TwitterSearchScraper
{
    public class TwitterBase
    {

        public int retryCountOnIssues = 10;

        //protected string cookie, xsrf, guesttoken;
        protected string guestToken, bearerToken;
        protected JsonSerializerOptions jsonOpt = new JsonSerializerOptions();

        private const int REQUESTS_PER_TOKEN = 100;
        private int requestsMadeWithCurrentGuestToken = 0;
        protected int RequestsMadeWithCurrentGuestToken
        {
            get { return requestsMadeWithCurrentGuestToken; }
            set { 
                requestsMadeWithCurrentGuestToken = value;
                if(requestsMadeWithCurrentGuestToken >= REQUESTS_PER_TOKEN)
                {
                    GetGuestToken();
                    requestsMadeWithCurrentGuestToken = 0;
                }
            }
        }

        protected void GetGuestToken()
        {
            bool successful = false;

            while (!successful) {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://api.twitter.com/1.1/guest/activate.json");
                req.Headers.Add("authorization", "Bearer "+bearerToken);
                req.Method = "POST";
                WebResponse response;
                try
                {
                    response = req.GetResponse();
                    string webcontent;
                    using (var strm = new StreamReader(response.GetResponseStream()))
                    {

                        // Headers
                        StringBuilder headersForLog = new StringBuilder();
                        Dictionary<string, string> headers = new Dictionary<string, string>();
                        webcontent = strm.ReadToEnd();
                        for (int i = 0; i < response.Headers.Count; ++i)
                        {
                            string header = response.Headers.GetKey(i);
                            foreach (string value in response.Headers.GetValues(i))
                            {
                                headers.Add(header, value);
                                headersForLog.AppendLine(header + ":" + value);
                            }
                        }
                        Logger.Log("twitterGuestToken", webcontent);
                        Logger.Log("twitterGuestTokenHeaders", headersForLog.ToString());

                        JSONModels.TwitterGuestToken.Rootobject ro = JsonSerializer.Deserialize<JSONModels.TwitterGuestToken.Rootobject>(webcontent, jsonOpt);
                        guestToken = ro.guest_token;
                        successful = true;
                    }

                }
                catch (WebException e)
                {
                    if(e.Response is HttpWebResponse)
                    {
                        if((e.Response as HttpWebResponse).StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            try
                            {
                                string webcontent;
                                using (var strm = new StreamReader((e.Response as HttpWebResponse).GetResponseStream()))
                                {

                                    // Headers
                                    StringBuilder headersForLog = new StringBuilder();
                                    Dictionary<string, string> headers = new Dictionary<string, string>();
                                    webcontent = strm.ReadToEnd();

                                    Console.WriteLine(webcontent);
                                }
                            } catch(Exception f)
                            {
                                Console.WriteLine("Error getting details on 429: " + f.Message);
                            }
                        }
                    }
                    
                    Console.WriteLine("Error: " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        public TwitterBase(string bearerTokenA)
        {

            bearerToken = bearerTokenA;
            jsonOpt.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
            Directory.CreateDirectory("logs");
            GetGuestToken();
        }
    }
}
