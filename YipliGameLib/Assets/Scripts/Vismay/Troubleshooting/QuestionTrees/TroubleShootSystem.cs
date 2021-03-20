using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN
using yipli.Windows;
#endif
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using YipliFMDriverCommunication;
using BLEFramework.MiniJSON;

public class TroubleShootSystem : MonoBehaviour
{
    // required objects
    // Questions
    [Header("Question Lists")]
    //[SerializeField] Questions gameQuestions;
    //[SerializeField] Questions matQuestions;
    [SerializeField] TroubleShootManagerS troubleshootManager;

    private AllQuestions aq;

    // yipli config
    [Header("Yipli config")]
    [SerializeField] YipliConfig currentYipliConfig;

    // Display Objects
    [Header("UI Elements")]
    // text elements
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI solutionText;
    [SerializeField] TextMeshProUGUI sampleText;
    [SerializeField] TextMeshProUGUI messageBoxText;
    // buttons
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button notSureButton;
    [SerializeField] Button continueButton;

    [Header("UI Panels")]
    // gameobjects
    [SerializeField] GameObject animationParent;
    [SerializeField] GameObject entryPanel;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject questionAnswerPage;
    [SerializeField] GameObject practicalTaskPanel;
    [SerializeField] GameObject messageBoxPanel;

    private PracticalTask practicalTaskManager;

    // blob data objects
    private List<BlobData> newBlobData = null;
    private BlobData blobData;

    //const string siliconLabsDesc = "Silicon Labs CP210x USB to UART Bridge";
    //const string siliconLabsManufacturer = "Silicon Labs";
    private bool checkMatActions = false;

    public bool yesClicked = false;
    public bool notSureClicked = false;
    public bool noClicked = false;

    // questions flags
    private bool questionAsked = false;
    private bool solutionProvided = false;

    // fm response variables
    private string lastPixelSituations = null;

    public bool QuestionAsked { get => questionAsked; set => questionAsked = value; }
    public bool SolutionProvided { get => solutionProvided; set => solutionProvided = value; }
    public string LastPixelSituations { get => lastPixelSituations; set => lastPixelSituations = value; }
    public bool YesClicked { get => yesClicked; set => yesClicked = value; }
    public bool NotSureClicked { get => notSureClicked; set => notSureClicked = value; }
    public bool NoClicked { get => noClicked; set => noClicked = value; }
    public bool CheckMatActions { get => checkMatActions; set => checkMatActions = value; }

    private void Awake()
    {
        aq = FindObjectOfType<AllQuestions>();
        practicalTaskManager = FindObjectOfType<PracticalTask>();
    }

    private void Start()
    {
        troubleshootManager.ResetTroubleShootChecks();
        ResetTroubleShooter();
    }

    private void ResetTroubleShooter()
    {
        TurnOffAllPanels();
        TurnOnEntryPanel();
    }

    private void Update()
    {
        if (CheckMatActions)
        {
            TroubleShootFmResponseManager();
        }
    }

    #region gameQuestions
    // game specific questions
    // Game question 2
    public void IsOsUpdatedToLates()
    {
        SetGameQuestionText(2);

        //troubleshootManager.OsUpdateCheck = true;
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

        //troubleshootManager.PlayerFetchingCheckDone = true;
    }

    // Game question 4
    public void IsStuckOnNoMatPanel()
    {
        if (YipliHelper.GetMatConnectionStatus() != "Connected")
        {
#if UNITY_ANDROID
            // check phone ble
            /*
            if (!IsBLEListHasYipli())
            {
                SetGameQuestionText(4);
            }*/
#elif UNITY_STANDALONE_WIN
            if (!IsMatConnectedToUSB())
            {
                SetGameSolutionText(6, 0); // ask user to connect via usb
            }

            IsBackgroundAppsRunning();
#endif
        }

        //troubleshootManager.NoMatPanelCheckDone = true;
    }

    // Game question 5
    public bool IsInternetAvailable()
    {
        //troubleshootManager.InternetConnectionTest = true;
        return YipliHelper.checkInternetConnection();
    }

    // Game question 6
    public bool IsMatConnectedToUSB()
    {
        //troubleshootManager.MatUsbConnectionTest = true;
        // check if driver can check it
        return true;
    }

    // Game question 7
    public bool IsPhoneBleOn()
    {
        //troubleshootManager.PhoneBleTest = true;
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

        //troubleshootManager.MatInYipliAccountCheckDone = true;
    }

    // Game question 9
    public void IsBackgroundAppsRunning()
    {
#if UNITY_STANDALONE_WIN
        // check if driver can check it
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (FileReadWrite.CheckIfOtherProcessesAreRunning())
            {
                SetGameSolutionText(9, 0);
            }
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            // try package manager for android
        }

        troubleshootManager.BackgroundAppsRunningCheckDone = true;
#endif
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

        //troubleshootManager.GamesAndAppUpdateCheckDone = true;
    }

    #endregion

    #region behaviour questions
    // Game question 11
    public void IsBehaviourSameGames()
    {
        if (troubleshootManager.SameBehaviourGamesAsked && troubleshootManager.SameBehaviourGamessolutionProvided) return;

        if (!troubleshootManager.SameBehaviourGamesAsked)
        {
            troubleshootManager.SameBehaviourGamesAsked = true;
            SetGameQuestionText(11); // provide question id from the flowchart
        }
        else if (NotSureClicked && !troubleshootManager.SameBehaviourGamessolutionProvided)
        {
            SetGameSolutionText(11, 0);
            troubleshootManager.SameBehaviourGamessolutionProvided = true;
        }
    }

    // Game question 12
    public void IsBehaviourSamePlatform()
    {
        if (troubleshootManager.SameBehaviourPlatformAsked && troubleshootManager.SameBehaviourPlatformsolutionProvided) return;

        if (!troubleshootManager.SameBehaviourPlatformAsked)
        {
            troubleshootManager.SameBehaviourPlatformAsked = true;
            SetGameQuestionText(12); // provide question id from the flowchart
        }
        else if (NotSureClicked && !troubleshootManager.SameBehaviourPlatformsolutionProvided)
        {
            SetGameSolutionText(12, 0);
            troubleshootManager.SameBehaviourPlatformsolutionProvided = true;
        }
    }

    // Game question 13
    public void IsBehaviourRandomOrPersistent()
    {
        if (troubleshootManager.BehaviourRondomOrPersistentAsked && troubleshootManager.BehaviourRondomOrPersistentProvided) return;

        if (!troubleshootManager.BehaviourRondomOrPersistentAsked)
        {
            troubleshootManager.BehaviourRondomOrPersistentAsked = true;
            SetGameQuestionText(13); // provide question id from the flowchart
        }
        else if (NotSureClicked && !troubleshootManager.BehaviourRondomOrPersistentProvided)
        {
            // set something for persistent or random behaviour
            troubleshootManager.BehaviourRondomOrPersistentProvided = true;
        }
    }

    #endregion

    #region Mat questions
    // Mat flow questions
    // Mat question 1
    public void IsMatOn()
    {
        if (troubleshootManager.MatOnCheck && troubleshootManager.IsMatOnSolutionProvided) return;

        if (!troubleshootManager.MatOnCheck)
        {
            SetMatQuestionText(1);
            troubleshootManager.MatOnCheck = true;
        }

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        if (NoClicked && !troubleshootManager.IsMatOnSolutionProvided)
        {
            SetMatSolutionText(1, 0);
            troubleshootManager.IsMatOnSolutionProvided = true;
        }
        else
        {
            troubleshootManager.IsMatOnSolutionProvided = true;
        }

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;
    }

    // Mat question 2
    public void WhatIsTheColorOfLED()
    {
        if (troubleshootManager.ColorOfLED && troubleshootManager.RedLedSolutionProvided) return;

        if (!troubleshootManager.ColorOfLED)
        {
            // if (battery level < 15)
            SetMatQuestionText(2);

            troubleshootManager.ColorOfLED = true;
        }

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        if (NoClicked && !troubleshootManager.RedLedSolutionProvided)
        {
            SetMatSolutionText(2, 0);
            troubleshootManager.RedLedSolutionProvided = true;
        }
        else
        {
            troubleshootManager.RedLedSolutionProvided = true;
        }

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;
    }

    // Mat question 3
    public void IsChargingLightVisible()
    {
        if (troubleshootManager.CharginglightVisibility && troubleshootManager.ChargingLightVisibilitySolutionProvided) return;

        if (!troubleshootManager.CharginglightVisibility)
        {
            SetMatQuestionText(3);
            troubleshootManager.CharginglightVisibility = true;

        }

        if ((NoClicked || NotSureClicked) && !troubleshootManager.ChargingLightVisibilitySolutionProvided)
        {
            SetMatSolutionText(3, 0);
            troubleshootManager.ChargingLightVisibilitySolutionProvided = true;
        }
        else
        {
            troubleshootManager.ChargingLightVisibilitySolutionProvided = true;
        }
    }

    // Mat question 4
    public IEnumerator IsBLEListHasYipliAndroid()
    {
        // ask driver to confirm

        Debug.LogError("MatIsNotGettingConnected : from ble list couroutine.");

        TurnOnLoadingPanel();
        InitBLE.ScanForPeripherals();

        yield return new WaitForSecondsRealtime(12f);

        // string peripheralJsonList = "F4:BF:80:63:E3:7A|honor Band 4-37A,F9:4B:4A:BF:66:C1|Amazfit Bip U,F5:FB:4A:55:76:22|Mi Smart Band 4,F5:FV:4A:55:80:22|Yipli";

        string peripheralJsonList = InitBLE.PeripheralJsonList;

        string[] allBleDevices = peripheralJsonList.Split(',');

        for (int i = 0; i < allBleDevices.Length; i++)
        {
            string[] tempSplits = allBleDevices[i].Split('|');

            if (tempSplits[1].Equals("YIPLI", StringComparison.OrdinalIgnoreCase))
            {
                troubleshootManager.BleScannedMacAddress = tempSplits[0];
                break;
            }
        }
        
        TurnOffLoadingPanel();

        if (troubleshootManager.BleScannedMacAddress == null || troubleshootManager.BleScannedMacAddress == "" || troubleshootManager.BleScannedMacAddress == string.Empty)
        {
            AreThereAnyOtherDeviceConectedToMat();

            // if question is asked or solution is provided that return;
            if (StopFurtherProcesses()) yield break;

            // ble is not shown
            SetMatSolutionText(4, 0);

            // if question is asked or solution is provided that return;
            if (StopFurtherProcesses()) yield break;
        }
        else
        {
            Debug.LogError("scan has ble. mac address is : " + troubleshootManager.BleScannedMacAddress);
            sampleText.text = "all mac addresses : " + InitBLE.PeripheralJsonList + "\nscan has ble. mac address is : " + troubleshootManager.BleScannedMacAddress;

            IsThisSameMatAddedInYipliAccountAndroid(troubleshootManager.BleScannedMacAddress);

            // if question is asked or solution is provided that return;
            if (StopFurtherProcesses()) yield break;

            // start suraj driver test
            StartPracticalTask();

            TurnOffLoadingPanel();
        }
    }

#if UNITY_STANDALONE_WIN
    // Mat question 5
    public void IsSiliconDrivreInstalled()
    {
        troubleshootManager.SiliconDriverInstallCheck = true;

        if (!troubleshootManager.SiliconDriverInstallCheck)
        {
            TroubleshootSystem();
        }
    }

    public void TroubleshootSystem()
    {
        FileReadWrite.WriteToFileForDriverSetup(currentYipliConfig.gameId);
        FileReadWrite.ReadFromFile();

        StartCoroutine(WindowsCMDCheck());

        FileReadWrite.CheckIfMatDriverIsInstalled(currentYipliConfig.gameId);
    }

    private IEnumerator WindowsCMDCheck()
    {
        TurnOnLoadingPanel();

        while (!FileReadWrite.DriverInstalledFinished)
        {
            yield return new WaitForSecondsRealtime(1f);

            FileReadWrite.ReadFromFile();
        }

        // after validation is done
        troubleshootManager.SiliconDriverInstallCheck = true;

        AreThereAnyOtherDeviceConectedToMat();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) yield break;

        // start suraj driver test
        StartPracticalTask();

        TurnOffLoadingPanel();
    }
#endif

    // Mat question 6
    public bool IsSiliconPortAvailableInDeviceManager()
    {
#if UNITY_STANDALONE_WIN
        troubleshootManager.SiliconPortAvailability = true;
        return FileReadWrite.DriverInstalledFinished;
#else
        return true;
#endif
    }

    // Mat question 7
    public void AreThereAnyOtherDeviceConectedToMat()
    {
        if (troubleshootManager.IsMatConnectedToOtherDeviceCheckDone && troubleshootManager.IsMatConnectedToOtherDeviceSolutionProvided) return;

        if (!troubleshootManager.IsMatConnectedToOtherDeviceCheckDone)
        {
            // ask driver to confirm
            SetMatQuestionText(7);
            troubleshootManager.IsMatConnectedToOtherDeviceCheckDone = true;
        }

        if (YesClicked && !troubleshootManager.IsMatConnectedToOtherDeviceSolutionProvided)
        {
            // on Yes click
            SetMatSolutionText(7, 0);
        }
        else
        {
            troubleshootManager.IsMatConnectedToOtherDeviceSolutionProvided = true;
        }
    }

    // Mat question 8
    public void IsThisSameMatAddedInYipliAccountAndroid(string scannedMacAddress)
    {
        if (!troubleshootManager.SameMatFromYipliCheckDone && !currentYipliConfig.matInfo.macAddress.Equals(scannedMacAddress, StringComparison.OrdinalIgnoreCase))
        {
            SetMatQuestionText(8); // provide question id from the flowchart
            troubleshootManager.SameMatFromYipliCheckDone = true;
        }

        if ((NoClicked || NotSureClicked) && !troubleshootManager.SameMatFromYipliSolutionProvided)
        {
            // else provide solution to make it active and reconnect
            SetMatSolutionText(8, 0);
            troubleshootManager.SameMatFromYipliSolutionProvided = true;
        }
    }

    #endregion

    #region UI Management
    // UI management
    private void SetGameQuestionText(int qid)
    {
        solutionText.text = "";
        questionText.text = "";

        for (int i = 0; i < aq.GameQuestions.Count; i++)
        {
            if (aq.GameQuestions[i].id == qid)
            {
                questionText.text = aq.GameQuestions[i].question;

                EnableChoices(i);
                break;
            }
        }

        QuestionAsked = true;
    }

    private void SetGameSolutionText(int qid, int solutionID)
    {
        DisableChoices();
        continueButton.gameObject.SetActive(true);

        solutionText.text = "";
        questionText.text = "";

        for (int i = 0; i < aq.GameQuestions.Count; i++)
        {
            if (aq.GameQuestions[i].id == qid)
            {
                solutionText.text = aq.GameQuestions[i].solutions[solutionID];
            }
        }

        solutionProvided = true;
    }

    private void SetMatQuestionText(int qid)
    {
        solutionText.text = "";
        questionText.text = "";

        for (int i = 0; i < aq.MatQuestions.Count; i++)
        {
            if (aq.MatQuestions[i].id == qid)
            {
                questionText.text = aq.MatQuestions[i].question;

                EnableChoices(i, "m");
                break;
            }
        }

        QuestionAsked = true;
    }

    private void SetMatSolutionText(int qid, int solutionID)
    {
        DisableChoices();
        continueButton.gameObject.SetActive(true);

        solutionText.text = "";
        questionText.text = "";

        for (int i = 0; i < aq.MatQuestions.Count; i++)
        {
            if (aq.MatQuestions[i].id == qid)
            {
                solutionText.text = aq.MatQuestions[i].solutions[solutionID];
            }
        }

        solutionProvided = true;
    }

    private void EnableChoices(int qid, string matOrGame = "g")
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        notSureButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);

        if (matOrGame == "m")
        {
            yesButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = aq.MatQuestions[qid].choices[0];
            noButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = aq.MatQuestions[qid].choices[1];
            notSureButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = aq.MatQuestions[qid].choices[2];
        }
        else
        {
            yesButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = aq.GameQuestions[qid].choices[0];
            noButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = aq.GameQuestions[qid].choices[1];
            notSureButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = aq.GameQuestions[qid].choices[2];
        }
    }

    private void DisableChoices()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        notSureButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    private bool StopFurtherProcesses()
    {
        TurnOffLoadingPanel();

        if (questionAsked) return true;
        if (solutionProvided) return true;

        return false;
    }

    private void TurnOffAllPanels()
    {
        entryPanel.SetActive(false);
        loadingPanel.SetActive(false);
        questionAnswerPage.SetActive(false);
        practicalTaskPanel.SetActive(false);
        messageBoxPanel.SetActive(false);
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

    private void TurnOnPracticalPanel()
    {
        practicalTaskPanel.SetActive(true);
    }

    private void TurnOffPracticalPanel()
    {
        practicalTaskPanel.SetActive(false);
    }

    private void TurnOnMessageBoxPanel(string message)
    {
        messageBoxText.text = message;
        messageBoxPanel.SetActive(true);
    }

    private void TurnOffMessageBoxPanel()
    {
        messageBoxText.text = "";
        messageBoxPanel.SetActive(false);
    }

    #endregion

    #region Flow Algorithms
    // flow algorithms
    // Set behaviour questions
    public void BehaviourQuestionsFlow()
    {
        // ask behavioural questions
        //troubleshootManager.CurrentAlgorithmID = 9;

        IsBehaviourSameGames();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        IsBehaviourSamePlatform();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        IsBehaviourRandomOrPersistent();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;
    }

    // Gameplay actions are not working
    public void GameplayActionsAreNotWorking()
    {
        StartUserInteraction();

        // assuming game is not crashing as applications is working
        troubleshootManager.CurrentAlgorithmID = 1;

        // Game questions flow
        IsStuckOnPlayerFetchingDetails();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        IsStuckOnNoMatPanel();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        AreGamesAndAppUpdated();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        // behaviout questions
        BehaviourQuestionsFlow();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        // mat questions flow
        IsMatOn();

        WhatIsTheColorOfLED();

        StartPracticalTask();

        TurnOffLoadingPanel();

        //Debug.LogError("No Problem is found");
       //// ResetTroubleShooter();
      ///  TurnOnMessageBoxPanel("No Problem is found");
    }

    // game crashing
    public void GameCrashingIssue()
    {
        StartUserInteraction();

        // games are already crashing
        troubleshootManager.CurrentAlgorithmID = 2;

        if (!troubleshootManager.OsUpdateCheck)
        {
            SetGameQuestionText(2);
            troubleshootManager.OsUpdateCheck = true;
        }

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        if ((NoClicked || NotSureClicked) && !troubleshootManager.OsUpdateSolutionProvided)
        {
            SetGameSolutionText(2, 1);
            troubleshootManager.OsUpdateSolutionProvided = true;
        }
        else if (YesClicked)
        {
            SetGameSolutionText(2, 0);
            troubleshootManager.OsUpdateSolutionProvided = true;
        }
        else
        {
            troubleshootManager.OsUpdateSolutionProvided = true;
        }

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        // ask user for the behaviour
        BehaviourQuestionsFlow();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        SetGameSolutionText(10, 0); // ask user to update app and games

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        TurnOffLoadingPanel();
    }

    // actions are not getting detected
    public void MatActionsAreNotGettingDetected()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 3;

        // mat questions flow
        IsMatOn();

        WhatIsTheColorOfLED();

        StartPracticalTask();

        TurnOffLoadingPanel();
    }

    // USB cable  is not working
    public void USBCableNotWorking()
    {
        if ((Application.platform != RuntimePlatform.WindowsEditor) || (Application.platform != RuntimePlatform.WindowsPlayer))
        {
            Debug.LogError("Non USB Platform is detected");
            ResetTroubleShooter();
            TurnOnMessageBoxPanel("This platform is not working with USB cable. Try on Windows machine");
            return;
        }

        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 4;

        // mat questions flow
        IsMatOn();

        WhatIsTheColorOfLED();

        // ask for charging light
        IsChargingLightVisible();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

#if UNITY_STANDALONE_WIN
        IsSiliconDrivreInstalled();
#endif
    }

    // Mat is not getting connected
    public void MatIsNotGettingConnected()
    {
        StartUserInteraction();

        troubleshootManager.CurrentAlgorithmID = 5;

        // mat questions flow
        IsMatOn();

        WhatIsTheColorOfLED();

        Debug.LogError("MatIsNotGettingConnected : color of led is done.");

#if UNITY_STANDALONE_WIN
        IsChargingLightVisible();

        // if question is asked or solution is provided that return;
        if (StopFurtherProcesses()) return;

        IsSiliconDrivreInstalled();
#elif UNITY_ANDROID
        Debug.LogError("MatIsNotGettingConnected : next line is ble list couroutine.");

        StartCoroutine(IsBLEListHasYipliAndroid());

        // if question is asked or solution is provided that return;
        //if (StopFurtherProcesses()) return;
#endif
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
        YesClicked = true;

        TurnOnLoadingPanel();

        questionAsked = false;
        solutionProvided = false;

        ManageCurrentAlgorithm();
    }

    public void NoButtonFunction()
    {
        NoClicked = true;

        TurnOnLoadingPanel();

        questionAsked = false;
        solutionProvided = false;

        ManageCurrentAlgorithm();
    }

    public void NotSureButtonFunction()
    {
        NotSureClicked = true;

        TurnOnLoadingPanel();

        questionAsked = false;
        solutionProvided = false;

        ManageCurrentAlgorithm();
    }

    public void OkayButton()
    {
        questionAsked = false;
        solutionProvided = false;

        TurnOffMessageBoxPanel();
    }

    public void ContinueButton()
    {
        questionAsked = false;
        solutionProvided = false;

        ResetButtonFlags();
        ResetTroubleShooter();
    }

    private void ResetButtonFlags()
    {
        YesClicked = false;
        NotSureClicked = false;
        NoClicked = false;
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

#region PracticalTaskManager

    // specific practical task
    public void StartPracticalTask()
    {
        TurnOffAllPanels();
        SetTroubleShootClusterID();

        TurnOffEntryPanel();
        TurnOnPracticalPanel();

        practicalTaskManager.ManagePracticalTaskStepOne();
    }

#endregion

#region DriverResponseMnaagers

    public async Task<object> GetPracticalTaskDriverResponse()
    {
        // update await part with driver response
        await Task.Delay(TimeSpan.FromSeconds(1));
        return newBlobData;
    }

    public void SetTroubleShootClusterID()
    {
        // trouble shooting new cluster id is 999
        YipliHelper.SetGameClusterId(999);
    }

    private void TroubleShootFmResponseManager()
    {
        // test only
        //practicalTaskManager.ManagePracticaltask("");
        //return;

        // actual code
        string fmActionData = InitBLE.GetFMResponse();
        Debug.Log("Json Data from Fmdriver : " + fmActionData);

        FmDriverResponseInfo singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);

        if (singlePlayerResponse == null) return;

        if (PlayerSession.Instance.currentYipliConfig.oldFMResponseCount != singlePlayerResponse.count)
        {
            Debug.LogError("FMResponse " + fmActionData);
            PlayerSession.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

            YipliUtils.PlayerActions providedAction = ActionAndGameInfoManager.GetActionEnumFromActionID(singlePlayerResponse.playerdata[0].fmresponse.action_id);

            switch (providedAction)
            {
                case YipliUtils.PlayerActions.TROUBLESHOOTING:

                    if (singlePlayerResponse.playerdata[0].fmresponse.properties.ToString() != "null")
                    {
                        string[] tokens = singlePlayerResponse.playerdata[0].fmresponse.properties.Split(':');

                        if (tokens.Length > 0)
                        {
                            if (tokens[0].Equals("array"))
                            {
                                if (LastPixelSituations != tokens[1])
                                {
                                    LastPixelSituations = tokens[1];

                                    Debug.LogError("array - response : " + tokens[1]);
                                    practicalTaskManager.ManagePracticaltask(tokens[1]);
                                }
                            }
                        }
                    }

                    //practicalTaskManager.TransformLegs("");
                    break;

                case YipliUtils.PlayerActions.JUMPING_JACK:

                    if (singlePlayerResponse.playerdata[0].fmresponse.properties.ToString() != "null")
                    {
                        string[] tokens = singlePlayerResponse.playerdata[0].fmresponse.properties.Split(':');

                        if (tokens.Length > 0)
                        {
                            if (tokens[0].Equals("array"))
                            {
                                practicalTaskManager.ManagePracticaltask(tokens[1], "JJ");

                                /*
                                if (LastPixelSituations != tokens[1])
                                {
                                    LastPixelSituations = tokens[1];

                                    Debug.LogError("array - response : " + tokens[1]);
                                    practicalTaskManager.ManagePracticaltask(tokens[1], "JJ");
                                }
                                */
                            }
                        }
                    }
                    break;

                default:
                    Debug.LogError("Wrong Actions detected : " + providedAction.ToString());
                    break;
            }
        }
    }

#endregion

#region Test functions

    public void TestYipliList()
    {
        StartCoroutine(IsBLEListHasYipliAndroid());

        //List<object> peripheralsList = new List<object>();

        //string peripheralJsonList = "deviceList" + ": [{" + "address" + ":" + "F4: BF: 80:63:E3: 7A" + "," + "name" + ":" + "honor Band 4 - 37A" + "},{" + "address" + ":" + "F9: 4B: 4A: BF: 66:C1" + "," + "name" + ":" + "Amazfit Bip U" + "},{" + "address" + ":" + "F5: FB: 4A: 55:76:22" + "," + "name" + ":" + "Mi Smart Band 4" + "}]";
    }

#endregion
}