using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YipliFMDriverCommunication;

public class MatInputController : MonoBehaviour
{
    const string LEFT = "left";
    const string RIGHT = "right";
    const string ENTER = "enter";

    [SerializeField] Button currentB;
    [SerializeField] private List<Button> currentMenuButtons;
    [SerializeField] int currentButtonIndex = 0;

    YipliUtils.PlayerActions detectedAction;

    [SerializeField] ScrollRect playerSelectionScrollRect;
    [SerializeField] RectTransform playerSelectionContentRectTransform;

    bool isThisPlayerSelectionPanel = false;

    public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }
    public bool IsThisPlayerSelectionPanel { get => isThisPlayerSelectionPanel; set => isThisPlayerSelectionPanel = value; }

    void Start()
    {
        SetProperClusterID(0);
    }

    void Update()
    {
        GetMatUIKeyboardInputs();
        //ManageMatActions();
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
        //string fmActionData = InitBLE.PluginClass.CallStatic<string>("_getFMResponse");
        string fmActionData = InitBLE.GetFMResponse();
        Debug.Log("Json Data from Fmdriver : " + fmActionData);

        /* New FmDriver Response Format
           {
              "count": 1,                 # Updates every time new action is detected
              "timestamp": 1597237057689, # Time at which response was packaged/created by Driver
              "playerdata": [                      # Array containing player data
                {
                  "id": 1,                         # Player ID (For Single-player-1 , Multiplayer it could be 1 or 2 )
                  "fmresponse": {
                    "action_id": "9D6O",           # Action ID-Unique ID for each action. Refer below table for all action IDs
                    "action_name": "Jump",         # Action Name for debugging (Gamers should strictly check action ID)
                    "properties": "null"           # Any properties action has - ex. Running could have Step Count, Speed
                  }
                },
                {null}
              ]
            }
        */

        FmDriverResponseInfo singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);

        if (singlePlayerResponse == null) return;

        if (PlayerSession.Instance.currentYipliConfig.oldFMResponseCount < singlePlayerResponse.count)
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

        if (Input.GetKeyDown(KeyCode.P))
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
