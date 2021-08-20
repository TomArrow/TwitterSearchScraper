using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace TwitterSearchScraper
{
    public class TwitterSearch : TwitterBase
    {
        public TwitterSearch(string bearerToken) : base(bearerToken){


        }

        public void doSearch(string term)
        {
            string link = "https://twitter.com/i/api/2/search/adaptive.json?include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&cards_platform=Web-12&include_cards=1&include_ext_alt_text=true&include_quote_count=true&include_reply_count=1&tweet_mode=extended&include_entities=true&include_user_entities=true&include_ext_media_color=true&include_ext_media_availability=true&send_error_codes=true&simple_quoted_tweet=true&q="+HttpUtility.UrlEncode(term)+"&result_filter=video&count=20&query_source=typed_query&pc=1&spelling_corrections=1&ext=mediaStats%2ChighlightedLabel%2CvoiceInfo";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
            req.Headers.Add("authorization", "Bearer " + bearerToken);
            req.Headers.Add("x-guest-token", guestToken);
            req.Method = "GET";
            try
            {
                var response = req.GetResponse();
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
                    Logger.Log("twitterSearch-"+term, webcontent);
                    Logger.Log("twitterGuestTokenHeaders-"+term, headersForLog.ToString());

                    JSONModels.TwitterSearch.Rootobject ro = JsonSerializer.Deserialize<JSONModels.TwitterSearch.Rootobject>(webcontent, jsonOpt);

                    Console.WriteLine("text");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }

            //FourPlebsSearchResult.Rootobject ro = JsonSerializer.Deserialize<FourPlebsSearchResult.Rootobject>(newText, opt);

        }

    }
}
