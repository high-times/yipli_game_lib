using UnityEngine;
using UnityEngine.UI;
using YipliFMDriverCommunication;
using DG.Tweening;

public class SecondTutorialManager : MonoBehaviour
{
    //required variables

    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialPanel = null;
    [SerializeField] private GameObject tutMOdelParent = null;
    [SerializeField] private Image runningManMat = null;
    [SerializeField] private GameObject threeBoxesParent = null;
    [SerializeField] private GameObject tryItBoxParent = null;
    [SerializeField] private GameObject finalTutParent = null;

    [Header("Script objects")]
    [SerializeField] private YipliConfig currentYipliConfig = null;
    [SerializeField] private NewMatInputController newMatInputController = null;
    [SerializeField] private MatInputController matInputController = null;
    [SerializeField] private ThreeDModelManager threeDModelManager = null;

    [Header("All Text messages")]
    [SerializeField] private GameObject letsLearnHowToUseMatMSG = null;
    [SerializeField] private GameObject highlightGameMenu = null;
    [SerializeField] private GameObject toSelectJumpOnMat = null;
    [SerializeField] private GameObject navigatingWithYourMatMSG = null;
    [SerializeField] private GameObject letsLearnToUseYourMatMSG = null;
    [SerializeField] private GameObject messageStepOnMatAndCenter = null;
    [SerializeField] private GameObject tapLeft3Times = null;
    [SerializeField] private GameObject tapRight3Times = null;
    [SerializeField] private GameObject jump3Times = null;
    [SerializeField] private GameObject awesomeJob = null;
    [SerializeField] private GameObject youGotThatRight = null;
    [SerializeField] private GameObject pauseTitle = null;
    [SerializeField] private GameObject pauseStatement = null;
    [SerializeField] private GameObject resumeTitle = null;
    [SerializeField] private GameObject resumeStatement = null;
    [SerializeField] private GameObject trainingDoneTitle = null;
    [SerializeField] private GameObject goodHeadStart = null;
    [SerializeField] private GameObject letsStartPlaying = null;

    [Header("All Required Colors")]
    [SerializeField] private Color yipliRed;
    [SerializeField] private Color yipliGreen;
    [SerializeField] private Color originalButtonColor;

    [Header("All Required Music")]
    [SerializeField] private AudioSource tutorialAudioSource;
    [SerializeField] private AudioClip checkMarkSound;
    [SerializeField] private AudioClip errorSound;

    // private variables
    YipliUtils.PlayerActions detectedAction;

    [Header("Test Area")]
    // bool values
    public bool leftTapsDone = false;
    public bool rightTapsDone = false;
    public bool jumpsDone = false;
    public bool runningIntroDone = false;
    public bool startIntroDone = false;
    public bool calculateTime = false;
    public bool tutorialStarted = false;
    public bool userInteractionStarted = false;
    public bool pauseFlowStarted = false;
    public bool resumeFlowStarted = false;
    public bool tapsAndJumpInfoFlowStarted = false;
    public bool simpleTutorialDone = false;
    public bool finalTutStarted = false;
    public bool finalMessageDisplayStarted = false;
    public bool waitForNextPart = false;

    // int values
    public int totalLeftTaps = 0;
    public int totalRightTaps = 0;
    public int totalJumps = 0;
    public int currentCircleChildActive = 0;
    public int requiredCHildElement = -1;

    // float time variables
    public float currentCalculatedTime = 0;

    // getters and setters
    public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }

    void Start() {
        runningManMat.gameObject.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void TurnOffEverything() {
        letsLearnHowToUseMatMSG.gameObject.SetActive(false);
        highlightGameMenu.gameObject.SetActive(false);
        toSelectJumpOnMat.gameObject.SetActive(false);
        navigatingWithYourMatMSG.gameObject.SetActive(false);
        letsLearnToUseYourMatMSG.gameObject.SetActive(false);
        messageStepOnMatAndCenter.gameObject.SetActive(false);
        tapLeft3Times.gameObject.SetActive(false);
        tapRight3Times.gameObject.SetActive(false);
        jump3Times.gameObject.SetActive(false);
        awesomeJob.gameObject.SetActive(false);
        youGotThatRight.gameObject.SetActive(false);
        pauseTitle.gameObject.SetActive(false);
        pauseStatement.gameObject.SetActive(false);
        resumeTitle.gameObject.SetActive(false);
        resumeStatement.gameObject.SetActive(false);
        trainingDoneTitle.gameObject.SetActive(false);
        goodHeadStart.gameObject.SetActive(false);
        letsStartPlaying.gameObject.SetActive(false);

        HideThreeBoxes();
        HideTryItBox();
        HideFinalTutElements();
    }

    void Update() {

        if (!tutorialStarted) return;

        CalculateTime();

        if (matInputController.IsTutorialRunning) {
            GetMatTutorialKeyboardInputs();
            ManageMatActionsForTutorial();
        }

        ManageMatTutorial();
    }

    public void SetTutorialClusterID(int clusterID)
    {
        try
        {
            //Debug.LogError("provided clusterID : " + clusterID);
            YipliHelper.SetGameClusterId(clusterID);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Something went wrong with setting the cluster id : " + e.Message);
        }
    }

    public void StartMatTutorial() {

        if (tutorialStarted) return;

        matInputController.IsTutorialRunning = true;
        tutorialStarted = true;

        SetTutorialClusterID(6);

        TurnOffEverything();

        tutorialPanel.SetActive(true);

        newMatInputController.DisplayMainMat();
        newMatInputController.HideChevrons();
        newMatInputController.UpdateCenterButtonColor();
        newMatInputController.EnableMatParentButtonAnimator();

        letsLearnHowToUseMatMSG.gameObject.SetActive(true);
        messageStepOnMatAndCenter.gameObject.SetActive(true);
        newMatInputController.DisplayTextButtons();
        newMatInputController.KeepLeftNadRightButtonColorToOriginal();
    }

    public void EndMatTutorial() {
        calculateTime = false;
        currentCalculatedTime = 0f;

        threeDModelManager.ApplyMainIdleOverride();
        threeDModelManager.Hide3DModel();

        newMatInputController.HideTextButtons();

        tutorialPanel.gameObject.SetActive(false);

        newMatInputController.UpdateCenterButtonWithOriginalColor();

        SetTutorialClusterID(0);

        matInputController.IsTutorialRunning = false;
        
        //ResetTutorial();

        newMatInputController.DisableMatParentButtonAnimator();

        FindObjectOfType<PlayerSelection>().OnTutorialContinuePress();
    }

    public void ManageMatTutorial() {
        if (!startIntroDone) {
            StartMatTutorial();
            return;
        }

        if (!runningIntroDone) {
            StartRunningIntro();

            if (currentCalculatedTime > 6f) {
                runningIntroDone = true;
                calculateTime = false;
            }
        }

        if (!runningIntroDone) return;

        if (!leftTapsDone) {
            StartLeftTapsPart();
        }

        if (waitForNextPart) {
            if (currentCalculatedTime > 1.5f) {
                waitForNextPart = false;
                calculateTime = false;
                currentCalculatedTime = 0;

                StartNextTapsPart();
            } else {
                return;
            }
        }

        if (leftTapsDone && rightTapsDone && jumpsDone && !simpleTutorialDone) {
            if (currentCalculatedTime > 1f) {
                calculateTime = false;
                simpleTutorialDone = true;
                StartPauseFlow();
            }
        }

        if (pauseFlowStarted) {
            if ((currentCalculatedTime > 2f && currentCalculatedTime < 2.5f) && !tryItBoxParent.activeSelf) {
                calculateTime = false;
                DisplayTryItBox();
                return;
            }

            if (currentCalculatedTime > 2.5f && tryItBoxParent.activeSelf) {
                StartResumeFlow();
            }
        }

        if (resumeFlowStarted) {
            if ((currentCalculatedTime > 2f &&  currentCalculatedTime < 2.5f) && !tryItBoxParent.activeSelf) {
                calculateTime = false;
                DisplayTryItBox();
                return;
            }

            if (currentCalculatedTime > 2.5f && tryItBoxParent.activeSelf) {
                HideTryItBox();

                resumeTitle.gameObject.SetActive(false);
                resumeStatement.gameObject.SetActive(false);

                TurnOnAwesomePart();
            }

            if (currentCalculatedTime > 5f) {
                resumeFlowStarted = false;
                StartTapAndJumpInfoFlow();
            }
        }

        if (tapsAndJumpInfoFlowStarted) {
            if (currentCalculatedTime > 2.5f && currentCalculatedTime < 3f) {
                highlightGameMenu.gameObject.SetActive(true);
            } else if (currentCalculatedTime > 3f && currentCalculatedTime < 7f) {
                toSelectJumpOnMat.gameObject.SetActive(true);
            } else if (currentCalculatedTime > 10f) {
                tapsAndJumpInfoFlowStarted = true;
                calculateTime = false;

                StartFinalTutorial();
            }
        }

        if (finalMessageDisplayStarted) {
            if (currentCalculatedTime > 3f && currentCalculatedTime < 7f) {
                TurnOffAwesomePart();

                trainingDoneTitle.gameObject.SetActive(true);
                goodHeadStart.gameObject.SetActive(true);
                letsStartPlaying.gameObject.SetActive(true);
            } else if (currentCalculatedTime > 10f) {
                EndMatTutorial();
            }
        }
    }

    private void CalculateTime() {
        if (calculateTime) {
            currentCalculatedTime += Time.deltaTime;
        } else {
            currentCalculatedTime = 0;
        }
    }

    // step 2
    private void StartRunningIntro()
    {
        if (calculateTime) return;

        TurnOffEverything();

        calculateTime = true;

        newMatInputController.HideTextButtons();
        newMatInputController.HideMainMat();
        letsLearnToUseYourMatMSG.gameObject.SetActive(true);

        // apply main animation here
        threeDModelManager.Display3DModel();
        threeDModelManager.EnableModelManagerAnimator();
        threeDModelManager.ApplyWalkingOverride();

        runningManMat.gameObject.SetActive(true);
    }

    // step 3
    private void StartLeftTapsPart() {
        if (userInteractionStarted) return;

        userInteractionStarted = true;

        tapLeft3Times.GetComponent<Animator>().enabled = false;
        tapRight3Times.GetComponent<Animator>().enabled = false;
        jump3Times.GetComponent<Animator>().enabled = false;

        tapLeft3Times.transform.GetChild(1).gameObject.SetActive(false);
        tapRight3Times.transform.GetChild(1).gameObject.SetActive(false);
        jump3Times.transform.GetChild(1).gameObject.SetActive(false);

        tapLeft3Times.gameObject.SetActive(true);
        threeDModelManager.ApplyLeftTapOverride();

        DisplayThreeBoxes();
    }

    // step 4
    private void StartRightTapsPart() {
        tapRight3Times.gameObject.SetActive(true);
        threeDModelManager.ApplyRightTapOverride();

        ResetBoxes();
    }

    // step 4
    private void StartJumpsPart() {
        tapRight3Times.transform.GetChild(1).gameObject.SetActive(true);
        tapRight3Times.GetComponent<Animator>().enabled = true;

        jump3Times.gameObject.SetActive(true);
        threeDModelManager.ApplyJumpOverride();

        ResetBoxes();
    }

    // Step 5
    private void EndTapsPart() {
        tapLeft3Times.gameObject.SetActive(false);
        tapRight3Times.gameObject.SetActive(false);
        jump3Times.gameObject.SetActive(false);
        letsLearnToUseYourMatMSG.gameObject.SetActive(false);

        threeDModelManager.ApplyMainIdleOverride();

        userInteractionStarted = false;
        calculateTime = true;

        HideThreeBoxes();

        threeDModelManager.ActivatePausePart();

        TurnOnAwesomePart();
    }

    private void TurnOnAwesomePart() {
        awesomeJob.gameObject.SetActive(true);
        youGotThatRight.gameObject.SetActive(true);
    }

    private void TurnOffAwesomePart() {
        awesomeJob.gameObject.SetActive(false);
        youGotThatRight.gameObject.SetActive(false);
    }

    // step 6 - pause part
    private void StartPauseFlow() {
        pauseFlowStarted = true;

        TurnOffAwesomePart();

        //threeDModelManager.ActivatePausePart();

        pauseStatement.gameObject.SetActive(true);
        pauseTitle.gameObject.SetActive(true);

        calculateTime = true;
    }

    // step 7 - resume part
    private void StartResumeFlow() {
        pauseFlowStarted = false;
        resumeFlowStarted = true;

        calculateTime = false;
        currentCalculatedTime = 0f;

        ResetTryItBox();

        pauseStatement.gameObject.SetActive(false);
        pauseTitle.gameObject.SetActive(false);

        resumeTitle.gameObject.SetActive(true);
        resumeStatement.gameObject.SetActive(true);
    }

    // step 8 - info panel
    private void StartTapAndJumpInfoFlow() {
        calculateTime = false;
        currentCalculatedTime = 0f;

        TurnOffAwesomePart();

        navigatingWithYourMatMSG.gameObject.SetActive(true);

        tapsAndJumpInfoFlowStarted = true;
        calculateTime = true;
    }

    // step 9 - Final Tutorial
    private void StartFinalTutorial() {
        navigatingWithYourMatMSG.gameObject.SetActive(false);
        highlightGameMenu.gameObject.SetActive(false);
        toSelectJumpOnMat.gameObject.SetActive(false);

        finalTutStarted = true;

        DisplayFinalTutElements();
    }

    // step 10 - training complete
    private void ShowFinalMessages() {
        finalTutStarted = false;
        finalMessageDisplayStarted = true;
        tapsAndJumpInfoFlowStarted = false;

        threeDModelManager.ApplyFistPumpOverride();

        HideFinalTutElements();

        awesomeJob.gameObject.SetActive(true);
        youGotThatRight.gameObject.SetActive(true);

        calculateTime = true;
    }

    // next part to invoke right taps and jump part following tutorial forward
    private void ProcessTapsPart() {
        if (!leftTapsDone && totalLeftTaps > 2) {
            leftTapsDone = true;

            tapLeft3Times.transform.GetChild(1).gameObject.SetActive(true);
            tapLeft3Times.GetComponent<Animator>().enabled = true;

            StartWaitPart();
            return;
        }

        if (!rightTapsDone && totalRightTaps > 2) {
            rightTapsDone = true;

            tapRight3Times.transform.GetChild(1).gameObject.SetActive(true);
            tapRight3Times.GetComponent<Animator>().enabled = true;

            StartWaitPart();
            return;
        }

        if (!jumpsDone && totalJumps > 2) {
            jumpsDone = true;

            jump3Times.transform.GetChild(1).gameObject.SetActive(true);
            jump3Times.GetComponent<Animator>().enabled = true;

            StartWaitPart();
            return;
        }
    }

    private void StartWaitPart() {
        waitForNextPart = true;
        calculateTime = true;
    }

    private void StartNextTapsPart() {
        if (leftTapsDone && !rightTapsDone && !jumpsDone) {
            StartRightTapsPart();
            return;
        }

        if (leftTapsDone && rightTapsDone && !jumpsDone) {
            StartJumpsPart();
            return;
        }

        if (leftTapsDone && rightTapsDone && jumpsDone) {
            EndTapsPart();
            return;
        }
    }

    // manage check mark boxes
    private void DisplayThreeBoxes() {
        threeBoxesParent.gameObject.SetActive(true);
    }

    private void HideThreeBoxes() {
        threeBoxesParent.gameObject.SetActive(false);
    }

    private void ResetBoxes() {
        threeBoxesParent.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.white;
        threeBoxesParent.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = Color.white;
        threeBoxesParent.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>().color = Color.white;
    }

    private void MarkFirstBox() {
        PlaySound(checkMarkSound);
        threeBoxesParent.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = yipliGreen;
    }

    private void MarkSecondBox() {
        PlaySound(checkMarkSound);
        threeBoxesParent.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = yipliGreen;
    }

    private void MarkThirdBox() {
        PlaySound(checkMarkSound);
        threeBoxesParent.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>().color = yipliGreen;
    }

    private void ProcessBoxCheckMarks(int taps) {
        switch(taps) {
            case 1:
                MarkFirstBox();
                break;

            case 2:
                MarkSecondBox();
                break;

            case 3:
                MarkThirdBox();
                ProcessTapsPart();
                break;

            default:
                break;
        }
    }

    private void ShakeBoxes() {
        for (int i = 0; i < threeBoxesParent.transform.childCount; i++) {
            if (threeBoxesParent.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color == yipliGreen) continue;

            PlaySound(errorSound);
            threeBoxesParent.transform.GetChild(i).GetComponent<DOTweenAnimation>().DOPlay();
        }
    }

    // try it box
    private void DisplayTryItBox() {
        tryItBoxParent.gameObject.SetActive(true);
    }

    private void HideTryItBox() {
        tryItBoxParent.gameObject.SetActive(false);
    }

    private void MarkTryItBox() {
        PlaySound(checkMarkSound);
        tryItBoxParent.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = yipliGreen;
    }

    private void ResetTryItBox() {
        tryItBoxParent.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.white;
    }

    private void ShakeTryItBox() {
        PlaySound(errorSound);
        tryItBoxParent.transform.GetChild(0).GetComponent<DOTweenAnimation>().DOPlay();
    }

    // final tutorial management
    private void DisplayFinalTutElements() {
        finalTutParent.gameObject.SetActive(true);

        MakeAllCircleChildrenNormal();

        MarkCircleAsRequiredToSelect(finalTutParent.transform.GetChild(2).transform.GetChild(2).gameObject);
        requiredCHildElement = 2;

        currentCircleChildActive = 0;
        TraverseThroughAllCirclesLikeButtons();
    }

    private void MakeAllCircleChildrenNormal() {
        for (int i = 0; i < finalTutParent.transform.GetChild(2).transform.childCount; i++) {
            finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
            //finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    private void HideFinalTutElements() {
        finalTutParent.gameObject.SetActive(false);
    }

    private void MarkCircleAsRequiredToSelect(GameObject childCirle) {
        childCirle.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void TraverseThroughAllCirclesLikeButtons() {
        for (int i = 0; i < finalTutParent.transform.GetChild(2).transform.childCount; i++) {
            if (i == currentCircleChildActive) {
                finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().color = yipliRed;
                finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.GetChild(2).gameObject.SetActive(false);
            } else {
                finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.localScale = new Vector3(1f, 1f, 1f);
                finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().color = Color.white;
                finalTutParent.transform.GetChild(2).transform.GetChild(i).transform.GetChild(2).gameObject.SetActive(true);
            }
        }
    }

    private void SelectCircleChild() {
        if (currentCircleChildActive == requiredCHildElement) {
            finalTutParent.transform.GetChild(2).transform.GetChild(currentCircleChildActive).transform.GetChild(1).GetComponent<Image>().color = Color.white;
            finalTutParent.transform.GetChild(2).transform.GetChild(currentCircleChildActive).transform.GetChild(2).GetComponent<Image>().color = yipliGreen;
            finalTutParent.transform.GetChild(2).transform.GetChild(currentCircleChildActive).transform.GetChild(2).gameObject.SetActive(true);
            
            threeDModelManager.ApplyFistPumpOverride();
            PlaySound(checkMarkSound);

            ProgressFinalTutorial();
        } else {
            threeDModelManager.ApplyHeadNodOverride();
            PlaySound(errorSound);
            ShakeCircles();
        }
    }

    private void ProgressFinalTutorial() {
        switch (requiredCHildElement) {
            case 0:
                MakeAllCircleChildrenNormal();
                MarkCircleAsRequiredToSelect(finalTutParent.transform.GetChild(2).transform.GetChild(1).gameObject);
                requiredCHildElement = 1;
                TraverseThroughAllCirclesLikeButtons();
                break;

            case 1:
                ShowFinalMessages();
                break;

            case 2:
                MakeAllCircleChildrenNormal();
                MarkCircleAsRequiredToSelect(finalTutParent.transform.GetChild(2).transform.GetChild(0).gameObject);
                requiredCHildElement = 0;
                TraverseThroughAllCirclesLikeButtons();
                break;
        }
    }

    private int GetNextCircleChild() {
        switch (currentCircleChildActive) {
            case 0:
                return 1;

            case 1:
                return 2;

            case 2:
                return 0;

            default:
                return 0;
        }
    }

    private int GetPreviousCircleChild() {
        switch (currentCircleChildActive) {
            case 0:
                return 2;

            case 1:
                return 0;

            case 2:
                return 1;

            default:
                return 0;
        }
    }

    private void ShakeCircles() {
        for (int i = 0; i < finalTutParent.transform.GetChild(2).transform.childCount; i++) {
            if (i == requiredCHildElement) continue;

            finalTutParent.transform.GetChild(2).transform.GetChild(i).GetComponent<DOTweenAnimation>().DOPlay();
        }
    }

    // controller part
    private void ManageMatActionsForTutorial()
    {
        //if (!currentYipliConfig.onlyMatPlayMode) return;

        string fmActionData = InitBLE.GetFMResponse();
        Debug.Log("Json Data from Fmdriver in matinput : " + fmActionData);

        FmDriverResponseInfo singlePlayerResponse = null;

        try {
            singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);
        } catch (System.Exception e) {
            Debug.Log("singlePlayerResponse is having problem : " + e.Message);
        }

        if (singlePlayerResponse == null) return;

        if (currentYipliConfig.oldFMResponseCount != singlePlayerResponse.count)
        {
            PlayerSession.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

            DetectedAction = ActionAndGameInfoManager.GetActionEnumFromActionID(singlePlayerResponse.playerdata[0].fmresponse.action_id);

            switch(DetectedAction)
            {
                // UI input executions
                case YipliUtils.PlayerActions.LEFT_TAP:
                    ManagePlayerActions();
                    break;

                case YipliUtils.PlayerActions.RIGHT_TAP:
                    ManagePlayerActions();
                    break;

                case YipliUtils.PlayerActions.JUMP:
                    ManagePlayerActions();
                    break;

                case YipliUtils.PlayerActions.PAUSE:
                    ManagePlayerActions();
                    break;

                default:
                    Debug.LogError("Wrong Action is detected : " + DetectedAction.ToString());
                    break;
            }
        }
    }

    private void GetMatTutorialKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DetectedAction = YipliUtils.PlayerActions.LEFT_TAP;
            ManagePlayerActions();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DetectedAction = YipliUtils.PlayerActions.RIGHT_TAP;
            ManagePlayerActions();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DetectedAction = YipliUtils.PlayerActions.JUMP;
            ManagePlayerActions();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DetectedAction = YipliUtils.PlayerActions.PAUSE;
            ManagePlayerActions();
        }
    }

    private void ManagePlayerActions() {

        if (!startIntroDone) {
            if (DetectedAction == YipliUtils.PlayerActions.LEFT_TAP) {
                EndMatTutorial();
                return;
            } else if (DetectedAction == YipliUtils.PlayerActions.RIGHT_TAP) {
                startIntroDone = true;
                ManagePlayerActions();
            } else {
                return;
            }
        }

        if (!leftTapsDone) {

            if (DetectedAction != YipliUtils.PlayerActions.LEFT_TAP) {
                ShakeBoxes();
                return;
            }

            totalLeftTaps++;
            ProcessBoxCheckMarks(totalLeftTaps);
        }

        if (!rightTapsDone) {
            if (!leftTapsDone) return;

            if (DetectedAction != YipliUtils.PlayerActions.RIGHT_TAP) {
                ShakeBoxes();
                return;
            }
            
            totalRightTaps++;
            ProcessBoxCheckMarks(totalRightTaps);
        }

        if (!jumpsDone) {
            if (!rightTapsDone) return;

            if (DetectedAction != YipliUtils.PlayerActions.JUMP) {
                ShakeBoxes();
                return;
            }

            totalJumps++;
            ProcessBoxCheckMarks(totalJumps);
        }

        if (pauseFlowStarted) {
            if (DetectedAction != YipliUtils.PlayerActions.PAUSE) {
                ShakeTryItBox();
                return;
            }

            calculateTime = true;

            MarkTryItBox();

            threeDModelManager.ResetAllTriggers();
            threeDModelManager.ActivateResumePart();
        }

        if (resumeFlowStarted) {
            if (DetectedAction != YipliUtils.PlayerActions.JUMP) {
                ShakeTryItBox();
                return;
            }

            calculateTime = true;

            MarkTryItBox();

            threeDModelManager.ResetAllTriggers();
            threeDModelManager.ApplyMainIdleOverride();
        }

        if (finalTutStarted) {
            if (DetectedAction == YipliUtils.PlayerActions.LEFT_TAP) {
                currentCircleChildActive = GetPreviousCircleChild();
                TraverseThroughAllCirclesLikeButtons();
            } else if (DetectedAction == YipliUtils.PlayerActions.RIGHT_TAP) {
                currentCircleChildActive = GetNextCircleChild();
                TraverseThroughAllCirclesLikeButtons();
            } else if (DetectedAction == YipliUtils.PlayerActions.JUMP) {
                SelectCircleChild();
            }
        }
    }

    public void ResetTutorial() {
        leftTapsDone = false;
        rightTapsDone = false;
        jumpsDone = false;
        runningIntroDone = false;
        startIntroDone = false;
        calculateTime = false;
        tutorialStarted = false;
        userInteractionStarted = false;
        pauseFlowStarted = false;
        resumeFlowStarted = false;
        tapsAndJumpInfoFlowStarted = false;
        simpleTutorialDone = false;
        finalTutStarted = false;
        finalMessageDisplayStarted = false;
        waitForNextPart = false;

        // int values
        totalLeftTaps = 0;
        totalRightTaps = 0;
        totalJumps = 0;
        currentCircleChildActive = 0;
        requiredCHildElement = -1;

        // float time variables
        currentCalculatedTime = 0;
    }

    // sound management
    private void PlaySound(AudioClip clip) {
        tutorialAudioSource.clip = clip;
        tutorialAudioSource.Play();
    }

    // button functions
    public void SkipTutorialButton() {
        DetectedAction = YipliUtils.PlayerActions.LEFT_TAP;
        ManagePlayerActions();
    }

    public void ContinueToTutorialButton() {
        DetectedAction = YipliUtils.PlayerActions.RIGHT_TAP;
        ManagePlayerActions();
    }
}