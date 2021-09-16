using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Threading;

namespace TwitterSearchScraper
{
    public struct TwitterSearchResult
    {
        public string[] photos;
        public string[] videos;
    }

    public class TwitterSearch : TwitterBase
    {
        public TwitterSearch(string bearerToken) : base(bearerToken){


        }

        public TwitterSearchResult doSearch(string term)
        {

            string[] filterKinds = new string[] { "", "video","image"};

            List<string> photos = new List<string>();
            List<string> videos = new List<string>();


            foreach(string kind in filterKinds)
            {

                bool endReached = false;

                string scroll = "";

                int index = 0;

                int errorsInARow = 0;

                long[] lastBatchTweetIds = new long[0];

                int endlessLoopDetectionsInARow = 0;
                int endlessLoopDetectionsTreshold = 10;

                while (!endReached) {
                    index++;
                    string link = "https://twitter.com/i/api/2/search/adaptive.json?include_profile_interstitial_type=1&include_blocking=1&include_blocked_by=1&include_followed_by=1&include_want_retweets=1&include_mute_edge=1&include_can_dm=1&include_can_media_tag=1&skip_status=1&cards_platform=Web-12&include_cards=1&include_ext_alt_text=true&include_quote_count=true&include_reply_count=1&tweet_mode=extended&include_entities=true&include_user_entities=true&include_ext_media_color=true&include_ext_media_availability=true&send_error_codes=true&simple_quoted_tweet=true&q="+HttpUtility.UrlEncode(term)+"&count=20&query_source=typed_query&pc=1&spelling_corrections=1&ext=mediaStats%2ChighlightedLabel%2CvoiceInfo"+(kind==""?"": "&result_filter="+kind)+scroll;
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
                    req.Headers.Add("authorization", "Bearer " + bearerToken);
                    req.Headers.Add("x-guest-token", guestToken);
                    req.Method = "GET";
                    WebResponse response;

                    List<long> tweetIds = new List<long>();


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

                            Logger.Log("twitterSearch-" + kind + "-" + term + "-" + index, webcontent);
                            Logger.Log("twitterSearchHeaders-" + kind + "-" + term + "-" + index, headersForLog.ToString());
                            
                            JSONModels.TwitterSearch.Rootobject ro = JsonSerializer.Deserialize<JSONModels.TwitterSearch.Rootobject>(webcontent, jsonOpt);


                            if(ro.globalObjects.tweets != null)
                            {
                                if(ro.globalObjects.tweets.Count == 0)
                                {
                                    endReached = true;
                                }

                                foreach(KeyValuePair<string,JSONModels.TwitterSearch.Tweet> tweet in ro.globalObjects.tweets)
                                {
                                    tweetIds.Add(tweet.Value.id);
                                    if(tweet.Value.extended_entities != null && tweet.Value.extended_entities.media != null) { 
                                    
                                        foreach(JSONModels.TwitterSearch.Medium2 medium in tweet.Value.extended_entities.media)
                                        {
                                            if(medium.type != null)
                                            {
                                                if (medium.type == "video")
                                                {
                                                    videos.Add(medium.expanded_url);
                                                } else if (medium.type == "photo")
                                                {
                                                    photos.Add(medium.expanded_url);
                                                }
                                            }
                                        }
                                    }

                                }
                            } else
                            {
                               //hmmm
                            }


                            // Check if tweet Ids were identical to last batch. Can indicate infinite loop
                            long[] tmp = tweetIds.ToArray();
                            bool oneNonDupe = false;

                            foreach(long oldId in lastBatchTweetIds)
                            {
                                bool dupeFound = false;
                                if(tweetIds.Count > 0) { 
                                    for(int i = tweetIds.Count - 1; i >= 0; i--)
                                    {
                                        if(tweetIds[i] == oldId)
                                        {
                                            tweetIds.RemoveAt(i);
                                            dupeFound = true;
                                            break;
                                        }
                                    }
                                }
                                if (!dupeFound)
                                {
                                    oneNonDupe = true;
                                }
                            }
                            lastBatchTweetIds = (long[])tmp.Clone();
                            // Two conditions
                            // 1. After removing dupes from new crawl, no items left
                            // 2. There are no entries in previous crawl that haven't found a match (oneNonDupe)
                            if(tweetIds.Count == 0 && oneNonDupe == false)
                            {
                                endlessLoopDetectionsInARow++;
                            } else
                            {
                                endlessLoopDetectionsInARow = 0;
                            }
                            if(endlessLoopDetectionsInARow > endlessLoopDetectionsTreshold)
                            {
                                endReached = true;
                            }

                            // Find scroll cursor for more results
                            scroll = "";
                            if(ro.timeline != null && ro.timeline.instructions != null)
                            {

                                foreach (JSONModels.TwitterSearch.Instruction instruction in ro.timeline.instructions)
                                {
                                    if(instruction.addEntries != null && instruction.addEntries.entries != null)
                                    {
                                        foreach(JSONModels.TwitterSearch.Entry entry in instruction.addEntries.entries)
                                        {
                                            if(entry.content != null && entry.content.operation != null && entry.content.operation.cursor != null && entry.content.operation.cursor.cursorType == "Bottom")
                                            {
                                                scroll = "&cursor=" + entry.content.operation.cursor.value;
                                            }
                                        }
                                    }
                                    if (instruction.replaceEntry != null && instruction.replaceEntry.entry != null)
                                    {

                                        JSONModels.TwitterSearch.Entry entry = instruction.replaceEntry.entry;
                                        if (entry.content != null && entry.content.operation != null && entry.content.operation.cursor != null && entry.content.operation.cursor.cursorType == "Bottom")
                                        {
                                            scroll = "&cursor=" + entry.content.operation.cursor.value;
                                        }
                                    }
                                }
                            }
                            if(scroll == "")
                            {
                                endReached = true;
                            }
                            errorsInARow = 0;
                        }

                    }
                    catch (Exception e)
                    {
                        errorsInARow++;
                        if(errorsInARow > retryCountOnIssues) {
                            Console.WriteLine(errorsInARow.ToString() + " errors in a row. Skipping this one!");
                            break;
                        }
                        Console.WriteLine("Error: "+e.Message);
                    }

                    RequestsMadeWithCurrentGuestToken++;
                }

            }
            TwitterSearchResult result = new TwitterSearchResult();
            result.videos = videos.ToArray();
            result.photos = photos.ToArray();

            return result;
            //FourPlebsSearchResult.Rootobject ro = JsonSerializer.Deserialize<FourPlebsSearchResult.Rootobject>(newText, opt);

        }

    }
}
