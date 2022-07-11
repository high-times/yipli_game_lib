using com.sun.tools.corba.se.idl.toJavaPortable;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using YipliFMDriverCommunication;

namespace Yipli.HttpMpdule
{
    public class HTTPPlayerSession : MonoBehaviour
    {
        [Header("Required Scriptable and Script Objects")]
        [SerializeField] private HTTPYipliConfig currentYipliConfig = null;
        [SerializeField] private HTTPRequestManager httpRequestManager = null;
        [SerializeField] private YipliAudioManager yipliAudioManager = null;
        [SerializeField] private HTTPPlayerSelection httpPlayerSelection = null;

        [Header("UI Objects")]
        [SerializeField] private GameObject YipliBackgroundPanel = null;
        [SerializeField] private GameObject BleErrorPanel = null;
        [SerializeField] private GameObject retryBleConnectionButton = null;
        [SerializeField] private GameObject LoadingScreen = null;
        [SerializeField] private GameObject netErrorPanel = null;
        [SerializeField] private GameObject yipliInfoPanel = null;

        [Header("HTTP Module Objects")]
        [SerializeField] private GameObject[] primaryObject = null;

        [Header("Text Objects")]
        [SerializeField] private TextMeshProUGUI infoPaneltext = null;
        [SerializeField] private TextMeshProUGUI bleErrorText = null;
        //[SerializeField] private TextMeshProUGUI playerNameGreetingText = null;

        // private variables
        // Dictionaries
        private IDictionary<YipliUtils.PlayerActions, int> playerActionCounts; // to be updated by the player movements
        private IDictionary<string, string> playerGameData; // to be used to store the player gameData like Highscore, last played level etc.

        // Floats
        private float calories = 0f;
        private float fitnesssPoints = 0f;
        private float gamePoints = 0f;
        private float duration = 0f;

        // Integers
        private int xp = 0;

        // strings
        private string intensityLevel = "";

        // Booleans
        private bool bIsPaused = false;
        private bool bIsBleCheckRunning = false;

        // static variables
        private static HTTPPlayerSession _instance = null;
        public static HTTPPlayerSession Instance { get { return _instance; } }

        // Getters and Setters
        public HTTPYipliConfig CurrentYipliConfig { get => currentYipliConfig; set => currentYipliConfig = value; }
        public IDictionary<YipliUtils.PlayerActions, int> PlayerActionCounts { get => playerActionCounts; set => playerActionCounts = value; }
        public float Calories { get => calories; set => calories = value; }
        public float FitnesssPoints { get => fitnesssPoints; set => fitnesssPoints = value; }
        public float GamePoints { get => gamePoints; set => gamePoints = value; }
        public float Duration { get => duration; set => duration = value; }
        public int Xp { get => xp; set => xp = value; }
        public IDictionary<string, string> PlayerGameData { get => playerGameData; set => playerGameData = value; }
        public string IntensityLevel { get => intensityLevel; set => intensityLevel = value; }
        public bool BIsPaused { get => bIsPaused; set => bIsPaused = value; }
        public bool BIsBleCheckRunning { get => bIsBleCheckRunning; set => bIsBleCheckRunning = value; }

        // Unity operations
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("Destroying current instance of http playersession and reinitializing");
                Destroy(gameObject);
                _instance = this;
            }
            else
            {
                _instance = this;
            }
        }

        // Custome operations
        public void HttpAwakeOperations()
        {
            httpRequestManager.GatherAllData();

            ChangeStateOfAllPrimaryObjects(true);
        }

        public void ChangeStateOfAllPrimaryObjects(bool state)
        {
            foreach (GameObject obj in primaryObject)
            {
                obj.SetActive(state);
            }
        }

        public void AddMultiPlayerAction(YipliUtils.PlayerActions action, PlayerDetails playerDetails, int count)
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            Debug.Log("Adding action in current player session.");
            if (playerDetails.playerActionCounts.ContainsKey(action))
                playerDetails.playerActionCounts[action] = playerDetails.playerActionCounts[action] + count;
            else
                playerDetails.playerActionCounts.Add(action, count);

            playerDetails.calories += YipliUtils.GetCaloriesPerAction(action) * count;
            playerDetails.fitnesssPoints += YipliUtils.GetFitnessPointsPerAction(action) * count * UnityEngine.Random.Range(0.92f, 1.04f); // this is to hide direct mapping between calories and fitnesspoint. small random multiplier is added fitness points to keep it random on single action level
        }

        public void AddPlayerAction(YipliUtils.PlayerActions action, int count)
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            if (count < 1) return;

            Debug.Log("Adding action in current player session.");
            if (PlayerActionCounts.ContainsKey(action))
            {
                PlayerActionCounts[action] = PlayerActionCounts[action] + count;
            }
            else
            {
                PlayerActionCounts.Add(action, count);
            }

            FitnesssPoints += YipliUtils.GetFitnessPointsPerAction(action) * count * UnityEngine.Random.Range(0.92f, 1.04f); // this is to hide direct mapping between calories and fitnesspoint. small random multiplier is added fitness points to keep it random on single action level
            Calories += YipliUtils.GetCaloriesPerAction(action) * count;
        }

        public void ChangePlayer()
        {
            ChangeStateOfAllPrimaryObjects(true);

            httpPlayerSelection.PlayerSelectionFlow();
        }

        public void CloseSPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            //Destroy current player session data
            Calories = 0;
            FitnesssPoints = 0;
            GamePoints = 0;
            Duration = 0;
            Debug.Log("Aborting current player session.");
        }

        public float GetCaloriesBurned()
        {
            if (Calories < 1f)
            {
                return 1f;
            }

            return Calories;
        }

        public string GetCurrentPlayer()
        {
            return currentYipliConfig.CurrentPlayer.Name;
        }

        public string GetCurrentPlayerId()
        {
            return currentYipliConfig.CurrentPlayer.PlayerID;
        }

        public string GetDriverAndGameVersion()
        {
            return HTTPHelper.GetFMDriverVersion() + " : " + Application.version;
        }

        public float GetFitnessPoints()
        {
            return FitnesssPoints;
        }

        public IDictionary<YipliUtils.PlayerActions, int> getMultiPlayerActionCounts(PlayerDetails playerDetails)
        {
            return playerDetails.playerActionCounts;
        }

        public Dictionary<string, dynamic> GetMultiPlayerSessionDataJsonDic(PlayerDetails playerDetails, string mpSessionUUID)
        {
            Debug.Log("UUID= " + mpSessionUUID);

            Dictionary<string, dynamic> x;
            x = new Dictionary<string, dynamic>();

            x.Add("game-id", playerDetails.gameId);
            x.Add("user-id", playerDetails.userId);
            x.Add("player-id", playerDetails.playerId);
            x.Add("minigame-id", playerDetails.minigameId);
            x.Add("age", int.Parse(playerDetails.playerAge));
            x.Add("points", (int)playerDetails.points);
            x.Add("height", playerDetails.playerHeight);
            x.Add("duration", (int)playerDetails.duration);
            x.Add("intensity", playerDetails.intensityLevel);
            x.Add("player-actions", playerDetails.playerActionCounts);
            x.Add("timestamp", ServerValue.Timestamp);
            x.Add("mp-session-id", mpSessionUUID);
            x.Add("calories", (int)playerDetails.calories);
            x.Add("fitness-points", (int)playerDetails.fitnesssPoints);
            x.Add("xp", Xp);

            if (PlayerGameData != null)
            {
                if (PlayerGameData.Count > 0)
                {
                    x.Add("game-data", PlayerGameData);
                }
                else
                {
                    Debug.Log("Game-data is empty");
                }
            }
            else
            {
                Debug.Log("Game-data is null");
            }

            return x;
        }

        public int GetOldFMResponseCount()
        {
            return currentYipliConfig.OldFMResponseCount;
        }

        public IDictionary<YipliUtils.PlayerActions, int> getPlayerActionCounts()
        {
            return PlayerActionCounts;
        }

        public Dictionary<string, dynamic> GetPlayerSessionDataJsonDic()
        {
            Dictionary<string, dynamic> x;
            x = new Dictionary<string, dynamic>();
            x.Add("game-id", currentYipliConfig.GameID);
            x.Add("user-id", currentYipliConfig.CurrentUserInfo.UserID);
            x.Add("player-id", currentYipliConfig.CurrentPlayer.PlayerID);
            x.Add("age", int.Parse(HTTPHelper.CalculateAge(currentYipliConfig.CurrentPlayer.Dob)));
            x.Add("points", (int)gamePoints);
            x.Add("height", currentYipliConfig.CurrentPlayer.Height);
            x.Add("duration", (int)duration);
            x.Add("intensity", IntensityLevel);
            x.Add("player-actions", playerActionCounts);
            x.Add("timestamp", ServerValue.Timestamp);
            x.Add("calories", (int)GetCaloriesBurned());
            x.Add("fitness-points", (int)GetFitnessPoints());

            if (playerGameData != null)
            {
                if (playerGameData.Count > 0)
                {
                    x.Add("game-data", playerGameData);
                }
                else
                {
                    Debug.Log("Game-data is empty");
                }
            }
            else
            {
                Debug.Log("Game-data is null");
            }

            // firebase function ignores and deletes folowing 2 lines. Uncomment lines once FB function is updated
            //x.Add("os", Application.platform);
            //x.Add("game-version", GetDriverAndGameVersion());

            //Removed following, since mat-id and mac-address couldnt be got on windows
            //x.Add("mat-id", currentYipliConfig.matInfo.matId);
            //x.Add("mac-address", currentYipliConfig.matInfo.macAddress);

#if UNITY_ANDROID
        if (currentYipliConfig.isDeviceAndroidTV) {
            x.Add("os", "atv");
        } else {
            x.Add("os", "a");
        }
#elif UNITY_IOS
        x.Add("os", "i");
#elif UNITY_STANDALONE_WIN
            x.Add("os", "w");
#endif

            x.Add("game-version", Application.version);

            return x;
        }

        public void GotoYipli()
        {
            HTTPHelper.GoToYipli(ProductMessages.openYipliApp);
        }

        public void LoadingScreenSetActive(bool bOn)
        {
            Debug.Log("Loading Screen called : " + bOn);
            YipliBackgroundPanel.SetActive(bOn);
            LoadingScreen.SetActive(bOn);
        }

        public void PauseMPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            PauseSPSession();
        }

        public void PauseSPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            Debug.Log("Pausing current player session.");
            BIsPaused = true; // only set the paused flat to true. Fixed update will take care of halting the time counter
                              //Ble check
            if (!YipliHelper.GetMatConnectionStatus().Equals("Connected", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("In PauseSPSession : Ble disconnected");
                if (!BleErrorPanel.activeSelf)
                {
                    yipliAudioManager.Play("BLE_failure");
                    YipliBackgroundPanel.SetActive(true);
                    BleErrorPanel.SetActive(true);
                }
            }
        }

        public void ReInitializeSPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            gamePoints = 0;
            duration = 0;
            bIsPaused = false;
            ActionAndGameInfoManager.SetYipliGameInfo(currentYipliConfig.GameID);
        }

        public void ResumeMPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            ResumeSPSession();
        }

        public void ResumeSPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            Debug.Log("Resuming current player session.");
            BIsPaused = false;
        }

        public void RetakeMatControlsTutorial()
        {
            if (currentYipliConfig.OnlyMatPlayMode)
            {
                _instance.currentYipliConfig.CallbackLevel = SceneManager.GetActiveScene().name;
                //currentYipliConfig.bIsRetakeTutorialFlagActivated = true;
                //SceneManager.LoadScene("yipli_lib_scene");
                SceneManager.LoadScene("gameLibTutorial");
            }
            else
            {
                // info panel text management
                infoPaneltext.text = "Mat Tutorial is not available in preview mode.";
                yipliInfoPanel.SetActive(true);
            }
        }

        public void SetGameId(string gameName)
        {
            currentYipliConfig.GameID = gameName;
        }

        public void SetMatPlayMode()
        {
        #if UNITY_EDITOR
            currentYipliConfig.OnlyMatPlayMode = false;
        #elif UNITY_ANDROID || UNITY_IOS
            currentYipliConfig.OnlyMatPlayMode = true;
        #elif UNITY_STANDALONE_WIN
            currentYipliConfig.OnlyMatPlayMode = true;
        #endif
        }

        public void SetMultiplayerGameInfo(string strGameName, string intensityLevel, int clusterId)
        {
            HTTPHelper.SetGameClusterId(clusterId);

            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.minigameId = strGameName;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.minigameId = strGameName;

            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.intensityLevel = intensityLevel;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.intensityLevel = intensityLevel;
        }

        public void SetOldFMResponseCount(int count)
        {
            currentYipliConfig.OldFMResponseCount = count;
        }

        public void SetSinglePlayerGameInfo(string intensityLevel, int clusterId)
        {
            HTTPHelper.SetGameClusterId(clusterId);
            IntensityLevel = intensityLevel;
        }

        public void StartCoroutineForBleReConnection()
        {
            try
            {
                Debug.Log("In StartCoroutineForBleReConnection.");
                if (!BIsBleCheckRunning)
                    StartCoroutine(ReconnectBleFromGame());
            }
            catch (Exception e)
            {
                Debug.Log("Exception in Retrying ble connection." + e.Message);
            }
        }

        public void StartMPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            Debug.Log("Starting multi player session.");
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.playerActionCounts = new Dictionary<YipliUtils.PlayerActions, int>();
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.playerActionCounts = new Dictionary<YipliUtils.PlayerActions, int>();

            ActionAndGameInfoManager.SetYipliMultiplayerGameInfo(currentYipliConfig.Mp_GameStateManager.minigameId);

            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.points = 0;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.points = 0;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.duration = 0;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.duration = 0;
        
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.calories = 0;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.fitnesssPoints = 0;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.calories = 0;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.fitnesssPoints = 0;

            Duration = 0;

            BIsPaused = false;
        }

        public void StartOperations()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (currentYipliConfig.OnlyMatPlayModeIsSet && !currentYipliConfig.OnlyMatPlayMode) return;

            Debug.Log("Starting the BLE routine check in PlayerSession Start()");

            StartCoroutine(CheckInternetConnection());
        }

        public void StartSPSession()
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            Debug.Log("Starting current player session.");
            playerActionCounts = new Dictionary<YipliUtils.PlayerActions, int>();
            gamePoints = 0;
            duration = 0;
            bIsPaused = false;
            ActionAndGameInfoManager.SetYipliGameInfo(currentYipliConfig.GameID);
        }

        public void StoreMPSession(float playerOneGamePoints, float playerTwoGamePoints)
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            Debug.LogError("Duration is " + Duration);
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.duration = duration;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.duration = duration;

            Debug.Log("Storing current player session to backend database.");

            Debug.Log("Count Test- " + currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.playerActionCounts.Count + " , " + currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.playerActionCounts.Count);


            currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails.points = playerOneGamePoints;
            currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails.points = playerTwoGamePoints;

            string mpSessionUUID = Guid.NewGuid().ToString();

            if (0 == ValidateMPSessionBeforePosting(currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails))
            {
                //Store the session data to backend.
                //FirebaseDBHandler.PostMultiPlayerSession(Instance, currentYipliConfig.Mp_GameStateManager.playerData.PlayerOneDetails, mpSessionUUID, () => { Debug.Log("Session stored in db"); });
                // Redesign
                Debug.Log("Player 1 session stored successfully.");
            }
            else
            {
                Debug.Log("Session not posted : Validation failed for sessoin data.");
            }

            if (0 == ValidateMPSessionBeforePosting(currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails))
            {
                //Store the session data to backend.
                //FirebaseDBHandler.PostMultiPlayerSession(Instance, currentYipliConfig.Mp_GameStateManager.playerData.PlayerTwoDetails, mpSessionUUID, () => { Debug.Log("Session stored in db"); });
                // ReDesign
                Debug.Log("Player 2 session stored successfully.");
            }
            else
            {
                Debug.Log("Session not posted : Validation failed for sessoin data.");
            }
        }

        public void StoreSPSession(float gamePoints)
        {
            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                Debug.LogError("onlyMatPlayMode is on, returning");
                return;
            }

            Debug.Log("Storing current player session to backend database.");
            this.gamePoints = gamePoints;

            //Calories = YipliUtils.GetCaloriesBurned(getPlayerActionCounts());
            //FitnesssPoints = YipliUtils.GetFitnessPointsWithRandomization(getPlayerActionCounts());
            xp = YipliUtils.GetXP(Math.Ceiling(duration));

            if (0 == ValidateSessionBeforePosting())
            {
                //Store the session data to backend.
                //FirebaseDBHandler.PostPlayerSession(Instance, () => { Debug.Log("Session stored in db"); });
                // Redesign
                Debug.Log("Single player session stored successfully.");
            }
            else
            {
                Debug.Log("Session not posted : Validation failed for sessoin data.");
            }
        }

        public void TroubleShootSystem()
        {
            SceneManager.LoadScene("Troubleshooting");
        }

        public void UpdateCurrentTicketData(Dictionary<string, object> currentTicketData)
        {
            /* redesign
            FirebaseDBHandler.UpdateCurrentTicketData(
                currentYipliConfig.CurrentUserInfo.UserID,
                currentTicketData,
                () => { Debug.Log("Ticket Generated successfully"); }
            );
            */
        }

        public void UpdateDuration()
        {
            Debug.Log("Updating duration for current player session.");
            if (bIsPaused == false)
            {
                duration += Time.deltaTime;
            }

            if (YipliHelper.GetMatConnectionStatus().Equals("Connected", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("In UpdateDuration : Ble connected");
                if (BleErrorPanel.activeSelf)
                {
                    yipliAudioManager.Play("BLE_success");
                    BleErrorPanel.SetActive(false);
                    YipliBackgroundPanel.SetActive(false);
                }
            }
        }

        public void UpdateGameData(Dictionary<string, string> update)
        {
            if (update != null)
            {
                PlayerGameData = new Dictionary<string, string>();
                PlayerGameData = update;
            }
        }

        public void UpdateOperations()
        {
            if (currentYipliConfig.OnlyMatPlayModeIsSet && !currentYipliConfig.OnlyMatPlayMode) return;

            Debug.Log("Game Cluster Id : " + YipliHelper.GetGameClusterId());

            if (currentYipliConfig.OnlyMatPlayMode)
            {
                CheckMatConnection();
            }
        }

        public void UpdateStoreData(Dictionary<string, object> dStoreData)
        {
            /* Redesign
            FirebaseDBHandler.UpdateStoreData(
                currentYipliConfig.CurrentUserInfo.UserID,
                currentYipliConfig.CurrentPlayer.PlayerID,
                currentYipliConfig.GameID,
                dStoreData,
                () => { Debug.Log("Got Game data successfully"); }
            );
            */
        }

        public void UpdateCurrentPlayersMatTutStatus()
        {
            // FirebaseDBHandler.UpdateTutStatusData(currentYipliConfig.CurrentUserInfo.UserID, currentYipliConfig.CurrentPlayer.PlayerID, 1);
            // Redesign
        }

        // Private Functions and Coroutines
        private IEnumerator ReconnectBleFromGame()
        {
            bIsBleCheckRunning = true;
            retryBleConnectionButton.SetActive(false);
            Debug.Log("In ReconnectBleFromGame.");
            try
            {
                //Initiate mat connection with last set GameCluterId
                Debug.Log("ReconnectBle with Game clster ID : " + YipliHelper.GetGameClusterId());
#if UNITY_ANDROID
                InitBLE.InitBLEFramework(currentYipliConfig.CurrentActiveMatData.MacAddress ?? "", YipliHelper.GetGameClusterId() != 1000 ? YipliHelper.GetGameClusterId() : 0, currentYipliConfig.CurrentActiveMatData.MatName ?? LibConsts.MatTempAdvertisingNameOnlyForNonIOS, currentYipliConfig.IsDeviceAndroidTV);
#elif UNITY_IOS
                InitBLE.InitBLEFramework(currentYipliConfig.CurrentActiveMatData.MacAddress ?? "", 0, currentYipliConfig.CurrentActiveMatData.MatName ?? LibConsts.MatTempAdvertisingNameOnlyForNonIOS);
#else
                InitBLE.InitBLEFramework(currentYipliConfig.CurrentActiveMatData.MacAddress ?? "", 0);
                //InitBLE.reconnectMat();
#endif
            }
            catch (Exception exp)
            {
                Debug.Log("Exception in InitBLEFramework from ReconnectBleFromGame" + exp.Message);
            }

            //Block this function for next 5 seconds by disabling the retry Button.
            //Dont allow user to initiate Bluetooth connection for atleast 5 secs, as 1 connecteion initiation is enough.
            yield return new WaitForSecondsRealtime(5f);
            retryBleConnectionButton.SetActive(true);
            bIsBleCheckRunning = false;
        }

        private int ValidateMPSessionBeforePosting(PlayerDetails playerDetails)
        {
            if (playerDetails.gameId == null || playerDetails.gameId == "")
            {
                Debug.Log("gameId is not set");
                return -1;
            }
            if (playerDetails.playerId == null || playerDetails.playerId == "")
            {
                Debug.Log("playerId is not set");
                return -1;
            }
            if (playerDetails.playerActionCounts.Count == 0)
            {
                Debug.Log("playerActionCounts is not set");
                return -1;
            }
            /* TODO : Confirm with kurus
            else if (playerDetails.playerActionCounts.Count < 1)
            {
                playerDetails.calories = 1;
            }
            */

            if (playerDetails.duration == 0)
            {
                Debug.Log("duration is 0");
                return -1;
            }
            return 0;
        }

        private int ValidateSessionBeforePosting()
        {
            if (string.IsNullOrEmpty(currentYipliConfig.GameID))
            {
                Debug.Log("gameId is not set");
                return -1;
            }

            if (string.IsNullOrEmpty(currentYipliConfig.CurrentPlayer.PlayerID))
            {
                Debug.Log("playerId is not set");
                return -1;
            }

            if (PlayerActionCounts.Count == 0)
            {
                Debug.Log("playerActionCounts is not set");
                return -1;
            }

            if (Duration == 0)
            {
                Debug.Log("duration is 0");
                return -1;
            }

            if (IntensityLevel == "")
            {
                Debug.Log("intensityLevel is not set");
                return -1;
            }

            return 0;
        }

        private void CheckMatConnection()
        {
            Debug.Log("Before Processing : BleErrorPanel.activeSelf = " + BleErrorPanel.activeSelf);

            if (YipliHelper.GetMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Mat connection is established.");

                if (BleErrorPanel.activeSelf)
                {
                    YipliBackgroundPanel.SetActive(false);
                    BleErrorPanel.SetActive(false);
                    yipliAudioManager.Play("BLE_success");
                }
            }
            else
            {
                Debug.Log("Mat connection is lost.");
                if (!BleErrorPanel.activeSelf)
                {
                    // Different mat connection (error)message based on Operating system and connectivity type.
#if UNITY_ANDROID
                if (currentYipliConfig.IsDeviceAndroidTV) {
                    bleErrorText.text = ProductMessages.Err_mat_connection_android_tv;
                } else {
                    bleErrorText.text = ProductMessages.Err_mat_connection_android_phone;
                }
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
                    bleErrorText.text = ProductMessages.Err_mat_connection_pc;
#endif

                    yipliAudioManager.Play("BLE_failure");
                    YipliBackgroundPanel.SetActive(true);
                    BleErrorPanel.SetActive(true);
                }
            }
        }

        // network connection panel management
        private IEnumerator CheckInternetConnection()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);

                if (YipliHelper.checkInternetConnection())
                {
                    if (netErrorPanel.activeSelf)
                    {
                        YipliBackgroundPanel.SetActive(false);
                        netErrorPanel.SetActive(false);
                        yipliAudioManager.Play("BLE_success");
                    }
                }
                else
                {
                    Debug.Log("Internect connection is lost.");
                    if (!netErrorPanel.activeSelf)
                    {
                        yipliAudioManager.Play("BLE_failure");
                        YipliBackgroundPanel.SetActive(true);
                        netErrorPanel.SetActive(true);
                    }
                }
            }
        }
    }
}