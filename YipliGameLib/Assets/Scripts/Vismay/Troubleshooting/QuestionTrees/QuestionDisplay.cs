using UnityEngine;
using TMPro;
using UnityEngine.UI;
using yipli.Windows;

public class QuestionDisplay : MonoBehaviour
{
    // required objects
    // Questions
    [Header("Question Lists")]
    [SerializeField] Questions gameQuestions;
    [SerializeField] Questions matQuestions;
    [SerializeField] TroubleShootManagerS troubleshootManager;

    // yipli config
    [Header("Yipli config")]
    [SerializeField] YipliConfig currentYipliConfig;

    // Display Objects
    [Header("UI Elements")]
    // text elements
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI solutionText;
    // buttons
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button notSureButton;

    [Header("UI Panels")]
    // gameobjects
    [SerializeField] GameObject animationParent;
    [SerializeField] GameObject entryPanel;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject questionAnswerPage;
    [SerializeField] GameObject practicalTaskPanel;

    const string siliconLabsDesc = "Silicon Labs CP210x USB to UART Bridge";
    const string siliconLabsManufacturer = "Silicon Labs";

    private bool yesButtonClicked = false;
    private bool notSureButtonClicked = false;
    private bool noButtonClicked = false;

    // questions flags
    private bool questionAsked = false;
    private bool solutionProvided = false;

    public bool QuestionAsked { get => questionAsked; set => questionAsked = value; }
    public bool SolutionProvided { get => solutionProvided; set => solutionProvided = value; }

    private void Start()
    {
        TurnOffAllPanels();
        TurnOffEntryPanel();
    }

    #region gameQuestions
    // game specific questions
    // Game question 2
    public void IsOsUpdatedToLates()
    {
        SetGameQuestionText(2);

        troubleshootManager.OsUpdateCheck = true;
    }

    // Game question 3
    public void IsStuckOnPlayerFetchingDetails()
    {
        if (currentYipliConfig.playerInfo == null)
        {
            if (IsInternetAvailable())
            {
                // TODO : check for backend curruption;
            }
            else
            {
                SetGameSolutionText(5, 0); // ask user to provide internet
            }
        }

        troubleshootManager.PlayerFetchingCheckDone = true;
    }

    // Game question 4
    public void IsStuckOnNoMatPanel()
    {
        if (YipliHelper.GetMatConnectionStatus() != "Connected")
        {
#if UNITY_ANDROID
            // check phone ble
#elif UNITY_STANDALONE_WIN
            if (!IsMatConnectedToUSB())
            {
                SetGameSolutionText(6, 0); // ask user to connect via usb
            }

            IsBackgroundAppsRunning();
#endif
        }

        troubleshootManager.NoMatPanelCheckDone = true;
    }

    // Game question 5
    public bool IsInternetAvailable()
    {
        troubleshootManager.InternetConnectionTest = true;
        return YipliHelper.checkInternetConnection();
    }

    // Game question 6
    public bool IsMatConnectedToUSB()
    {
        troubleshootManager.MatUsbConnectionTest = true;
        // check if driver can check it
        return true;
    }

    // Game question 7
    public bool IsPhoneBleOn()
    {
        troubleshootManager.PhoneBleTest = true;
        // check if driver can check it
        return true;
    }

    // Game question 8
    public void IsMatAddedInYipliAccount()
    {
        if (YipliHelper.GetMatConnectionStatus() != "Connected")
        {
            // ask driver if the mac address is same that is being trying to connect
            // if not same aske user to add it
            SetGameQuestionText(8); // provide question id from the flowchart
            // else provide solution to make it active and reconnect
            SetGameSolutionText(8, 2);  
        }

        troubleshootManager.MatInYipliAccountCheckDone = true;
    }

    // Game question 9
    public void IsBackgroundAppsRunning()
    {
        // check if driver can check it
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (FileReadWrite.CheckIfOtherProcessesAreRunning())
            {
                SetGameSolutionText(9, 0);
            }
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            // try package manager for android
        }

        troubleshootManager.BackgroundAppsRunningCheckDone = true;
    }

    // Game question 10
    public void AreGamesAndAppUpdated()
    {
        // compare version here
        int gameVersionCode = YipliHelper.convertGameVersionToBundleVersionCode(Application.version);
        int inventoryVersionCode = YipliHelper.convertGameVersionToBundleVersionCode(currentYipliConfig.gameInventoryInfo.gameVersion);


        if (inventoryVersionCode > gameVersionCode)
        {
            //Ask user to Update Game version option
            SetGameSolutionText(10, 0);
        }

        troubleshootManager.GamesAndAppUpdateCheckDone = true;
    }

    #endregion

    #region behaviour questions
    // Game question 11
    public void IsBehaviourSameGames()
    {
        if (!troubleshootManager.SameBehaviourGamesAsked)
        {
            troubleshootManager.SameBehaviourGamesAsked = true;
            SetGameQuestionText(11); // provide question id from the flowchart
        }
        else
        {
            // else provide solution to make it active and reconnect
            SetGameSolutionText(11, 2);
        }
    }

    // Game question 12
    public void IsBehaviourSamePlatform()
    {
        if (!troubleshootManager.SameBehaviourPlatformAsked)
        {
            troubleshootManager.SameBehaviourPlatformAsked = true;
            SetGameQuestionText(12); // provide question id from the flowchart
        }
        else
        {
            // else provide solution to make it active and reconnect
            SetGameSolutionText(13, 2);

            // start mat troubleshoot flow
        }
    }

    // Game question 13
    public void IsBehaviourRandomOrPersistent()
    {
        if (!troubleshootManager.BehaviourRondomOrPersistentAsked)
        {
            troubleshootManager.BehaviourRondomOrPersistentAsked = true;
            SetGameQuestionText(13); // provide question id from the flowchart
        }
        else
        {
            // else provide solution to make it active and reconnect
            SetGameSolutionText(13, 2);
        }
    }

    #endregion

    #region Mat questions
    // Mat flow questions
    // Mat question 1
    public void IsMatOn()
    {
        if (YipliHelper.GetMatConnectionStatus() != "Connected")
        {
            SetMatQuestionText(1);
        }

        troubleshootManager.MatOnCheck = true;
    }

    // Mat question 2
    public void WhatIsTheColorOfLED()
    {
        // if (battery level < 15)
        SetMatSolutionText(2, 0);

        troubleshootManager.ColorOfLED = true;
    }

    // Mat question 3
    public void IsChargingLightVisible()
    {
        SetMatQuestionText(3);
        troubleshootManager.CharginglightVisibility = true;
    }

    // Mat question 4
    public void IsBLEListHasYipli()
    {
        // ask driver to confirm
        troubleshootManager.BleListHasYipliCheckDone = true;
    }

    // Mat question 5
    public bool IsSiliconDrivreInstalled()
    {
        troubleshootManager.SiliconDriverInstallCheck = true;
        return FileReadWrite.DriverInstalledFinished;
    }

    // Mat question 6
    public bool IsSiliconPortAvailableInDeviceManager()
    {
        troubleshootManager.SiliconPortAvailability = true;
        return FileReadWrite.DriverInstalledFinished;
    }

    // Mat question 7
    public void AreThereAnyOtherDeviceConectedToMat()
    {
        // ask driver to confirm
        SetMatQuestionText(7);

        // on Yes click
        SetMatSolutionText(7, 0);
        //return false;

        // on No click
        // continue flow

        troubleshootManager.MatConnectionToOtherDeviceCheckDone = true;
    }

    // Mat question 8
    public void IsThisSameMatAddedInYipliAccount()
    {
        if (YipliHelper.GetMatConnectionStatus() != "Connected")
        {
            // ask driver if the mac address is same that is being trying to connect
            // if not same aske user to add it
            SetMatQuestionText(8); // provide question id from the flowchart
            // else provide solution to make it active and reconnect
            SetMatSolutionText(8, 0);
        }

        troubleshootManager.SameMatFromYipliCheckDone = true;
    }

    #endregion

    #region UI Management
    // UI management
    private void SetGameQuestionText(int qid)
    {
        EnableChoices();

        for(int i = 0; i < gameQuestions.GetQuestions().Count; i++)
        {
            if (gameQuestions.GetQuestions()[i].id == qid)
            {
                questionText.text = gameQuestions.GetQuestions()[i].question;
            }
        }

        QuestionAsked = true;
    }

    private void SetGameSolutionText(int qid, int solutionID)
    {
        DisableChoices();

        for (int i = 0; i < gameQuestions.GetQuestions().Count; i++)
        {
            if (gameQuestions.GetQuestions()[i].id == qid)
            {
                solutionText.text = gameQuestions.GetQuestions()[i].solutions[solutionID];
            }
        }

        solutionProvided = true;
    }

    private void SetMatQuestionText(int qid)
    {
        EnableChoices();

        for (int i = 0; i < matQuestions.GetQuestions().Count; i++)
        {
            if (matQuestions.GetQuestions()[i].id == qid)
            {
                questionText.text = matQuestions.GetQuestions()[i].question;
            }
        }

        QuestionAsked = true;
    }

    private void SetMatSolutionText(int qid, int solutionID)
    {
        DisableChoices();

        for (int i = 0; i < matQuestions.GetQuestions().Count; i++)
        {
            if (matQuestions.GetQuestions()[i].id == qid)
            {
                solutionText.text = matQuestions.GetQuestions()[i].solutions[solutionID];
            }
        }

        solutionProvided = true;
    }

    private void EnableChoices(string yes = "YES", string no = "NO", string notSure = "NOT SURE")
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        notSureButton.gameObject.SetActive(true);

        yesButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = yes;
        noButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = no;
        notSureButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = notSure;
    }

    private void DisableChoices()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        notSureButton.gameObject.SetActive(false);
    }

    private void StopFurtherProcesses()
    {
        TurnOffLoadingPanel();

        if (questionAsked) return;
        if (solutionProvided) return;
    }

    private void TurnOffAllPanels()
    {
        entryPanel.SetActive(false);
        loadingPanel.SetActive(false);
        questionAnswerPage.SetActive(false);
        practicalTaskPanel.SetActive(false);
    }

    private void TurnOnLoadingPanel()
    {
        loadingPanel.SetActive(true);
    }

    private void TurnOffLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    private void TurnOnEntryPanel()
    {
        entryPanel.SetActive(true);
    }

    private void TurnOffEntryPanel()
    {
        entryPanel.SetActive(false);
    }

    private void StartUserInteraction()
    {
        TurnOffEntryPanel();

        loadingPanel.SetActive(true);
        questionAnswerPage.SetActive(true);
    }

    #endregion

    #region Flow Algorithms
    // flow algorithms
    // Set behaviour questions
    public void BehaviourQuestionsFlow()
    {
        
    }

    // Gameplay actions are not working
    public void GameplayActionsAreNotWorking()
    {
        StartUserInteraction();

        // assuming game is not crashing as applications is working
        troubleshootManager.CurrentAlgorithmID = 1;

        // Game questions flow
        if (!troubleshootManager.PlayerFetchingCheckDone)
        {
            IsStuckOnPlayerFetchingDetails();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!troubleshootManager.NoMatPanelCheckDone)
        {
            IsStuckOnNoMatPanel();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!troubleshootManager.GamesAndAppUpdateCheckDone)
        {
            AreGamesAndAppUpdated();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        // behaviout questions
        BehaviourQuestionsFlow();

        // mat questions flow
        if (!troubleshootManager.ColorOfLED)
        {
            WhatIsTheColorOfLED();
        }

        TurnOffLoadingPanel();
    }

    // game crashing
    public void GameCrashingIssue()
    {
        StartUserInteraction();

        // games are already crashing
        troubleshootManager.CurrentAlgorithmID = 2;

        SetGameSolutionText(10, 0); // ask user to update app and games

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        TurnOffLoadingPanel();
    }

    // actions are not getting detected
    public void MatActionsAreNotGettingDetected()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 3;

        // mat questions flow
        if (!troubleshootManager.MatOnCheck)
        {
            IsMatOn();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!troubleshootManager.ColorOfLED)
        {
            WhatIsTheColorOfLED();
        }

        // start suraj driver test
        // on requirement start practical test

        TurnOffLoadingPanel();
    }

    // USB cable  is not working
    public void USBCableNotWorking()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 4;

        // mat questions flow
        if (!troubleshootManager.MatOnCheck)
        {
            IsMatOn();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!troubleshootManager.ColorOfLED)
        {
            WhatIsTheColorOfLED();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        // ask for charging light
        IsChargingLightVisible();

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!IsSiliconDrivreInstalled())
        {
            // start silicon driver installation process
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (IsSiliconPortAvailableInDeviceManager())
        {
            // start silicon driver installation process
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        // start suraj driver test

        TurnOffLoadingPanel();
    }

    // Mat is not getting connected
    public void MatIsNotGettingConnected()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 5;

        // mat questions flow
        if (!troubleshootManager.MatOnCheck)
        {
            IsMatOn();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!troubleshootManager.ColorOfLED)
        {
            WhatIsTheColorOfLED();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

#if UNITY_STANDALONE_WIN
        if (!troubleshootManager.CharginglightVisibility)
        {
            IsChargingLightVisible();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!IsSiliconDrivreInstalled())
        {
            // start silicon driver installation process
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (IsSiliconPortAvailableInDeviceManager())
        {
            // start silicon driver installation process
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

#elif UNITY_ANDROID
        if (!troubleshootManager.BleListHasYipliCheckDone)
        {
            IsBLEListHasYipli();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();

        if (!troubleshootManager.SameMatFromYipliCheckDone)
        {
            IsThisSameMatAddedInYipliAccount();
        }

        // if question is asked or solution is provided that return;
        StopFurtherProcesses();
#endif
        if (!troubleshootManager.MatConnectionToOtherDeviceCheckDone)
        {
            AreThereAnyOtherDeviceConectedToMat();
        }

        // start suraj driver test

        TurnOffLoadingPanel();
    }

    // Game is not getting launched
    public void GameisNotGetingLaunched()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 6;
        // start yipli app trouble shooting flow

        TurnOffLoadingPanel();
    }

    // Mat is not starting
    public void MatIsNotStarting()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 7;
        // start ticket system flow

        TurnOffLoadingPanel();
    }

    // full troubleshoot
    public void FullTroubleShoot()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 8;
        // start ticket system flow

        TurnOffLoadingPanel();
    }

    #endregion

    #region Button Functions
    // button functions
    public void YesButtonFunction()
    {
        TurnOnLoadingPanel();

        yesButtonClicked = true;
        ManageCurrentAlgorithm();
    }

    public void NoButtonFunction()
    {
        TurnOnLoadingPanel();

        noButtonClicked = true;
        ManageCurrentAlgorithm();
    }

    public void NotSureButtonFunction()
    {
        TurnOnLoadingPanel();

        notSureButtonClicked = true;
        ManageCurrentAlgorithm();
    }

    // current algorithm manager
    public void ManageCurrentAlgorithm()
    {
        switch (troubleshootManager.CurrentAlgorithmID)
        {
            case 1:
                GameplayActionsAreNotWorking();
                break;

            case 2:
                GameCrashingIssue();
                break;

            case 3:
                MatActionsAreNotGettingDetected();
                break;

            case 4:
                USBCableNotWorking();
                break;

            case 5:
                MatIsNotGettingConnected();
                break;

            case 6:
                GameisNotGetingLaunched();
                break;

            case 7:
                MatIsNotStarting();
                break;

            case 8:
                FullTroubleShoot();
                break;

            default:
                FullTroubleShoot();
                break;
        }
    }

    #endregion
}