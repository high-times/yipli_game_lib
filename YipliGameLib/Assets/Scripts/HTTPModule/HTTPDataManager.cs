using Yipli.HttpMpdule.Classes;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Yipli.HttpMpdule
{
    public static class HTTPDataManager
    {
        public static string userJsonData = "{\n  \"UserID\": \"lC4qqZCFEaMogYswKjd0ObE6nD43\",\n  \"ContactNo\": \"7874579889\",\n  \"CountryCode\": \"+91\",\n  \"CurrentMatId\": \"-N0FBaBdH_MiOgPkWIjz\",\n  \"CurrentPlayerId\": \"-MSX--0uyqI7KgKmNOIY\",\n  \"DisplayName\": \"Vismay Patel\",\n  \"Email\": \"vismay@playyipli.com\",\n  \"HasSubcribed\": false,\n  \"ProfilePicUrl\": \"f6bdea10-bf6f-4fd1-b8f1-fc6b91bcff12\",\n  \"TotalPlayers\": 2\n}";
        
        public static string playerJsonData = "[\n    {\n        \"PlayerID\" : \"-MSX--0uyqI7KgKmNOIY\",\n        \"AddedOn\": 1612256903290,\n        \"Dob\": \"07-01-1990\",\n        \"Gender\": \"Female\",\n        \"Height\": \"172\",\n        \"MatTutDone\": 1,\n        \"Name\": \"Elden ring\",\n        \"ProfilePicUrl\": \"-MSX--0uyqI7KgKmNOIY.jpg\",\n        \"UserId\": \"lC4qqZCFEaMogYswKjd0ObE6nD43\",\n        \"Weight\": \"54\"\n    },\n    {\n        \"PlayerID\" : \"-MzpCjARFpSBzkPalPrr\",\n        \"AddedOn\": 1649086493404,\n        \"Dob\": \"01-01-1987\",\n        \"Gender\": \"Male\",\n        \"Height\": \"161\",\n        \"MatTutDone\": 1,\n        \"Name\": \"Hogwargs Legacy\",\n        \"ProfilePicUrl\": \"-MzpCjARFpSBzkPalPrr.jpg\",\n        \"UserId\": \"lC4qqZCFEaMogYswKjd0ObE6nD43\",\n        \"Weight\": \"39\"\n    }\n]";
        public static string currentMatJson = "{\n    \"MatFbId\" : \"-N0FBaBdH_MiOgPkWIjz\",\n    \"DisplayName\": \"Black yipli F4\",\n    \"MacAddress\": \"A4:DA:32:4F:BF:F4\",\n    \"MacName\": \"YIPLI-C8\"\n}";

        public static string gameDataJson = "{\n  \"AndroidMinVersion\": \"3.01.005\",\n  \"AndroidTvMinVersion\": \",\",\n  \"CurrentVersion\": \"4.03.063\",\n  \"DaysBeforeNextUpdatePrompt\": 6,\n  \"description\": \"Help yourself to get out of the jungle trap\",\n  \"IconImgUrl\": \"Trappedpng1.png\",\n  \"IosCurrentVersion\": \"4.03.064\",\n  \"IosMinVersion\": \"4.02.012\",\n  \"IsGameUnderMaintenance\": 1,\n  \"MaintenanceMessage\": \"Game is under maintenance\",\n  \"Name\": \"Trapped\",\n  \"OnlyMatPlayMode\": \",\",\n  \"OsListForMaintanence\": \",\",\n  \"StorageExeName\": \"Yipli_Trapped.exe\",\n  \"Type\": \"FITNESS\",\n  \"VersionUpdateMessage\": \",\",\n  \"WinMinVersion\": \"4.03.036\",\n  \"WinRegistryKey\": \"trapped\",\n  \"WinVersion\": \"4.03.066\"\n}";

        public static string urlDataJson = "{\n    \"YipliWebUrlIN\" : \"http://in.playyipli.com\",\n    \"YipliWebUrlUS\": \"http://www.playyipli.com\",\n    \"YipliAppWinDownloadUrl\": \"http://www.google.co.in\"\n}";
    }
}

// 973391