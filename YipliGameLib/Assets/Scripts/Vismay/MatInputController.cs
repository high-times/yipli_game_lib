using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YipliFMDriverCommunication;

public class MatInputController : MonoBehaviour
{
    // required variables
    [Header("current yipli config")]
    public YipliConfig currentYipliConfig;

    // fix switch cases
    const string LEFT = "left";
    const string RIGHT = "right";
    const string ENTER = "enter";

    // current button this value will keep changing with mat left and right
    [SerializeField] Button currentB;
    [SerializeField] private List<Button> currentMenuButtons; // list of current buttons for mat controls. This list will change based on current active panel
    [SerializeField] int currentButtonIndex = 0;

    YipliUtils.PlayerActions detectedAction;

    // canvas object for keeping the active button top to the list. Yhis part is only used for PlayerSelection panel.
    [SerializeField] ScrollRect playerSelectionScrollRect;
    [SerializeField] RectTransform playerSelectionContentRectTransform;

    // flag to check if current panel is playerselection panel or not.
    bool isThisPlayerSelectionPanel = false;

    // flag to disable the MatControls when tutorial is active.
    bool isTutorialRunning = false;

    // string to store current playername
    string currentPlayerName = null;

    // required getters and setters.
    public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }
    public bool IsThisPlayerSelectionPanel { get => isThisPlayerSelectionPanel; set => isThisPlayerSelectionPanel = value; }
    public bool IsTutorialRunning { get => isTutorialRunning; set => isTutorialRunning = value; }
    public string CurrentPlayerName { get => currentPlayerName; set => currentPlayerName = value; }

    void Start()
    {
        // set cluster id to 0 as it is the only cluster id needed till main menu arrives.
        SetProperClusterID(0);
    }

    void Update()
    {
        // mat and keyboard controls will be stopped when tutorial is active.
        if (!IsTutorialRunning)
        {
            GetMatUIKeyboardInputs();
            ManageMatActions();
        }
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

        FmDriverResponseInfo singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);

        if (singlePlayerResponse == null) return;

        if (currentYipliConfig.oldFMResponseCount != singlePlayerResponse.count)
        {
            PlayerSession.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

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
        /*if (currentMenuButtons != null)
        {
            currentMenuButtons.Clear();
        }*/
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
                break;

            case RIGHT:
                currentButtonIndex = GetNextButton();
                ManageCurrentButton(IsThisPlayerSelectionPanel);
                break;

            case ENTER:
                currentB.onClick.Invoke();
                break;

            default:
                Debug.Log("Wrong Input");
                break;
        }
    }

    private int GetNextButton()
    {
        if ((currentButtonIndex + 1) == currentMenuButtons.Count)
        {
            return 0;
        }
        else
        {
            return currentButtonIndex + 1;
        }
    }

    private int GetPreviousButton()
    {
        if (currentButtonIndex == 0)
        {
            return currentMenuButtons.Count - 1;
        }
        else
        {
            return currentButtonIndex - 1;
        }
    }

    private void ManageCurrentButton(bool isPlayerSelectionPanel)
    {
        if (isPlayerSelectionPanel)
        {
            for (int i = 0; i < currentMenuButtons.Count; i++)
            {
                if (i == currentButtonIndex)
                {
                    // animate button
                    currentMenuButtons[i].GetComponent<Image>().color = Color.black;
                    currentMenuButtons[i].transform.GetChild(0).GetComponent<Animator>().enabled = true;
                    currentB = currentMenuButtons[i];

                    CurrentPlayerName = currentB.name;

                    ScrollButtonList(currentB);
                }
                else
                {
                    // do nothing
                    currentMenuButtons[i].GetComponent<Image>().color = Color.white;
                    currentMenuButtons[i].transform.GetChild(0).GetComponent<Animator>().enabled = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < currentMenuButtons.Count; i++)
            {
                if (i == currentButtonIndex)
                {
                    // animate button
                    currentMenuButtons[i].GetComponent<Animator>().enabled = true;
                    currentB = currentMenuButtons[i];
                }
                else
                {
                    // do nothing
                    currentMenuButtons[i].transform.localScale = new Vector3(1f, 1f, 1f);
                    currentMenuButtons[i].GetComponent<Animator>().enabled = false;
                }
            }
        }
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            ProcessMatInputs(ENTER);
        }
    }

    private void ScrollButtonList(Button currentButton)
    {
        Canvas.ForceUpdateCanvases();
        playerSelectionContentRectTransform.anchoredPosition =
            (Vector2)playerSelectionScrollRect.transform.InverseTransformPoint(playerSelectionContentRectTransform.position)
            - (Vector2)playerSelectionScrollRect.transform.InverseTransformPoint(currentButton.transform.position);
        playerSelectionContentRectTransform.anchoredPosition = new Vector2(0f, playerSelectionContentRectTransform.anchoredPosition.y - 50f);
    }

    public GameObject GetCurrentButton()
    {
        return currentB.gameObject;
    }
}
