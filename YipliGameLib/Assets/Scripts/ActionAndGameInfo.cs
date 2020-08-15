using System;
using UnityEngine;

namespace YipliFMDriverCummunication
{ 
    public static class ActionAndGameInfoManager
    {
        public static string getActionIDFromActionName(string actionName)
        {
            switch (actionName.ToLower())
            {
                case "left":
                    return "9GO5";

                case "right":
                    return "3KWN";

                case "enter":
                    return "PLW3";

                case "pause":
                    return "UDH0";

                case "running":
                    return "SWLO";

                case "running stopped":
                    return "7RCE";

                case "jump":
                    return "9D6O";

                case "right move":
                    return "DMEY";

                case "left move":
                    return "38UF";

                case "jump in":
                    return "EUOA";

                case "jump out":
                    return "QRTY";
            }

            Debug.Log("Invalid action. Returning null Action ID.");
            return null;
        }

        //Pass here name of the game
        public static void SetYipliGameInfo(string strGameName)
        {
            switch (strGameName.ToLower())
            {
                case "unleash":
                    PlayerSession.Instance.gameId = strGameName;
                    PlayerSession.Instance.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "trapped":
                    PlayerSession.Instance.gameId = strGameName;
                    PlayerSession.Instance.SetGameClusterId(1);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "joyfuljumps":
                    PlayerSession.Instance.gameId = strGameName;
                    PlayerSession.Instance.SetGameClusterId(1);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "eggcatcher":
                    PlayerSession.Instance.gameId = strGameName;
                    PlayerSession.Instance.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "low";
                    break;

                case "yiplirunner":
                    PlayerSession.Instance.gameId = strGameName;
                    PlayerSession.Instance.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "rollingball":
                    PlayerSession.Instance.gameId = strGameName;
                    PlayerSession.Instance.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "skater":
                    PlayerSession.Instance.gameId = strGameName;
                    PlayerSession.Instance.SetGameClusterId(3);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                default:
                    PlayerSession.Instance.gameId = "";
                    PlayerSession.Instance.intensityLevel = "";
                    break;
            }
        }
    }

    [Serializable]
    public class FmDriverResponseInfo
    {
        public int response_count;
        public double response_timestamp;
        public PlayerData[] playerdata;
    }

    [Serializable]
    public class PlayerData
    {
        public int id;
        public FMResponse fmresponse;
    }

    [Serializable]
    public class FMResponse
    {
        public string ation_id;          // Action ID-Unique ID for each action. Refer below table for all action IDs
        public string action_name;         //Action Name for debugging (Gamers should strictly check action ID)
        public string properties;          //Any properties action has - ex. Running could have Step Count, Speed
    }
}


