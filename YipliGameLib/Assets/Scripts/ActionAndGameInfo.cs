using System;
using UnityEngine;

namespace YipliFMDriverCommunication
{
    public static class ActionAndGameInfoManager
    {
        public static YipliUtils.PlayerActions GetActionEnumFromActionID(string actionID)
        {
            switch (actionID)
            {
                case "9GO5":
                    return YipliUtils.PlayerActions.LEFT;
                case "3KWN":
                    return YipliUtils.PlayerActions.RIGHT;
                case "PLW3":
                    return YipliUtils.PlayerActions.ENTER;
                case "UDH0":
                    return YipliUtils.PlayerActions.PAUSE;
                case "SWLO":
                    return YipliUtils.PlayerActions.RUNNING;
                case "7RCE":
                    return YipliUtils.PlayerActions.RUNNINGSTOPPED;
                case "9D6O":
                    return YipliUtils.PlayerActions.JUMP;
                case "DMEY":
                    return YipliUtils.PlayerActions.RIGHTMOVE;
                case "38UF":
                    return YipliUtils.PlayerActions.LEFTMOVE;
                case "EUOA":
                    return YipliUtils.PlayerActions.JUMPIN;
                case "QRTY":
                    return YipliUtils.PlayerActions.JUMPOUT;
                case "TIL3":
                    return YipliUtils.PlayerActions.TILES;
            }
            Debug.Log("Invalid action. Returning null Action ID.");
            return YipliUtils.PlayerActions.INVALID_ACTION;
        }

        public static string getActionIDFromActionName(YipliUtils.PlayerActions actionName)
        {
            switch (actionName)
            {
                case YipliUtils.PlayerActions.LEFT:
                    return "9GO5";

                case YipliUtils.PlayerActions.RIGHT:
                    return "3KWN";

                case YipliUtils.PlayerActions.ENTER:
                    return "PLW3";

                case YipliUtils.PlayerActions.PAUSE:
                    return "UDH0";

                case YipliUtils.PlayerActions.RUNNING:
                    return "SWLO";

                case YipliUtils.PlayerActions.RUNNINGSTOPPED:
                    return "7RCE";

                case YipliUtils.PlayerActions.JUMP:
                    return "9D6O";

                case YipliUtils.PlayerActions.RIGHTMOVE:
                    return "DMEY";

                case YipliUtils.PlayerActions.LEFTMOVE:
                    return "38UF";

                case YipliUtils.PlayerActions.JUMPIN:
                    return "EUOA";

                case YipliUtils.PlayerActions.JUMPOUT:
                    return "QRTY";

                case YipliUtils.PlayerActions.TILES:
                    return "TIL3";
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
                    YipliHelper.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "trapped":
                    YipliHelper.SetGameClusterId(1);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "joyfuljumps":
                    YipliHelper.SetGameClusterId(1);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "eggcatcher":
                    YipliHelper.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "low";
                    break;

                case "yiplirunner":
                    YipliHelper.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "rollingball":
                    YipliHelper.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "skater":
                    YipliHelper.SetGameClusterId(3);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "penguinpop":
                    YipliHelper.SetGameClusterId(4);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "treewarrior":
                    YipliHelper.SetGameClusterId(2);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "tugofwar":
                    YipliHelper.SetGameClusterId(4);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                case "matbeats":
                    YipliHelper.SetGameClusterId(5);
                    PlayerSession.Instance.intensityLevel = "medium";
                    break;

                default:
                    YipliHelper.SetGameClusterId(0);
                    PlayerSession.Instance.intensityLevel = "";
                    break;
            }
        }
        public static void SetYipliMultiplayerGameInfo(string strGameName)
        {
            switch (strGameName.ToLower())
            {
                
                case "penguinpop":
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerOneDetails.gameId = strGameName;
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerTwoDetails.gameId = strGameName;
                    YipliHelper.SetGameClusterId(4);
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerOneDetails.intensityLevel = "medium";
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerTwoDetails.intensityLevel = "medium";
                    break;

                case "treewarrior":
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerOneDetails.gameId = strGameName;
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerTwoDetails.gameId = strGameName;
                    YipliHelper.SetGameClusterId(2);
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerOneDetails.intensityLevel = "medium";
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerTwoDetails.intensityLevel = "medium";
                    break;

                case "tugofwar":
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerOneDetails.gameId = strGameName;
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerTwoDetails.gameId = strGameName;
                    YipliHelper.SetGameClusterId(4);
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerOneDetails.intensityLevel = "medium";
                    PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData.PlayerTwoDetails.intensityLevel = "medium";
                    break;

                default:
                    PlayerSession.Instance.intensityLevel = "";
                    break;
            }
        }
    }

    [Serializable]
    public class FmDriverResponseInfo
    {
        public int count;
        public double timestamp;
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
        public string action_id;          // Action ID-Unique ID for each action. Refer below table for all action IDs
        public string action_name;         //Action Name for debugging (Gamers should strictly check action ID)
        public string properties;          //Any properties action has - ex. Running could have Step Count, Speed
    }

    #region Multiplayer Classes

    [Serializable]
    public class FmDriverResponseInfoMP
    {
        public int count;
        public double timestamp;
        public MultiPlayerData[] playerdata;
    }
    [Serializable]
    public class MultiPlayerData
    {
        public int id;
        public int count;
        public FMResponse fmresponse;
    }

    #endregion
}


