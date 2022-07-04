using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Yipli.HttpMpdule.Classes;
using UnityEngine.SceneManagement;

namespace Yipli.HttpMpdule
{
    public class HTTPPlayerSelection : MonoBehaviour
    {
        // required variables
        [Header("Scriptable Objects")]
        [SerializeField] private HTTPYipliConfig currentYipliConfig = null;
        [SerializeField] private HTTPMatManager httpMatManager = null;
        [SerializeField] private HTTPRequestManager httpRequestManager = null;
        [SerializeField] private HTTPMatController matInputController = null;
        [SerializeField] private NewMatInputController newMatInputController = null;
        [SerializeField] private HTTPNewUIManager newUIManager = null;

        [Header("UI Objects")]
        [SerializeField] private GameObject phoneAnimationOBJ = null;
        [SerializeField] private GameObject stickAnimationOBJ = null;
        [SerializeField] private GameObject pcAnimationOBJ = null;

        [Header("UI Panel")]
        [SerializeField] private GameObject noNetworkPanel = null;
        [SerializeField] private GameObject phoneHolderInfo = null;
        [SerializeField] private GameObject playerSelectionPanel = null;
        [SerializeField] private GameObject switchPlayerPanel = null;
        [SerializeField] private GameObject Minimum2PlayersPanel = null;
        [SerializeField] private GameObject GuestUserPanel = null;
        [SerializeField] private GameObject LaunchFromYipliAppPanel = null;
        [SerializeField] private GameObject LoadingPanel = null;
        [SerializeField] private GameObject TutorialPanel = null;

        [Header("Text Objects")]
        [SerializeField] private TextMeshProUGUI gameAndDriverVersionText = null;

        [Header("Player Selection Objects")]
        [SerializeField] private GameObject PlayersContainer = null;

        // private variables
        // Booleans
        private bool allowPhoneHolderAudioPlay = false;
        private bool startDataManagement = false;
        private bool matConnectionStarted = false;

        // Floats
        private float currentTimePassed = 0;

        // Lists
        private List<GameObject> generatedObjects = new List<GameObject>();

        public bool StartDataManagement { get => startDataManagement; set => startDataManagement = value; }

        // Unity Operations
        private void OnEnable()
        {
            //Todo: Can shift this to onEnable ? - tried with http module
            UpdateGameAndDriverVersionText();
        }

        // When the game starts
        private void Start()
        {
            //Todo : Verify that following line is not needed.
            //newMatInputController.DisableMatParentButtonAnimator();

            TurnOffAllDeviceSpecificTextObject();
        }

        private void Update()
        {
            if (!currentYipliConfig.BAllDataIsReceived) return;

            if (!currentYipliConfig.BIsInternetConnected) return;

            // TO DO : Move below operation in another file. Look for executions start(). - use flag -> get game info query completed
            if (!currentYipliConfig.OnlyMatPlayModeIsSet)
            {
                SetOnlyMatPlayMode();
            }

            // wait for only matplay mode to be set
            if (!currentYipliConfig.OnlyMatPlayModeIsSet) return;

            //Debug.LogError("onlyMatPlayMode : only mat play mode : " + currentYipliConfig.OnlyMatPlayMode);

            if (!currentYipliConfig.OnlyMatPlayMode)
            {
                if (currentYipliConfig.SceneLoadedDirectly) return;

                //Debug.LogError("onlyMatPlayMode : next line is matSelectionScript.LoadMainGameSceneDirectly();");
                httpMatManager.LoadMainGameSceneDirectly();
                return;
            }

            if (allowPhoneHolderAudioPlay)
            {
                PlayComeAndJumpAudio();
            }

            if (!playerSelectionPanel.activeSelf)
            {
                DestroyPlayerSelectionButtons();
            }

            if (playerSelectionPanel.activeSelf)
            {
                matInputController.IsThisPlayerSelectionPanel = true;
            }
            else
            {
                matInputController.IsThisPlayerSelectionPanel = false;
            }

            if (StartDataManagement)
            {
                InitializeAndStartPlayerSelectionNoCoroutine();
            }
        }

        // Custome Operations
        // turn of all devicespecific tutorial objects invisible
        private void TurnOffAllDeviceSpecificTextObject()
        {
            phoneAnimationOBJ.SetActive(false);
            stickAnimationOBJ.SetActive(false);
            pcAnimationOBJ.SetActive(false);
        }

        // no internet check coroutine
        private IEnumerator CheckNoInternetConnection()
        {
            //Todo : remove this and shift it to FirebaseHandler Listner function itself.
            //Set default valuye of currentYipliConfig.bIsInternetConnected to true, to avoid the Internet panel from coming for 1st fraction of second
            yield return new WaitForSecondsRealtime(8f);

            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);

                if (HTTPHelper.CheckInternetConnection()) // update this line with http ping
                {
                    //newUIManager.TurnOffMainCommonButton();
                    noNetworkPanel.SetActive(false);

                    //Time.timeScale = 0f; // pause everything
                }
                else
                {
                    //newUIManager.UpdateButtonDisplay(noNetworkPanel.tag);
                    noNetworkPanel.SetActive(true);

                    //Time.timeScale = 1f; // resume everything
                }
            }
        }

        private void UpdateGameAndDriverVersionText()
        {
            gameAndDriverVersionText.text = Application.version;
        }

        private void SetOnlyMatPlayMode()
        {
            if (currentYipliConfig.CurrentGameInfo == null) return;

            if (currentYipliConfig.CurrentGameInfo.OnlyMatPlayMode.Contains("a") && Application.platform == RuntimePlatform.Android)
            {
                //Debug.LogError("Executing a");
                currentYipliConfig.OnlyMatPlayMode = false;
            }
            else if (currentYipliConfig.CurrentGameInfo.OnlyMatPlayMode.Contains("s") && Application.platform == RuntimePlatform.Android && currentYipliConfig.IsDeviceAndroidTV)
            {
                //Debug.LogError("Executing atv");
                currentYipliConfig.OnlyMatPlayMode = false;
            }
            else if (currentYipliConfig.CurrentGameInfo.OnlyMatPlayMode.Contains("i") && Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //Debug.LogError("Executing i");
                currentYipliConfig.OnlyMatPlayMode = false;
            }
            else if (currentYipliConfig.CurrentGameInfo.OnlyMatPlayMode.Contains("w") && Application.platform == RuntimePlatform.WindowsPlayer)
            {
                // for testing in editor (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                //Debug.LogError("Executing w");
                currentYipliConfig.OnlyMatPlayMode = false;
            }
            // testing purpose only
            // else if (currentYipliConfig.gameInventoryInfo.onlyMatPlayMode.Contains("a") && Application.platform == RuntimePlatform.WindowsEditor)
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                currentYipliConfig.OnlyMatPlayMode = true;
            }
            else
            {
                // defalt value reset
                //Debug.LogError("Executing final else");
                currentYipliConfig.OnlyMatPlayMode = true;
            }

            currentYipliConfig.OnlyMatPlayModeIsSet = true;
        }

        private void PlayComeAndJumpAudio()
        {
            if (HTTPHelper.GetMatConnectionStatus().Equals("connected", System.StringComparison.OrdinalIgnoreCase)) // update this with httphelper
            {
                currentTimePassed += Time.deltaTime;

                if (currentTimePassed > 30f)
                {
                    phoneHolderInfo.GetComponent<AudioSource>().Play();
                }
            }
        }

        // player selection operations
        // destroy all generated objects
        public void DestroyPlayerSelectionButtons()
        {
            foreach (var obj1 in generatedObjects)
            {
                Destroy(obj1);
            }
        }

        private void InitializeAndStartPlayerSelectionNoCoroutine() {
            //if (currentYipliConfig.BIsChangePlayerCalled) return;

            // game info status check
            if (currentYipliConfig.CurrentGameInfo == null) return;

            //SetOnlyMatPlayMode();

            //Setting User Id in the scriptable Object
            if (string.IsNullOrEmpty(currentYipliConfig.CurrentUserInfo.UserID)) return;

            if (currentYipliConfig.CurrentPlayer == null)
            {
                InitDefaultPlayer();
            }

            // only once start the connection flow
            if (!matConnectionStarted && currentYipliConfig.OnlyMatPlayMode)
            {
                matConnectionStarted = true;
                httpMatManager.MatConnectionFlow();
            }

            // no need to execute further if mat connection is not available
            if (currentYipliConfig.OnlyMatPlayMode && !HTTPHelper.GetMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase)) return;

            //Special handling in case of Multiplayer games
            if (currentYipliConfig.CurrentGameInfo.ThisGameType == GameType.MULTIPLAYER_GAMING)
            {
                // Check if atleast 2 players are available for playing the multiplayer game
                if (currentYipliConfig.AllPlayersOfThisUser.Count < 2)
                {
                    //Set active a panel to handle atleast 2 players should be there to play
                    TurnOffAllPanels();
                    Minimum2PlayersPanel.SetActive(true);
                    //newUIManager.UpdateButtonDisplay(Minimum2PlayersPanel.tag);
                }
                else
                {
                    LoadingPanel.SetActive(false);
                    //Skip player selection as it will be handled by game side, directly go to the mat selection flow
                    // TODO : this needs to be updated to direct game scene launch. No need to go through entire connection process
                    //matSelectionScript.MatConnectionFlow();
                    httpMatManager.LoadMainGameSceneIfMatIsConnected();
                }
            }
            else
            {
                if (currentYipliConfig.CurrentPlayer != null && currentYipliConfig.CurrentPlayer.MatTutDone == 0 && currentYipliConfig.OnlyMatPlayMode)
                {
                    LoadingPanel.SetActive(false);
                    //Debug.LogError("Retake Tutorial : next line is playdevice specific tutorial");
                    playDeviceSpecificMatTutorial();
                }
                else
                {
                    SwitchPlayerFlow();
                }
            }

            Debug.LogError("At the end of InitializeAndStartPlayerSelectionNoCoroutine");
        }

        private void InitDefaultPlayer()
        {
            SelectPlayerBasedOnPlayerID(currentYipliConfig.CurrentUserInfo.CurrentPlayerId);
        }

        private void SelectPlayerBasedOnPlayerID(string currentPlayerId)
        {
            foreach (PlayerInfo tempPlayer in currentYipliConfig.AllPlayersOfThisUser) {
                if (tempPlayer.PlayerID.Equals(currentPlayerId)) {
                    currentYipliConfig.CurrentPlayer = tempPlayer;
                    break;
                }
            }
        }

        public void SwitchPlayerFlow()//Call this for every StartGame()/Game Session
        {
            Debug.Log("Checking current player.");

            TurnOffAllPanels();
            LoadingPanel.SetActive(true);

            if (currentYipliConfig.CurrentPlayer != null)
            {
                //This means we have the default Player info from backend.
                //In this case we need to call the player change screen and not the player selection screen
                Debug.LogError("Found current player : " + currentYipliConfig.CurrentPlayer.Name);

                //Since default player is there, directly go to the mat selection flow
                //matSelectionScript.MatConnectionFlow();
                httpMatManager.LoadMainGameSceneIfMatIsConnected();
            }
            else //Current player not found in Db.
            {
                //Force to switch player as no default player found.
                OnSwitchPlayerPress(true);
            }
        }

        public void OnSwitchPlayerPress(bool isInternalCall = false) /* If called internally that means no default player found.*/
        {
            TurnOffAllPanels();

            if (HTTPHelper.CheckInternetConnection())
            {
                PlayerSelectionFlow();
            }
        }

        public void PlayerSelectionFlow()
        {
            Debug.Log("In Player selection flow.");

            if (currentYipliConfig.AllPlayersOfThisUser.Count != 0) //Atleast 1 player found for the corresponding userId
            {
                Debug.Log("Player/s found from firebase : " + currentYipliConfig.AllPlayersOfThisUser.Count);

                try
                {
                    Quaternion spawnrotation = Quaternion.identity;
                    Vector3 playerTilePosition = PlayersContainer.transform.localPosition;
                }
                catch (Exception exp)
                {
                    Debug.Log("Exception in Adding player : " + exp.Message);
                    //Application.Quit();
                }
                TurnOffAllPanels();

                // set all button list for wheel scrolling
                // PlayersMenuObject.GetComponent<InfiniteWheel.InfiniteWheelController>().SetitemsList(allButtons);

                playerSelectionPanel.SetActive(true);
                newMatInputController.DisplayMainMat();
                newMatInputController.SetMatPlayerSelectionPosition();
                newMatInputController.KeepLeftNadRightButtonColorToOriginal();
                newMatInputController.DisplayChevrons();
                newMatInputController.DisplayLegs();
                newMatInputController.HideTextButtons();
            }
            else
            {
                Debug.Log("No player found from firebase.");
                TurnOffAllPanels();
                //zeroPlayersPanel.SetActive(true);
                // TODO : if no players are found
            }
        }

        // Panel specifics
        private void TurnOffAllPanels()
        {
            phoneHolderInfo.SetActive(false);
            switchPlayerPanel.SetActive(false);
            playerSelectionPanel.SetActive(false);
            Minimum2PlayersPanel.SetActive(false);
            noNetworkPanel.SetActive(false);
            GuestUserPanel.SetActive(false);
            LaunchFromYipliAppPanel.SetActive(false);
            LoadingPanel.SetActive(false);
            TutorialPanel.SetActive(false);

            newMatInputController.HideMainMat();
        }

        // mat Tutorial Specific
        public void playDeviceSpecificMatTutorial()
        {
            TurnOffAllPanels();
            
            phoneHolderInfo.SetActive(true);
            newUIManager.UpdateButtonDisplay(phoneHolderInfo.gameObject.tag);

            if (HTTPHelper.GetMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                phoneHolderInfo.GetComponent<AudioSource>().Play();
            }

            allowPhoneHolderAudioPlay = true;

    #if UNITY_ANDROID || UNITY_IOS
            //StartCoroutine(ChangeTextMessageAndoridPhone());
            if (currentYipliConfig.isDeviceAndroidTV)
            {
                PlayTVStickTutStartAnimation();
            }
            else
            {
                PlayPhoneTutStartAnimation();
            }
            //Debug.LogError("Retake tutorial : proper animation is activated");
    #elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            //StartCoroutine(ChangeTextMessageWindowsPC());
            PlayPCTutStartAnimation();
    #endif
        }

        void PlayPhoneTutStartAnimation()
        {
            phoneAnimationOBJ.SetActive(true);
        }

        void PlayTVStickTutStartAnimation()
        {
            stickAnimationOBJ.SetActive(true);
        }

        void PlayPCTutStartAnimation()
        {
            pcAnimationOBJ.SetActive(true);
        }

        public void OnJumpOnMat()
        {
            allowPhoneHolderAudioPlay = false;
            SceneManager.LoadScene("gameLibTutorial");
        }

        // Go to Yipli Stuff
        public void OnGoToYipliPress()
        {
            HTTPHelper.GoToYipli(ProductMessages.noPlayerAdded);
        }
        
        // Game Updates on Androif
        public void OnUpdateGameClick()
        {
            string gameAppId = Application.identifier;
            Debug.Log("App Id is : " + gameAppId);
            HTTPHelper.GoToPlaystoreUpdate(gameAppId);
        }

        // Internet connection Operations
        public void TryAgainInternetConnection()
        {
            if (HTTPHelper.CheckInternetConnection())
            {
                noNetworkPanel.SetActive(false);
            }
            else
            {
                //newUIManager.UpdateButtonDisplay(noNetworkPanel.tag);
                noNetworkPanel.SetActive(true);
            }
        }

        public void DataManagementIsFinished()
        {
            StartDataManagement = false;
            // Disable UI from Here;

            Debug.LogError($"DataManagement is done : {StartDataManagement}");
        }
    }
}