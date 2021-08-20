
using System.Collections.Generic;

namespace JSONModels.TwitterSearch
{
    public class Rootobject
    {
        public Globalobjects globalObjects { get; set; }
        //public Timeline timeline { get; set; }
    }

    public class Globalobjects
    {
        public Dictionary<string, Tweet> tweets { get; set; }
        //public Dictionary<string, Tweet> Tweet[] tweets { get; set; }
        /*public Users11 users { get; set; }
        public Moments moments { get; set; }
        public Cards cards { get; set; }
        public Places places { get; set; }
        public Media media { get; set; }
        public Broadcasts broadcasts { get; set; }
        public Topics topics { get; set; }
        public Lists lists { get; set; }*/
    }


    public class Tweet
    {
        /*
        public string created_at { get; set; }
        public long id { get; set; }
        public string id_str { get; set; }
        public string full_text { get; set; }
        public bool truncated { get; set; }
        public int[] display_text_range { get; set; }
        public Entities entities { get; set; }*/
        public Extended_Entities extended_entities { get; set; }/*
        public string source { get; set; }
        public object in_reply_to_status_id { get; set; }
        public object in_reply_to_status_id_str { get; set; }
        public object in_reply_to_user_id { get; set; }
        public object in_reply_to_user_id_str { get; set; }
        public object in_reply_to_screen_name { get; set; }
        public long user_id { get; set; }
        public string user_id_str { get; set; }
        public object geo { get; set; }
        public object coordinates { get; set; }
        public object place { get; set; }
        public object contributors { get; set; }
        public bool is_quote_status { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public int reply_count { get; set; }
        public int quote_count { get; set; }
        public long conversation_id { get; set; }
        public string conversation_id_str { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public bool possibly_sensitive { get; set; }
        public bool possibly_sensitive_editable { get; set; }
        public string lang { get; set; }
        public object supplemental_language { get; set; }
        public Self_Thread self_thread { get; set; }
        public Card card { get; set; }*/
    }


    public class Extended_Entities
    {
        public Medium2[] media { get; set; }
    }


    public class Entities
    {
        public Hashtag[] hashtags { get; set; }
        public object[] symbols { get; set; }
        public User_Mentions[] user_mentions { get; set; }
        public Url[] urls { get; set; }
    }

    public class Hashtag
    {
        public string text { get; set; }
        public int[] indices { get; set; }
    }

    public class User_Mentions
    {
        public string screen_name { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string id_str { get; set; }
        public int[] indices { get; set; }
    }

    public class Url
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public int[] indices { get; set; }
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
        public int width { get; set; }
        public int height { get; set; }
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
        public int width { get; set; }
        public int height { get; set; }
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
        public int width { get; set; }
        public int height { get; set; }
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
        public int width { get; set; }
        public int height { get; set; }
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
        public int width { get; set; }
        public int height { get; set; }
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
        public int[] indices { get; set; }
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
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }

    public class Profile_Image_Extensions
    {
        public Mediastats mediaStats { get; set; }
    }

    public class Mediastats
    {
        public R r { get; set; }
        public int ttl { get; set; }
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
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }

    public class Profile_Banner_Extensions
    {
        public Mediastats1 mediaStats { get; set; }
    }

    public class Mediastats1
    {
        public R1 r { get; set; }
        public int ttl { get; set; }
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
        public int ttl { get; set; }
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
        public int[] indices { get; set; }
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
        public int width { get; set; }
        public int height { get; set; }
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
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Small
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Large
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Medium1
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Medium2
    {/*
        public long id { get; set; }
        public string id_str { get; set; }
        public int[] indices { get; set; }
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }*/
        public string expanded_url { get; set; }
        public string type { get; set; }/*
        public Original_Info1 original_info { get; set; }
        public Sizes1 sizes { get; set; }
        public Video_Info video_info { get; set; }
        public string media_key { get; set; }
        public object ext_alt_text { get; set; }
        public Ext_Media_Availability ext_media_availability { get; set; }
        public Ext_Media_Color ext_media_color { get; set; }
        public Ext1 ext { get; set; }
        public Additional_Media_Info additional_media_info { get; set; }*/
    }

    public class Original_Info1
    {
        public int width { get; set; }
        public int height { get; set; }
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
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Small1
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Large1
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Medium3
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Video_Info
    {
        public int[] aspect_ratio { get; set; }
        public int duration_millis { get; set; }
        public Variant[] variants { get; set; }
    }

    public class Variant
    {
        public int bitrate { get; set; }
        public string content_type { get; set; }
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
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }

    public class Ext1
    {
        public Mediastats2 mediaStats { get; set; }
    }

    public class Mediastats2
    {
        public R3 r { get; set; }
        public int ttl { get; set; }
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
        public int startIndex { get; set; }
        public int endIndex { get; set; }
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



}