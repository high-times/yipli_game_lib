using Yipli.HttpMpdule.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace Yipli.HttpMpdule
{
    public class HTTPYipliConfig : ScriptableObject
    {
        [SerializeField] GameData currentGameInfo = null;
        [SerializeField] UserData currentUserInfo = null;
        [SerializeField] List<PlayerData> allPlayersOfThisUser = new List<PlayerData>();
        [SerializeField] List<MatData> allMatsOfThisUser = new List<MatData>();
        [SerializeField] MatData currentActiveMatData = null;
        [SerializeField] PlayerData currentPlayer = null;

        [SerializeField] string yipliAppDownloadUrl = null;
        [SerializeField] string callbackLevel = null;

        [SerializeField] bool bIsInternetConnected = false;
        [SerializeField] bool onlyMatPlayModeIsSet = false;
        [SerializeField] bool onlyMatPlayMode = false;
        [SerializeField] bool isDeviceAndroidTV = false;
        [SerializeField] bool sceneLoadedDirectly = false;
        [SerializeField] bool bIsChangePlayerCalled = false;

        // getters and setter
        public GameData CurrentGameInfo { get => currentGameInfo; set => currentGameInfo = value; }
        public UserData CurrentUserInfo { get => currentUserInfo; set => currentUserInfo = value; }
        public List<PlayerData> AllPlayersOfThisUser { get => allPlayersOfThisUser; set => allPlayersOfThisUser = value; }
        public List<MatData> AllMatsOfThisUser { get => allMatsOfThisUser; set => allMatsOfThisUser = value; }
        public MatData CurrentActiveMatData { get => currentActiveMatData; set => currentActiveMatData = value; }
        public PlayerData CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }

        public string YipliAppDownloadUrl { get => yipliAppDownloadUrl; set => yipliAppDownloadUrl = value; }
        public string CallbackLevel { get => callbackLevel; set => callbackLevel = value; }

        public bool BIsInternetConnected { get => bIsInternetConnected; set => bIsInternetConnected = value; }
        public bool OnlyMatPlayModeIsSet { get => onlyMatPlayModeIsSet; set => onlyMatPlayModeIsSet = value; }
        public bool OnlyMatPlayMode { get => onlyMatPlayMode; set => onlyMatPlayMode = value; }
        public bool IsDeviceAndroidTV { get => isDeviceAndroidTV; set => isDeviceAndroidTV = value; }
        public bool SceneLoadedDirectly { get => sceneLoadedDirectly; set => sceneLoadedDirectly = value; }
        public bool BIsChangePlayerCalled { get => bIsChangePlayerCalled; set => bIsChangePlayerCalled = value; }
    }
}
