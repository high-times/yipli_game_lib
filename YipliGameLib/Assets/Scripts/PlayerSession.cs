using Firebase.Database;

#if UNITY_STANDALONE_WIN
using FMInterface_Windows;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yipli.HttpMpdule;
using YipliFMDriverCommunication;

public class PlayerSession : MonoBehaviour
{
    private static bool httpOrFirebase = false; // if true use Http module or use firebase module for false value
    private static string callbackLevel;

    private static PlayerSession _instance;
    public static PlayerSession Instance { get { return _instance; } }

    public static bool HttpOrFirebase { get => httpOrFirebase; set => httpOrFirebase = value; }

    //Delegates for Firebase Listeners
    public delegate void OnDefaultMatChanged();
    public static event OnDefaultMatChanged NewMatFound;

    [Header("InformationPanel")]
    [SerializeField] private GameObject yipliInfoPanel = null;
    [SerializeField] private TextMeshProUGUI infoPaneltext = null;

    private void Awake()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.HTTPAwakeOperations();
        }
        else
        {
            PlayerSessionFB.Instance.PSFirebaseAwake();
        }
    }

    public void Start()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.StartOperations();
        }
        else
        {
            PlayerSessionFB.Instance.StartOperations();
        }
    }

    public void Update()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.UpdateOperations();
        }
        else
        {
            PlayerSessionFB.Instance.UpdateOperations();
        }
    }

    public string GetCurrentPlayer()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetCurrentPlayer();
        }
        else
        {
            return PlayerSessionFB.Instance.GetCurrentPlayer();
        }
    }

    public string GetCurrentPlayerId()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetCurrentPlayerId();
        }
        else
        {
            return PlayerSessionFB.Instance.GetCurrentPlayerId();
        }
    }

    public void ChangePlayer()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.ChangePlayer();
        }
        else
        {
            PlayerSessionFB.Instance.ChangePlayer();
        }
    }
    
    public void UpdateGameData(Dictionary<string, string> update)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.UpdateGameData(update);
        }
        else
        {
            PlayerSessionFB.Instance.UpdateGameData(update);
        }
    }

    //To be called from void awake/start of the games 1st scene
    public void SetGameId(string gameName)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.SetGameId(gameName);
        }
        else
        {
            PlayerSessionFB.Instance.SetGameId(gameName);
        }
    }

    // Get player game data
    public DataSnapshot GetGameData()
    {
        return PlayerSessionFB.Instance.GetGameData(); // TODO: treat differently for HTTP requests
    }

    //First function to be called only once when the game starts()
    //To be used for error handling
    //Call in case of exception while playing game.
    public void CloseSPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.CloseSPSession();
        }
        else
        {
            PlayerSessionFB.Instance.CloseSPSession();
        }
    }

    //to be called from all the player movment actions handled script
    //To be called from GameObject FixedUpdate
    public void UpdateDuration()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.UpdateDuration();
        }
        else
        {
            PlayerSessionFB.Instance.UpdateDuration();
        }
    }

    public void StartCoroutineForBleReConnection()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.StartCoroutineForBleReConnection();
        }
        else
        {
            PlayerSessionFB.Instance.StartCoroutineForBleReConnection();
        }
    }

    public void LoadingScreenSetActive(bool bOn)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.LoadingScreenSetActive(bOn);
        }
        else
        {
            PlayerSessionFB.Instance.LoadingScreenSetActive(bOn);
        }
    }

    // Update store data witout gameplay. To be called by games Shop Manager.
    public void UpdateStoreData(Dictionary<string, object> dStoreData)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.UpdateStoreData(dStoreData);
        }
        else
        {
            PlayerSessionFB.Instance.UpdateStoreData(dStoreData);
        }
    }

    public void GotoYipli()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.GotoYipli();
        }
        else
        {
            PlayerSessionFB.Instance.GotoYipli();
        }
    }

#region Single Player Session Functions

    public void ReInitializeSPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.ReInitializeSPSession();
        }
        else
        {
            PlayerSessionFB.Instance.ReInitializeSPSession();
        }
    }

    public IDictionary<YipliUtils.PlayerActions, int> getPlayerActionCounts()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.getPlayerActionCounts();
        }
        else
        {
            return PlayerSessionFB.Instance.getPlayerActionCounts();
        }
    }
    
    public Dictionary<string, dynamic> GetPlayerSessionDataJsonDic()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetPlayerSessionDataJsonDic();
        }
        else
        {
            return PlayerSessionFB.Instance.GetPlayerSessionDataJsonDic();
        }
    }

    public void StartSPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.StartSPSession();
        }
        else
        {
            PlayerSessionFB.Instance.StartSPSession();
        }
    }

    public void StoreSPSession(float gamePoints)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.StoreSPSession(gamePoints);
        }
        else
        {
            PlayerSessionFB.Instance.StoreSPSession(gamePoints);
        }
    }

    public void PauseSPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.PauseSPSession();
        }
        else
        {
            PlayerSessionFB.Instance.PauseSPSession();
        }
    }

    public void ResumeSPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.ResumeSPSession();
        }
        else
        {
            PlayerSessionFB.Instance.ResumeSPSession();
        }
    }

    public void AddPlayerAction(YipliUtils.PlayerActions action, int count = 1)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.AddPlayerAction(action, count);
        }
        else
        {
            PlayerSessionFB.Instance.AddPlayerAction(action, count);
        }
    }

    #endregion

    #region Multi Player Session Functions

    public IDictionary<YipliUtils.PlayerActions, int> getMultiPlayerActionCounts(PlayerDetails playerDetails)
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.getMultiPlayerActionCounts(playerDetails);
        }
        else
        {
            return PlayerSessionFB.Instance.getMultiPlayerActionCounts(playerDetails);
        }
    }
    public Dictionary<string, dynamic> GetMultiPlayerSessionDataJsonDic(PlayerDetails playerDetails, string mpSessionUUID)
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetMultiPlayerSessionDataJsonDic(playerDetails, mpSessionUUID);
        }
        else
        {
            return PlayerSessionFB.Instance.GetMultiPlayerSessionDataJsonDic(playerDetails, mpSessionUUID);
        }
    }
    public void StartMPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.StartMPSession();
        }
        else
        {
            PlayerSessionFB.Instance.StartMPSession();
        }
    }
    public void StoreMPSession(float playerOneGamePoints, float playerTwoGamePoints)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.StoreMPSession(playerOneGamePoints, playerTwoGamePoints);
        }
        else
        {
            PlayerSessionFB.Instance.StoreMPSession(playerOneGamePoints, playerTwoGamePoints);
        }
    }

    public void PauseMPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.PauseMPSession();
        }
        else
        {
            PlayerSessionFB.Instance.PauseMPSession();
        }
    }
    public void ResumeMPSession()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.ResumeMPSession();
        }
        else
        {
            PlayerSessionFB.Instance.ResumeMPSession();
        }
    }
    public void AddMultiPlayerAction(YipliUtils.PlayerActions action, PlayerDetails playerDetails, int count = 1)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.AddMultiPlayerAction(action, playerDetails, count);
        }
        else
        {
            PlayerSessionFB.Instance.AddMultiPlayerAction(action, playerDetails, count);
        }
    }

    #endregion

    // get game and driver version
    public string GetDriverAndGameVersion()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetDriverAndGameVersion();
        }
        else
        {
            return PlayerSessionFB.Instance.GetDriverAndGameVersion();
        }
    }

    // get fitness poins
    public float GetFitnessPoints()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetFitnessPoints();
        }
        else
        {
            return PlayerSessionFB.Instance.GetFitnessPoints();
        }
    }

    // get calories
    public float GetCaloriesBurned()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetCaloriesBurned();
        }
        else
        {
            return PlayerSessionFB.Instance.GetCaloriesBurned();
        }
    }

    // quit from playersession canvas
    public void QuitApplication()
    {
        Application.Quit();
    }

    // retake tutorial
    public void RetakeMatControlsTutorial()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.RetakeMatControlsTutorial();
        }
        else
        {
            PlayerSessionFB.Instance.RetakeMatControlsTutorial();
        }
    }

    public void YipliInfoPanleOkayButton()
    {
        // info panel text management
        infoPaneltext.text = "";
        yipliInfoPanel.SetActive(false);
    }

    // set mat play mode
    public void SetMatPlayMode()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.SetMatPlayMode();
        }
        else
        {
            PlayerSessionFB.Instance.SetMatPlayMode();
        }
    }

    // TroubleShoot System
    public void TroubleShootSystem()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.TroubleShootSystem();
        }
        else
        {
            PlayerSessionFB.Instance.TroubleShootSystem();
        }
    }

    // Ticket system
    // Update current ticket data.
    public void UpdateCurrentTicketData(Dictionary<string, object> currentTicketData)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.UpdateCurrentTicketData(currentTicketData);
        }
        else
        {
            PlayerSessionFB.Instance.UpdateCurrentTicketData(currentTicketData);
        }
    }

   // #if UNITY_STANDALONE_WIN
        // application quit systems
        void OnApplicationQuit()
        {
            #if UNITY_STANDALONE_WIN
                Debug.LogError("Inside OnApplicationQuit");
                DeviceControlActivity._disconnect();
                DeviceControlActivity.readThread.Abort();
            #elif UNITY_IOS
                InitBLE.DisconnectMat();
            #endif
        }
    //#endif

    // Application State Management
    void OnApplicationFocus(bool focus)
    {
#if UNITY_IOS
        if (focus)
        {
            Debug.LogError("Test Poc is in focus : " + focus + " .. Reconnecting mat");
            InitBLE.ConnectPeripheral(InitBLE.MAT_UUID);
        }
        else
        {
            Debug.LogError("Test Poc is in focus : " + focus + " .. Disconnecting mat");
            InitBLE.DisconnectMat();
        }
#endif
    }

    // Test functions
    public void PrintBundleIdentifier() {
        PlayerPrefs.SetString("skippedDate", new DateTime(2021, 07, 21).ToString());
    }

    private void TimeDifferenceManager() {
        PlayerPrefs.SetString("skippedDate", DateTime.Today.ToString());

        DateTime todaysDate = new DateTime(2021, 07, 21);
        DateTime skippedDate = DateTime.Parse(PlayerPrefs.GetString("skippedDate"));

        int totalDaysSinceLastSkipped = (int)(todaysDate - skippedDate).TotalDays;

        Debug.LogError("todaysDate : " + totalDaysSinceLastSkipped);
    }

    #region Game info operations

    public void SetSinglePlayerGameInfo(string intensityLevel, int clusterId)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.SetSinglePlayerGameInfo(intensityLevel, clusterId);
        }
        else
        {
            PlayerSessionFB.Instance.SetSinglePlayerGameInfo(intensityLevel, clusterId);
        }
    }

    public void SetMultiPlayerGameInfo(string strGameName, string intensityLevel, int clusterId)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.SetMultiplayerGameInfo(strGameName, intensityLevel, clusterId);
        }
        else
        {
            PlayerSessionFB.Instance.SetMultiplayerGameInfo(strGameName, intensityLevel, clusterId);
        }
    }

    public void SetOldFMResponseCount(int count)
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.SetOldFMResponseCount(count);
        }
        else
        {
            PlayerSessionFB.Instance.SetOldFMResponseCount(count);
        }
    }

    public int GetOldFMResponseCount()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.GetOldFMResponseCount();
        }
        else
        {
            return PlayerSessionFB.Instance.GetOldFMResponseCount();
        }
    }

    #endregion

    #region Yipli Config operations

    public bool GetOnlyMatPlayModeStatus()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.CurrentYipliConfig.OnlyMatPlayMode;
        }
        else
        {
            return PlayerSessionFB.Instance.currentYipliConfig.onlyMatPlayMode;
        }
    }

    public int GetPlayersMatTutDoneStatus()
    {
        if (HttpOrFirebase)
        {
            return HTTPPlayerSession.Instance.CurrentYipliConfig.CurrentPlayer.MatTutDone;
        }
        else
        {
            return PlayerSessionFB.Instance.currentYipliConfig.playerInfo.isMatTutDone;
        }
    }

    public void UpdateCurrentPlayersMatTutStatus()
    {
        if (HttpOrFirebase)
        {
            HTTPPlayerSession.Instance.UpdateCurrentPlayersMatTutStatus();
        }
        else
        {
            PlayerSessionFB.Instance.UpdateCurrentPlayersMatTutStatus();
        }
    }

    #endregion

    #region Scene management

    public void LaunchCallBackLevel()
    {
        SceneManager.LoadScene(callbackLevel);
    }

    #endregion
}