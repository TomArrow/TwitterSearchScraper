
using SQLite;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JSONModels.TwitterSearch
{
    public class Rootobject
    {
        public Globalobjects globalObjects { get; set; }
        public Timeline timeline { get; set; }
    }

    public class Globalobjects
    {
        public Dictionary<long, Tweet> tweets { get; set; }
        //public Dictionary<string, Tweet> Tweet[] tweets { get; set; }
        public Dictionary<long, User> users { get; set; }
        /*public Moments moments { get; set; }
        public Cards cards { get; set; }
        public Places places { get; set; }
        public Media media { get; set; }
        public Broadcasts broadcasts { get; set; }
        public Topics topics { get; set; }
        public Lists lists { get; set; }*/
    }



    public class Tweet
    {
        bool isFixed = false;

        public static void createTables(SQLiteConnection conn)
        {
            conn.CreateTable<Tweet>();
            conn.CreateTable<User>();
            conn.CreateTable<Medium2>();
            conn.CreateTable<Variant>();
            conn.CreateTable<Url>();
        }

        public void insertOrIgnoreIntoSQLite(SQLiteConnection conn)
        {
            doFixesBeforeSaving();
            conn.Insert(this, "OR IGNORE");

            if (extended_entities != null && extended_entities.urls != null)
            {
                foreach (Url url in extended_entities.urls)
                {
                    conn.Insert(url, "OR IGNORE");
                }
            }
            if (entities != null && entities.urls != null)
            {
                foreach (Url url in entities.urls)
                {
                    conn.Insert(url, "OR IGNORE");
                }
            }

            if (extended_entities != null && extended_entities.media != null)
            {
                foreach (Medium2 medium in extended_entities.media)
                {
                    conn.Insert(medium, "OR IGNORE");
                    if (medium.video_info != null && medium.video_info.variants != null)
                    {
                        foreach (Variant variant in medium.video_info.variants)
                        {
                            conn.Insert(variant, "OR IGNORE");
                        }
                    }
                }
            }

            if (entities != null && entities.media != null)
            {
                foreach (Medium2 medium in entities.media)
                {
                    conn.Insert(medium, "OR IGNORE");
                    if (medium.video_info != null && medium.video_info.variants != null)
                    {
                        foreach (Variant variant in medium.video_info.variants)
                        {
                            conn.Insert(variant, "OR IGNORE");
                        }
                    }
                }
            }

        }

        public void doFixesBeforeSaving()
        {
            if (!isFixed)
            {

                Int64 tweetIdTimestamp = 1288834974657 + (id >> 22);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(tweetIdTimestamp);
                created_time_from_id = dateTimeOffset.UtcDateTime;

                string tmp = full_text;
                if (entities != null && entities.urls != null)
                {
                    foreach (Url url in entities.urls)
                    {
                        url.tweetId = id;
                        tmp = tmp.Replace(url.url, url.expanded_url);
                    }
                } 
                if (extended_entities != null && extended_entities.urls != null)
                {
                    foreach (Url url in extended_entities.urls)
                    {
                        url.tweetId = id;
                    }
                } /*
                if (entities != null && entities.media != null)
                {
                    foreach (Medium2 url in entities.media)
                    {
                        tmp = tmp.Replace(url.url, url.expanded_url);
                    }
                } */

                full_text_urlsreplaced = tmp;

                // Give media a reference to main tweet
                if (entities != null && entities.media != null)
                {
                    foreach (Medium2 medium in entities.media)
                    {
                        Int64 mediumIdTimestamp = 1288834974657 + (medium.id >> 22);
                        dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(mediumIdTimestamp);
                        medium.created_time_from_id = dateTimeOffset.UtcDateTime;

                        medium.tweetId = id;
                        if (medium.video_info != null && medium.video_info.aspect_ratio != null)
                        {
                            medium.video_aspect_ratio = String.Join("x", medium.video_info.aspect_ratio);
                        }
                        if(medium.video_info != null && medium.video_info.duration_millis != null)
                        {
                            medium.video_duration_millis = medium.video_info.duration_millis.Value;
                        }
                        if(medium.video_info != null && medium.video_info.variants != null)
                        {
                            foreach(Variant variant in medium.video_info.variants)
                            {
                                variant.mediumId = medium.id;
                            }
                        }
                    }
                }
                if (extended_entities != null && extended_entities.media != null)
                {
                    foreach (Medium2 medium in extended_entities.media)
                    {
                        Int64 mediumIdTimestamp = 1288834974657 + (medium.id >> 22);
                        dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(mediumIdTimestamp);
                        medium.created_time_from_id = dateTimeOffset.UtcDateTime;

                        medium.tweetId = id;

                        if (medium.video_info != null && medium.video_info.aspect_ratio != null)
                        {
                            medium.video_aspect_ratio = String.Join("x", medium.video_info.aspect_ratio);
                        }
                        if (medium.video_info != null && medium.video_info.duration_millis != null)
                        {
                            medium.video_duration_millis = medium.video_info.duration_millis.Value;
                        }
                        if (medium.video_info != null && medium.video_info.variants != null)
                        {
                            foreach (Variant variant in medium.video_info.variants)
                            {
                                variant.mediumId = medium.id;
                            }
                        }
                    }
                }


                if(place != null && place.bounding_box != null && place.bounding_box.coordinates != null)
                {
                    place_coordinates = place.bounding_box.coordinates;
                }

                isFixed = true;
            }
        }

        // Manually set values
        public string username { get; set; }
        public string screenname { get; set; }
        public string full_text_urlsreplaced { get; set; }
        public string place_coordinates { get; set; }
        public DateTime created_time_from_id { get; set; }

        // Actual response:
        /*
        public string created_at { get; set; }*/
        [PrimaryKey,  Indexed]
        public long id { get; set; }
        public string id_str { get; set; }

        public string full_text { get; set; }/*
        public bool truncated { get; set; }
        public long[] display_text_range { get; set; }*/
        [Ignore] // Bc we'll do this manually
        public Entities entities { get; set; }
        [Ignore] // Bc we'll do this manually
        public Extended_Entities extended_entities { get; set; }/*
        public string source { get; set; }*/
        public long? in_reply_to_status_id { get; set; }
        [Ignore]
        public string in_reply_to_status_id_str { get; set; }
        public long? in_reply_to_user_id { get; set; }
        [Ignore]
        public string in_reply_to_user_id_str { get; set; }
        public string in_reply_to_screen_name { get; set; }
        [Indexed]
        public long user_id { get; set; }
        [Ignore]
        public string user_id_str { get; set; }
        //public object geo { get; set; }
        [JsonConverter(typeof(RawPropertyConverter))]
        public string coordinates { get; set; }
        [Ignore] // Bc we'll do this manually
        public Place place { get; set; }/*
        public object contributors { get; set; }*/
        public bool is_quote_status { get; set; }
        public long retweet_count { get; set; }
        public long favorite_count { get; set; }
        public long reply_count { get; set; }
        public long quote_count { get; set; }
        public long conversation_id { get; set; }
        public string conversation_id_str { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public bool possibly_sensitive { get; set; }
        public bool possibly_sensitive_editable { get; set; }
        public string lang { get; set; }
        public string supplemental_language { get; set; }/*
        public Self_Thread self_thread { get; set; }
        public Card card { get; set; }*/
    }



    public class Place
    {
        public BoundingBox bounding_box { get; set; }
    }
    public class BoundingBox
    {
        public string type { get; set; }
        //public double[][][] coordinates { get; set; }
        [JsonConverter(typeof(RawPropertyConverter))]
        public string coordinates { get; set; }
    }


    public class Extended_Entities
    {
        public Medium2[] media { get; set; }
        public Url[] urls { get; set; }
    }


    public class Entities
    {
        public Hashtag[] hashtags { get; set; }
        public object[] symbols { get; set; }
        public User_Mentions[] user_mentions { get; set; }
        public Url[] urls { get; set; }
        public Medium2[] media { get; set; }
    }

    public class Hashtag
    {
        public string text { get; set; }
        public long[] indices { get; set; }
    }

    public class User_Mentions
    {
        public string screen_name { get; set; }
        public string name { get; set; }
        public long id { get; set; }
        public string id_str { get; set; }
        public long[] indices { get; set; }
    }

    public class Url
    {
        // Generated:
        [Indexed]
        public long tweetId { get; set; }

        // API:
        [PrimaryKey,Indexed]
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        [Ignore]
        public long[] indices { get; set; }
    }

    public class Card
    {
        public string name { get; set; }
        public string url { get; set; }
        public string card_type_url { get; set; }
        public Binding_Values binding_values { get; set; }
        //public Users users { get; set; }
        public Card_Platform card_platform { get; set; }
    }

    public class Binding_Values
    {
        public Vanity_Url vanity_url { get; set; }
        public Player_Url player_url { get; set; }
        public App_Is_Free app_is_free { get; set; }
        public App_Price_Currency app_price_currency { get; set; }
        public App_Price_Amount app_price_amount { get; set; }
        public Domain domain { get; set; }
        public App_Num_Ratings app_num_ratings { get; set; }
        public App_Star_Rating app_star_rating { get; set; }
        public App_Name app_name { get; set; }
        public Player_Width player_width { get; set; }
        public Player_Height player_height { get; set; }
        public Site site { get; set; }
        public Title title { get; set; }
        public Description description { get; set; }
        public Player_Image_Small player_image_small { get; set; }
        public Player_Image player_image { get; set; }
        public Player_Image_Large player_image_large { get; set; }
        public Player_Image_X_Large player_image_x_large { get; set; }
        public Player_Image_Original player_image_original { get; set; }
        public Card_Url card_url { get; set; }
    }

    public class Vanity_Url
    {
        public string type { get; set; }
        public string string_value { get; set; }
        public string scribe_key { get; set; }
    }

    public class Player_Url
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class App_Is_Free
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class App_Price_Currency
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class App_Price_Amount
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class Domain
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class App_Num_Ratings
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class App_Star_Rating
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class App_Name
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class Player_Width
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class Player_Height
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class Site
    {
        public string type { get; set; }
        public User_Value user_value { get; set; }
        public string scribe_key { get; set; }
    }

    public class User_Value
    {
        public string id_str { get; set; }
        public object[] path { get; set; }
    }

    public class Title
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class Description
    {
        public string type { get; set; }
        public string string_value { get; set; }
    }

    public class Player_Image_Small
    {
        public string type { get; set; }
        public Image_Value image_value { get; set; }
    }

    public class Image_Value
    {
        public string url { get; set; }
        public long width { get; set; }
        public long height { get; set; }
        public object alt { get; set; }
    }

    public class Player_Image
    {
        public string type { get; set; }
        public Image_Value1 image_value { get; set; }
    }

    public class Image_Value1
    {
        public string url { get; set; }
        public long width { get; set; }
        public long height { get; set; }
        public object alt { get; set; }
    }

    public class Player_Image_Large
    {
        public string type { get; set; }
        public Image_Value2 image_value { get; set; }
    }

    public class Image_Value2
    {
        public string url { get; set; }
        public long width { get; set; }
        public long height { get; set; }
        public object alt { get; set; }
    }

    public class Player_Image_X_Large
    {
        public string type { get; set; }
        public Image_Value3 image_value { get; set; }
    }

    public class Image_Value3
    {
        public string url { get; set; }
        public long width { get; set; }
        public long height { get; set; }
        public object alt { get; set; }
    }

    public class Player_Image_Original
    {
        public string type { get; set; }
        public Image_Value4 image_value { get; set; }
    }

    public class Image_Value4
    {
        public string url { get; set; }
        public long width { get; set; }
        public long height { get; set; }
        public object alt { get; set; }
    }

    public class Card_Url
    {
        public string type { get; set; }
        public string string_value { get; set; }
        public string scribe_key { get; set; }
    }


    public class Entities1
    {
        public Url1 url { get; set; }
        public Description1 description { get; set; }
    }

    public class Url1
    {
        public Url2[] urls { get; set; }
    }

    public class Url2
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public long[] indices { get; set; }
    }

    public class Description1
    {
        public object[] urls { get; set; }
    }

    public class Profile_Image_Extensions_Media_Color
    {
        public Palette[] palette { get; set; }
    }

    public class Palette
    {
        public Rgb rgb { get; set; }
        public float percentage { get; set; }
    }

    public class Rgb
    {
        public long red { get; set; }
        public long green { get; set; }
        public long blue { get; set; }
    }

    public class Profile_Image_Extensions
    {
        public Mediastats mediaStats { get; set; }
    }

    public class Mediastats
    {
        public R r { get; set; }
        public long ttl { get; set; }
    }

    public class R
    {
        public object missing { get; set; }
    }

    public class Profile_Banner_Extensions_Media_Color
    {
        public Palette1[] palette { get; set; }
    }

    public class Palette1
    {
        public Rgb1 rgb { get; set; }
        public float percentage { get; set; }
    }

    public class Rgb1
    {
        public long red { get; set; }
        public long green { get; set; }
        public long blue { get; set; }
    }

    public class Profile_Banner_Extensions
    {
        public Mediastats1 mediaStats { get; set; }
    }

    public class Mediastats1
    {
        public R1 r { get; set; }
        public long ttl { get; set; }
    }

    public class R1
    {
        public object missing { get; set; }
    }

    public class Ext
    {
        public Highlightedlabel highlightedLabel { get; set; }
    }

    public class Highlightedlabel
    {
        public R2 r { get; set; }
        public long ttl { get; set; }
    }

    public class R2
    {
        public Ok ok { get; set; }
    }

    public class Ok
    {
    }

    public class Card_Platform
    {
        public Platform platform { get; set; }
    }

    public class Platform
    {
        public Device device { get; set; }
        public Audience audience { get; set; }
    }

    public class Device
    {
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Audience
    {
        public string name { get; set; }
        public object bucket { get; set; }
    }

    public class Medium
    {
        public long id { get; set; }
        public string id_str { get; set; }
        public long[] indices { get; set; }
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public string type { get; set; }
        public Original_Info original_info { get; set; }
        public Sizes sizes { get; set; }
    }

    public class Original_Info
    {
        public long width { get; set; }
        public long height { get; set; }
    }

    public class Sizes
    {
        public Thumb thumb { get; set; }
        public Small small { get; set; }
        public Large large { get; set; }
        public Medium1 medium { get; set; }
    }

    public class Thumb
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Small
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Large
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Medium1
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Medium2
    {
        // Self-generated:
        [Indexed]
        public long tweetId { get; set; }
        public DateTime created_time_from_id { get; set; }

        public string video_aspect_ratio { get; set; }
        public long? video_duration_millis { get; set; }

        // From API:
        [PrimaryKey,Indexed]
        public long id { get; set; }
        public string id_str { get; set; }/*
        public long[] indices { get; set; }
        */
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }

        public string type { get; set; }/*
        public Original_Info1 original_info { get; set; }
        public Sizes1 sizes { get; set; }
        */
        [Ignore]
        public Video_Info video_info { get; set; }/*
        public string media_key { get; set; }*/
        public string ext_alt_text { get; set; }/*
        public Ext_Media_Availability ext_media_availability { get; set; }
        public Ext_Media_Color ext_media_color { get; set; }
        public Ext1 ext { get; set; }
        public Additional_Media_Info additional_media_info { get; set; }*/
    }

    public class Original_Info1
    {
        public long width { get; set; }
        public long height { get; set; }
    }

    public class Sizes1
    {
        public Thumb1 thumb { get; set; }
        public Small1 small { get; set; }
        public Large1 large { get; set; }
        public Medium3 medium { get; set; }
    }

    public class Thumb1
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Small1
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Large1
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Medium3
    {
        public long w { get; set; }
        public long h { get; set; }
        public string resize { get; set; }
    }

    public class Video_Info
    {
        public long[] aspect_ratio { get; set; }
        public long? duration_millis { get; set; }
        public Variant[] variants { get; set; }
    }

    public class Variant
    {
        // Manual:
        [Indexed]
        public long mediumId { get; set; }

        // API:
        public long bitrate { get; set; }
        public string content_type { get; set; }
        [Unique]
        public string url { get; set; }
    }

    public class Ext_Media_Availability
    {
        public string status { get; set; }
    }

    public class Ext_Media_Color
    {
        public Palette2[] palette { get; set; }
    }

    public class Palette2
    {
        public Rgb2 rgb { get; set; }
        public float percentage { get; set; }
    }

    public class Rgb2
    {
        public long red { get; set; }
        public long green { get; set; }
        public long blue { get; set; }
    }

    public class Ext1
    {
        public Mediastats2 mediaStats { get; set; }
    }

    public class Mediastats2
    {
        public R3 r { get; set; }
        public long ttl { get; set; }
    }

    public class R3
    {
        public Ok1 ok { get; set; }
    }

    public class Ok1
    {
        public string viewCount { get; set; }
    }

    public class Additional_Media_Info
    {
        public string title { get; set; }
        public string description { get; set; }
        public bool embeddable { get; set; }
        public bool monetizable { get; set; }
    }

    public class Self_Thread
    {
        public long id { get; set; }
        public string id_str { get; set; }
    }



    public class Moments
    {
    }

    public class Cards
    {
    }

    public class Places
    {
    }

    public class Media
    {
    }

    public class Broadcasts
    {
    }

    public class Topics
    {
    }

    public class Lists
    {
    }

    public class Timeline
    {
        public string id { get; set; }
        public Instruction[] instructions { get; set; }
    }

    public class Instruction
    {
        public Addentries addEntries { get; set; }
        public Replaceentry replaceEntry { get; set; }
    }

    public class Replaceentry
    {
        public string entryIdToReplace { get; set; }
        public Entry entry { get; set; }
    }

    public class Addentries
    {
        public Entry[] entries { get; set; }
    }

    public class Entry
    {
        public string entryId { get; set; }
        public string sortIndex { get; set; }
        public Content content { get; set; }
    }

    public class Content
    {
        public Item item { get; set; }
        public Operation operation { get; set; }
    }

    public class Item
    {
        public Content1 content { get; set; }
        public Clienteventinfo clientEventInfo { get; set; }
    }

    public class Content1
    {
        public Tweet tweet { get; set; }
    }

    public class Highlights
    {
        public Texthighlight[] textHighlights { get; set; }
    }

    public class Texthighlight
    {
        public long startIndex { get; set; }
        public long endIndex { get; set; }
    }

    public class Clienteventinfo
    {
        public string component { get; set; }
        public string element { get; set; }
        public Details details { get; set; }
    }

    public class Details
    {
        public Timelinesdetails timelinesDetails { get; set; }
    }

    public class Timelinesdetails
    {
        public string controllerData { get; set; }
    }

    public class Operation
    {
        public Cursor cursor { get; set; }
    }

    public class Cursor
    {
        public string value { get; set; }
        public string cursorType { get; set; }
    }


    public class User
    {
        // Generated:


        [PrimaryKey, Indexed]
        public long id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        //[Ignore]
        //public UserDetails entities { get; set; }

        [JsonConverter(typeof(RawPropertyConverter))]
        public string entities { get; set; }
        public bool _protected { get; set; }
        public long followers_count { get; set; }
        public long fast_followers_count { get; set; }
        public long normal_followers_count { get; set; }
        public long friends_count { get; set; }
        public long listed_count { get; set; }
        public string created_at { get; set; }
        public long favourites_count { get; set; }
        public string utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public long statuses_count { get; set; }
        public long media_count { get; set; }
        public string lang { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
        public bool is_translation_enabled { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_image_extensions_alt_text { get; set; }
        //public object profile_image_extensions_media_availability { get; set; }
        [Ignore]
        public Profile_Image_Extensions_Media_Color profile_image_extensions_media_color { get; set; }

        [Ignore]
        public Profile_Image_Extensions profile_image_extensions { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool profile_use_background_image { get; set; }
        public bool has_extended_profile { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        [JsonConverter(typeof(RawPropertyConverter))]
        public string pinned_tweet_ids { get; set; }
        //public long?[] pinned_tweet_ids { get; set; }
        [JsonConverter(typeof(RawPropertyConverter))]
        public string pinned_tweet_ids_str { get; set; }
        //public string[] pinned_tweet_ids_str { get; set; }
        public bool has_custom_timelines { get; set; }
        /*
        public object can_dm { get; set; }
        public object following { get; set; }
        public object follow_request_sent { get; set; }
        public object notifications { get; set; }
        public object muting { get; set; }
        public object blocking { get; set; }
        public object blocked_by { get; set; }
        public object want_retweets { get; set; }*/
        public string advertiser_account_type { get; set; }
        [JsonConverter(typeof(RawPropertyConverter))]
        public string advertiser_account_service_levels { get; set; }
        //public string[] advertiser_account_service_levels { get; set; }
        public string profile_longerstitial_type { get; set; }
        public string business_profile_state { get; set; }
        public string translator_type { get; set; }
        [JsonConverter(typeof(RawPropertyConverter))]
        public string withheld_in_countries { get; set; }
        //public string[] withheld_in_countries { get; set; }
        //public object followed_by { get; set; }
        [Ignore]
        public Ext ext { get; set; }
        public bool require_some_consent { get; set; }
        public string profile_banner_url { get; set; }
        //public object profile_banner_extensions_alt_text { get; set; }
        //public object profile_banner_extensions_media_availability { get; set; }
        [Ignore]
        public Profile_Banner_Extensions_Media_Color profile_banner_extensions_media_color { get; set; }
        [Ignore]
        public Profile_Banner_Extensions profile_banner_extensions { get; set; }
    }

    public class UserDetails
    {
        public Description2 description { get; set; }
        public UrlMulti url { get; set; }
    }

    public class Description2
    {
        public Url[] urls { get; set; }
    }
    public class UrlMulti
    {
        public Url[] urls { get; set; }
    }










}