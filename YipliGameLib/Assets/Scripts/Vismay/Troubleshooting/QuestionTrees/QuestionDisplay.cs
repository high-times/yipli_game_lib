using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionDisplay : MonoBehaviour
{
    // required objects
    // Questions
    [Header("Question Lists")]
    [SerializeField] Questions gameQuestions;
    [SerializeField] Questions matQuestions;

    // yipli config
    [Header("Yipli config")]
    [SerializeField] YipliConfig currentYipliConfig;

    // Display Objects
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI solutionText;
    [SerializeField] TextMeshProUGUI didItWorkText;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    public void StartGameLibTroubleShoot()
    {
        //game is not crashed as application is running

        if (currentYipliConfig.playerInfo != null)
        {
            // not stuck on fetching player detailes
        }
        else
        {
            if (!YipliHelper.checkInternetConnection())
            {
                // ask user to turn on the internet
            }
            else
            {
                // check 1st if backend data is currupted or not

                // check for the game version of the current game.

                /*
                if (version is not up to date) { 
                    // ask user to update the game to the latest version
                }
                else { 
                    // ask game questions 10, 11, 12 one by one

                    // after this start mat troubleswhoot module
                }
                */
            }
        }

        if (YipliHelper.GetMatConnectionStatus() == "Connected")
        {
            // not stuck on no mat connection status
        }
        else
        {
#if UNITY_STANDALONE_WIN
            // check for silicon port availability

            /*
            if (not available) { 
                // ask user to connect mat via usb
            }
            */

#elif UNITY_ANDROID
            // check for phone ble status

            /*
            if (ble off) { 
                // ask user to turn on ble
            }
            else {
                // check if current mat is added in User account

                if (not added) {
                    // ask user to add mat through yipli app
                }
            }
            */
#endif

            /*
            else {
                // ask user to check for any backgroung running games in android

                // in windows check for processes of yipli games

                if (running any yipli games) {
                    // ask user to close all the game and restart the current game
                }
                else {
                    // check for the game version of the current game.

                    /*
                    if (version is not up to date) { 
                        // ask user to update the game to the latest version
                    }
                    else { 
                        // ask game questions 10, 11, 12 one by one

                        // after this start Mat troubleshoot module
                    }
                    */
                    /*
                }
            }
            */
        }
    }

    public bool IsStuckOnPlayerFetchingDetails()
    {
        return currentYipliConfig.playerInfo == null;
    }

    private void SetQuestionText(string question)
    {
        questionText.text = question;
    }

    private void SetSolutionText(string solution)
    {
        solutionText.text = solution;
    }

    private void EnableChoices(string yes = "YES", string no = "NO")
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        yesButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = yes;
        noButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = no;
    }

    private void DisableChoices(string yes = "YES", string no = "NO")
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }
}
