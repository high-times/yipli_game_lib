using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yipli.HttpMpdule.Classes;
using YipliFMDriverCommunication;

namespace Yipli.HttpMpdule
{
    public class HTTPMatController : MonoBehaviour
    {
        // required variables
        [Header("current yipli config and other script objects")]
        [SerializeField] HTTPYipliConfig currentYipliConfig = null;
        [SerializeField] HTTPPlayerSelection httpPlayerSelection = null;

        // fix switch cases
        const string LEFT = "left";
        const string RIGHT = "right";
        const string ENTER = "enter";

        // current button this value will keep changing with mat left and right
        [SerializeField] Button currentB;
        [SerializeField] Button mainButton;
        [SerializeField] private List<Button> currentMenuButtons; // list of current buttons for mat controls. This list will change based on current active panel
        [SerializeField] int currentButtonIndex = 0;

        YipliUtils.PlayerActions detectedAction;

        // canvas object for keeping the active button top to the list. Yhis part is only used for PlayerSelection panel.
        //[SerializeField] ScrollRect playerSelectionScrollRect;
        //[SerializeField] RectTransform playerSelectionContentRectTransform;

        // flag to check if current panel is playerselection panel or not.
        public bool isThisPlayerSelectionPanel = false;
        public bool isThisSwitchPlayerPanel = false;

        // flag to disable the MatControls when tutorial is active.
        bool isTutorialRunning = false;

        // string and int to store current playername and index
        string currentPlayerName = null;
        int currentPlayerIndex = -2;

        // colors
        [Header("Required Colors")]
        [SerializeField] private Color lightBlueBorder;
        [SerializeField] private Color tangerineBorder;

        [Header("UIMangers")]
        [SerializeField] private NewMatInputController newMatInputController = null;

        [Header("Player Selection Area")]
        [SerializeField] private GameObject playerLeft = null;
        [SerializeField] private GameObject playerMiddle = null;
        [SerializeField] private GameObject playerRight = null;
        [SerializeField] private GameObject switchPlayerPanelPlayerObject = null;
        [SerializeField] private Sprite defaultProfilePic = null;

        [Header("playerButtons")]
        [SerializeField] Button leftButton = null;
        [SerializeField] Button middleButton = null;
        [SerializeField] Button rightButton = null;

        // required getters and setters.
        public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }
        public bool IsThisPlayerSelectionPanel { get => isThisPlayerSelectionPanel; set => isThisPlayerSelectionPanel = value; }
        public bool IsTutorialRunning { get => isTutorialRunning; set => isTutorialRunning = value; }
        public string CurrentPlayerName { get => currentPlayerName; set => currentPlayerName = value; }
        public bool IsThisSwitchPlayerPanel { get => isThisSwitchPlayerPanel; set => isThisSwitchPlayerPanel = value; }

        void Start()
        {
            // set cluster id to 0 as it is the only cluster id needed till main menu arrives.
            SetProperClusterID(0);
            IsThisSwitchPlayerPanel = false;
            IsThisPlayerSelectionPanel = false;
        }

        void Update()
        {
            GetMatUIKeyboardInputs();
            ManageMatActions();
        }

        public void SetProperClusterID(int clusterID)
        {
            try
            {
                //Debug.LogError("provided clusterID : " + clusterID);
                YipliHelper.SetGameClusterId(clusterID);
            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong with setting the cluster id : " + e.Message);
            }
        }

        private void ManageMatActions()
        {
            string fmActionData = InitBLE.GetFMResponse();
            Debug.Log("Json Data from Fmdriver in matinput : " + fmActionData);

            FmDriverResponseInfo singlePlayerResponse = null;

            try {
                singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);
            } catch (System.Exception e) {
                Debug.Log("singlePlayerResponse is having problem : " + e.Message);
            }

            if (singlePlayerResponse == null) return;

            if (currentYipliConfig.OldFMResponseCount != singlePlayerResponse.count)
            {
                PlayerSession.Instance.SetOldFMResponseCount(singlePlayerResponse.count);

                DetectedAction = ActionAndGameInfoManager.GetActionEnumFromActionID(singlePlayerResponse.playerdata[0].fmresponse.action_id);

                switch(DetectedAction)
                {
                    // UI input executions
                    case YipliUtils.PlayerActions.LEFT:
                        ProcessMatInputs(LEFT);
                        break;

                    case YipliUtils.PlayerActions.RIGHT:
                        ProcessMatInputs(RIGHT);
                        break;

                    case YipliUtils.PlayerActions.ENTER:
                        ProcessMatInputs(ENTER);
                        break;

                    default:
                        Debug.LogError("Wrong Action is detected : " + DetectedAction.ToString());
                        break;
                }
            }
        }

        public void UpdateButtonList(List<Button> newButtons, int newCurrentButtonIndex, bool isPlayerSelectionPanel)
        {
            currentButtonIndex = newCurrentButtonIndex;
            currentMenuButtons = newButtons;

            IsThisPlayerSelectionPanel = isPlayerSelectionPanel;

            ManageCurrentButton(IsThisPlayerSelectionPanel);
        }

        private void ProcessMatInputs(string matInput)
        {
            switch (matInput)
            {
                case LEFT:

                    currentButtonIndex = GetPreviousButton();
                    ManageCurrentButton(IsThisPlayerSelectionPanel);
                    newMatInputController.EnableMatLeftButtonAnimator();
                    break;

                case RIGHT:

                    currentButtonIndex = GetNextButton();
                    ManageCurrentButton(IsThisPlayerSelectionPanel);
                    newMatInputController.EnableMatRightButtonAnimator();
                    break;

                case ENTER:

                    if (IsThisPlayerSelectionPanel)
                    {
                        currentPlayerName = playerMiddle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                        playerMiddle.GetComponent<Button>().onClick.Invoke();
                    }
                    else
                    {
                        mainButton.onClick.Invoke();
                    }
                    break;

                default:
                    Debug.Log("Wrong Input");
                    break;
            }
        }

        private int GetNextButton()
        {
            if (IsThisPlayerSelectionPanel) {
                if ((currentButtonIndex + 1) == currentYipliConfig.AllPlayersOfThisUser.Count)
                {
                    return 0;
                }
                else
                {
                    return currentButtonIndex + 1;
                }
            } else {
                if ((currentButtonIndex + 1) == currentMenuButtons.Count)
                {
                    return 0;
                }
                else
                {
                    return currentButtonIndex + 1;
                }
            }
        }

        private int GetPreviousButton()
        {
            if (IsThisPlayerSelectionPanel) {
                if (currentButtonIndex == 0)
                {
                    return currentYipliConfig.AllPlayersOfThisUser.Count - 1;
                }
                else
                {
                    return currentButtonIndex - 1;
                }
            } else {
                if (currentButtonIndex == 0)
                {
                    return currentMenuButtons.Count - 1;
                }
                else
                {
                    return currentButtonIndex - 1;
                }
            }
        }

        private void ManageCurrentButton(bool isPlayerSelectionPanel)
        {
            if (isPlayerSelectionPanel)
            {
                playerLeft.transform.GetChild(0).GetComponent<Animator>().SetTrigger("fadePlayerName"); // player name
                playerLeft.transform.GetChild(1).transform.GetChild(0).GetComponent<Animator>().SetTrigger("fadeProfilePic"); // player profile pic

                playerMiddle.transform.GetChild(0).GetComponent<Animator>().SetTrigger("fadePlayerName"); // player name
                playerMiddle.transform.GetChild(1).transform.GetChild(0).GetComponent<Animator>().SetTrigger("fadeProfilePic"); // player profile pic

                playerRight.transform.GetChild(0).GetComponent<Animator>().SetTrigger("fadePlayerName"); // player name
                playerRight.transform.GetChild(1).transform.GetChild(0).GetComponent<Animator>().SetTrigger("fadeProfilePic"); // player profile pic
            }
        }

        public void StartScrolling() {
            ScrollButtonList(currentButtonIndex);
        }

        private void GetMatUIKeyboardInputs()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ProcessMatInputs(LEFT);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ProcessMatInputs(RIGHT);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                ProcessMatInputs(ENTER);
            }
        }

        public void ScrollButtonList(int btnIndex)
        {
            if (currentYipliConfig.AllPlayersOfThisUser.Count == 1) {
                UpdateButtonObject(currentYipliConfig.AllPlayersOfThisUser[0], playerMiddle, 0);

                playerLeft.SetActive(false);
                playerRight.SetActive(false);
            } else {
                if (btnIndex == 0) {
                    UpdateButtonObject(currentYipliConfig.AllPlayersOfThisUser[GetPreviousButton()], playerLeft, -1);
                } else {
                    UpdateButtonObject(currentYipliConfig.AllPlayersOfThisUser[btnIndex - 1], playerLeft, -1);
                }

                UpdateButtonObject(currentYipliConfig.AllPlayersOfThisUser[btnIndex], playerMiddle, 0);

                if (btnIndex == currentYipliConfig.AllPlayersOfThisUser.Count) {
                    UpdateButtonObject(currentYipliConfig.AllPlayersOfThisUser[0], playerRight, 1);
                } else {
                    UpdateButtonObject(currentYipliConfig.AllPlayersOfThisUser[GetNextButton()], playerRight, 1);
                }
            }
        }

        private void UpdateButtonObject(PlayerInfo playerInfo, GameObject playerObject, int playerInspectorIndex) {
            playerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerInfo.Name;
            
            if (playerInfo.PlayerProfilePicIMG != null) {
                playerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = playerInfo.PlayerProfilePicIMG;
            } else {
                playerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = defaultProfilePic;
            }

            playerObject.transform.GetComponent<Button>().onClick.AddListener(httpPlayerSelection.SelectPlayer);

            switch(playerInspectorIndex) {
                case -1:
                    leftButton.onClick.AddListener(httpPlayerSelection.SelectPlayer);
                    break;

                case 0:
                    middleButton.onClick.AddListener(httpPlayerSelection.SelectPlayer);
                    break;

                case 1:
                    rightButton.onClick.AddListener(httpPlayerSelection.SelectPlayer);
                    break;

                default:
                    Debug.LogError("Wrong inspector index detected : " + playerInspectorIndex);
                    break;
            }
        }

        public void UpdateSwitchPlayerPanelPlayerObject() {
            switch(currentPlayerIndex) {
                case -1:
                    switchPlayerPanelPlayerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Continue with " + playerLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    switchPlayerPanelPlayerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = playerLeft.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite;
                    break;

                case 0:
                    switchPlayerPanelPlayerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Continue with " + playerMiddle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    switchPlayerPanelPlayerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = playerMiddle.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite;
                    break;

                case 1:
                    switchPlayerPanelPlayerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Continue with " + playerRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    switchPlayerPanelPlayerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = playerRight.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite;
                    break;

                default:
                    Debug.LogError("Wrong inspector index detected : " + currentPlayerIndex);
                    break;
            }
        }

        public GameObject GetCurrentButton()
        {
            return currentB.gameObject;
        }

        public void PlayerLeftButtonFunction() {
            currentPlayerName = playerLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            currentPlayerIndex = -1;
        }

        public void PlayerMiddleButtonFunction() {
            currentPlayerName = playerMiddle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            currentPlayerIndex = 0;
        }

        public void PlayerRightButtonFunction() {
            currentPlayerName = playerRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            currentPlayerIndex = 1;
        }

        public void ManualLeftButton() {
            ProcessMatInputs(LEFT);
        }

        public void ManualRightButton() {
            ProcessMatInputs(RIGHT);
        }
    }
}