using UnityEngine;

namespace Yipli.HttpMpdule.Classes
{
    public class HTTPMetaDataClasses {}

    // Full profile data
    // Full json
    public class RequestedJson
    {
        public int Status;
        public JsonBody Body;
    }

    // Full Body
    public class JsonBody
    {
        public string Query;
        public UserData Response;
    }

    // Response
    [System.Serializable]
    public class UserData
    {
        // string data
        public string UserID;
        public string ContactNo;
        public string CountryCode;
        public string CurrentMatId;
        public string current_player_id;
        public string display_name;
        public string email;
        public string profile_pic_url;

        // int or numbers data
        public long created_on;

        // Boolean data
        public bool has_subcribed;

        // class data
        //public RemotePlay remote_play;
        //public Mat[] mats;
        //public Player[] players;

        // Methods
        public static UserData CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<UserData>(jsonString);
        }
    }
    
    // Remote Play data
    public class RemotePlay
    {
        public string RemoteCode;
        public long Timestamp;
    }

    // Mat Class
    [System.Serializable]
    public class MatData
    {
        public string MatFbId;
        public string DisplayName;
        public string MacAddress;
        public string MacName;
        public string RegisteredOn;
        public string Status;
    }

    // Player Class
    [System.Serializable]
    public class PlayerData
    {
        // number data
        public long added_on;
        public int mat_tut_done;

        // string data
        public string playerID;
        public string dob;
        public string gender;
        public string height;
        public string name;
        public string profile_pic_url;
        public string user_id;
        public string weight;

        // activity statistics
        //public ActivityStatistics activity_statistics;
    }

    // Game Data
    [System.Serializable]
    public class GameData
    {
        public string AndroidMinVersion;
        public string AndroidTvMinVersion;
        public string CurrentVersion;
        public string IconImgUrl;
        public string IosCurrentVersion;
        public string IosMinVersion;
        public string MaintenanceMessage;
        public string Name;
        public string OnlyMatPlayMode;
        public string OsListForMaintanence;
        public string StorageExeName;
        public string VersionUpdateMessage;
        public string WinMinVersion;
        public string WinRegistryKey;
        public string WinVersion;
        
        public GameType Type;

        public int DaysBeforeNextUpdatePrompt;
        public int IsGameUnderMaintenance;
    }
}