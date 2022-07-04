using Yipli.HttpMpdule.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace Yipli.HttpMpdule
{
    [CreateAssetMenu(fileName = "HTTP Config", menuName = "HTTP_Module/New Http Config", order = 0)]
    public class HTTPYipliConfig : ScriptableObject
    {
        [SerializeField] private GameData currentGameInfo = null;
        [SerializeField] private UserData currentUserInfo = null;
        [SerializeField] private List<PlayerInfo> allPlayersOfThisUser = new List<PlayerInfo>();
        [SerializeField] private List<MatData> allMatsOfThisUser = new List<MatData>();
        [SerializeField] private MatData currentActiveMatData = null;
        [SerializeField] private PlayerInfo currentPlayer = null;
        [SerializeField] private UrlData allUrls = null;

        [SerializeField] private string yipliAppDownloadUrl = null;
        [SerializeField] private string callbackLevel = null;

        [SerializeField] private bool bAllDataIsReceived = false;
        [SerializeField] private bool bIsInternetConnected = false;
        [SerializeField] private bool onlyMatPlayModeIsSet = false;
        [SerializeField] private bool onlyMatPlayMode = false;
        [SerializeField] private bool isDeviceAndroidTV = false;
        [SerializeField] private bool sceneLoadedDirectly = false;
        [SerializeField] private bool bIsChangePlayerCalled = false;
        [SerializeField] private bool skipNormalUpdateClicked = false;

        [SerializeField] private int oldFMResponseCount = 0;

        // getters and setter
        public GameData CurrentGameInfo { get => currentGameInfo; set => currentGameInfo = value; }
        public UserData CurrentUserInfo { get => currentUserInfo; set => currentUserInfo = value; }
        public List<PlayerInfo> AllPlayersOfThisUser { get => allPlayersOfThisUser; set => allPlayersOfThisUser = value; }
        public List<MatData> AllMatsOfThisUser { get => allMatsOfThisUser; set => allMatsOfThisUser = value; }
        public MatData CurrentActiveMatData { get => currentActiveMatData; set => currentActiveMatData = value; }
        public PlayerInfo CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public UrlData AllUrls { get => allUrls; set => allUrls = value; }

        public string YipliAppDownloadUrl { get => yipliAppDownloadUrl; set => yipliAppDownloadUrl = value; }
        public string CallbackLevel { get => callbackLevel; set => callbackLevel = value; }

        public bool BAllDataIsReceived { get => bAllDataIsReceived; set => bAllDataIsReceived = value; }
        public bool BIsInternetConnected { get => bIsInternetConnected; set => bIsInternetConnected = value; }
        public bool OnlyMatPlayModeIsSet { get => onlyMatPlayModeIsSet; set => onlyMatPlayModeIsSet = value; }
        public bool OnlyMatPlayMode { get => onlyMatPlayMode; set => onlyMatPlayMode = value; }
        public bool IsDeviceAndroidTV { get => isDeviceAndroidTV; set => isDeviceAndroidTV = value; }
        public bool SceneLoadedDirectly { get => sceneLoadedDirectly; set => sceneLoadedDirectly = value; }
        public bool BIsChangePlayerCalled { get => bIsChangePlayerCalled; set => bIsChangePlayerCalled = value; }
        public bool SkipNormalUpdateClicked { get => skipNormalUpdateClicked; set => skipNormalUpdateClicked = value; }

        public int OldFMResponseCount { get => oldFMResponseCount; set => oldFMResponseCount = value; }

        // Custome Operations
        public void ResetData()
        {
            CurrentGameInfo = null;
            CurrentUserInfo = null;
            AllPlayersOfThisUser = new List<PlayerInfo>();
            AllMatsOfThisUser = new List<MatData>();
            CurrentActiveMatData = null;
            CurrentPlayer = null;
            AllUrls = null;

            YipliAppDownloadUrl = null;
            CallbackLevel = null;

            BAllDataIsReceived = false;
            BIsInternetConnected = false;
            OnlyMatPlayModeIsSet = false;
            OnlyMatPlayMode = false;
            IsDeviceAndroidTV = false;
            SceneLoadedDirectly = false;
            BIsChangePlayerCalled = false;
            SkipNormalUpdateClicked = false;

            OldFMResponseCount = 0;
        }
    }
}
