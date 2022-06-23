using UnityEngine;

namespace Yipli.HttpMpdule.Classes
{
    public class HTTPMetaDataClasses {}

    // Full profile data
    // Full json
    public class RequestedJson
    {
        public int status;
        public JsonBody body;
    }

    // Full Body
    public class JsonBody
    {
        public string query;
        public BodyResponse response;
    }

    // Response
    public class BodyResponse
    {
        // string data
        public string contact_no;
        public string country_code;
        public string current_mat_id;
        public string current_player_id;
        public string display_name;
        public string email;
        public string profile_pic_url;

        // int or numbers data
        public int created_on;

        // Boolean data
        public bool has_subcribed;

        // class data
        public RemotePlay remote_play;
        public Mat[] mats;
        public Player[] players;
    }
    
    // Remote Play data
    public class RemotePlay
    {
        public string remote_code;
        public long timestamp;
    }

    // Mat Class
    public class Mat
    {
        public string display_name;
        public string mac_ddress;
        public string mac_name;
        public string registered_on;
        public string status;
    }

    // Player Class
    public class Player
    {
        // number data
        public long added_on;
        public int mat_tut_done;

        // string data
        public string dob;
        public string gender;
        public string height;
        public string name;
        public string profile_pic_url;
        public string user_id;
        public string weight;

        // activity statistics
        public ActivityStatistics activity_statistics;
    }

    // Activity statics
    public class ActivityStatistics
    {
        // number data
    }
}