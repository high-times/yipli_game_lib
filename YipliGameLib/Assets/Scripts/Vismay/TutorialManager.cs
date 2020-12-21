using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YipliFMDriverCommunication;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<Button> tuorialButtons;
    [SerializeField] Button continueButton;
    [SerializeField] Button currentB;

    [SerializeField] TextMeshProUGUI instructionOne;
    [SerializeField] TextMeshProUGUI instructionTwo;
    [SerializeField] TextMeshProUGUI finalInstruction;

    const string LEFT = "left";
    const string RIGHT = "right";
    const string ENTER = "enter";

    int currentButtonIndex = 0;
    YipliUtils.PlayerActions detectedAction;

    [SerializeField] bool leftTapDone = false;
    [SerializeField] bool rightTapDone = false;
    [SerializeField] bool enterDone = false;

    public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }

    private void Start()
    {
        currentButtonIndex = 1;
        ManageCurrentButton();

        instructionOne.text = "Do Left tap";
        instructionTwo.text = "";

        finalInstruction.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    void Update()
    {
        GetMatUIKeyboardInputs();
        //ManageMatActions();
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

    private void ManageMatActions()
    {
        string fmActionData = InitBLE.GetFMResponse();
        Debug.Log("Json Data from Fmdriver : " + fmActionData);

        FmDriverResponseInfo singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);

        if (singlePlayerResponse == null) return;

        if (PlayerSession.Instance.currentYipliConfig.oldFMResponseCount < singlePlayerResponse.count)
        {
            PlayerSession.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

            DetectedAction = ActionAndGameInfoManager.GetActionEnumFromActionID(singlePlayerResponse.playerdata[0].fmresponse.action_id);

            switch (DetectedAction)
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

    private void ManageCurrentButton()
    {
        for (int i = 0; i < tuorialButtons.Count; i++)
        {
            if (i == currentButtonIndex)
            {
                // animate button
                tuorialButtons[i].GetComponent<Animator>().enabled = true;
                currentB = tuorialButtons[i];
            }
            else
            {
                // do nothing
                tuorialButtons[i].transform.localScale = new Vector3(1f, 1f, 1f);
                tuorialButtons[i].GetComponent<Animator>().enabled = false;
            }
        }
    }

    private void ProcessMatInputs(string matInput)
    {
        switch (matInput)
        {
            case LEFT:
                currentButtonIndex = GetPreviousButton();
                ManageCurrentButton();
                ProgressTutorial(LEFT);
                break;

            case RIGHT:
                currentButtonIndex = GetNextButton();
                ManageCurrentButton();
                ProgressTutorial(RIGHT);
                break;

            case ENTER:
                ProgressTutorial(ENTER);
                break;

            default:
                Debug.Log("Wrong Input");
                break;
        }
    }

    private int GetNextButton()
    {
        if ((currentButtonIndex + 1) == tuorialButtons.Count)
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
            return tuorialButtons.Count - 1;
        }
        else
        {
            return currentButtonIndex - 1;
        }
    }

    private void ProgressTutorial(string providedAction)
    {
        if(providedAction == LEFT && !leftTapDone && !rightTapDone && !enterDone)
        {
            leftTapDone = true;
            instructionOne.text = "Do Right tap";
        }
        else if (providedAction == RIGHT && leftTapDone && !rightTapDone && !enterDone)
        {
            rightTapDone = true;
            instructionOne.text = "Jump to Click the Button";
        }
        else if (providedAction == ENTER && leftTapDone && rightTapDone && !enterDone)
        {
            enterDone = true;
            instructionOne.text = "";

            foreach (Button b in tuorialButtons)
            {
                b.gameObject.SetActive(false);
            }

            finalInstruction.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
        }
        else if (providedAction == ENTER && leftTapDone && rightTapDone && enterDone)
        {
            ContinueButton();
        }
    }

    public void ContinueButton()
    {
        Debug.Log("Continue Butoon clicked.");
    }
}
