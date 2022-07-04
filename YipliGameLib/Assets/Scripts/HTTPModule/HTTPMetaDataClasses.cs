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
        public string CurrentPlayerId;
        public string DisplayName;
        public string Email;
        public string ProfilePicUrl;

        // int or numbers data
        public long CreatedOn;
        public int TotalPlayers;

        // Boolean data
        public bool HasSubcribed;
    }
    
    // Remote Play data
    public class RemotePlay
    {
        public string RemoteCode;
        public long TimeStamp;
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
    public class PlayerInfo
    {
        // number data
        public long AddedOn;
        public int MatTutDone;

        // string data
        public string PlayerID;
        public string Dob;
        public string Gender;
        public string Height;
        public string Name;
        public string ProfilePicUrl;
        public string UserId;
        public string Weight;

        // Sprite data
        public Sprite PlayerProfilePicIMG;
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
        
        public string Type;
        public GameType ThisGameType;

        public int DaysBeforeNextUpdatePrompt;
        public int IsGameUnderMaintenance;
    }

    // Url Data
    public class UrlData
    {
        public string YipliWebUrlIN = string.Empty;
        public string YipliWebUrlUS = string.Empty;
        public string YipliAppWinDownloadUrl = string.Empty;
    }
}