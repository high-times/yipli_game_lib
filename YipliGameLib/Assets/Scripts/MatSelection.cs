using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatSelection : MonoBehaviour
{
    private const int MaxBleCheckCount = 20;
    public TextMeshProUGUI noMatText;

    public TextMeshProUGUI bleSuccessMsg;
    public TextMeshProUGUI passwordErrorText;
    public InputField inputPassword;
    public GameObject loadingPanel;
    public GameObject BluetoothSuccessPanel;

    public GameObject NoMatPanel;
    public GameObject SkipMatButton;
    public GameObject secretEntryPanel;
    public YipliConfig currentYipliConfig;
    private string connectionState;
    private int checkMatStatusCount;
    public GameObject tick;

    private bool autoSkipMatConnection;

    private bool bIsGameMainSceneLoading = false;

    private bool bIsRetryConnectionCalled = false;

    private bool bIsMatFlowInitialized = false;
    private void Start()
    {
        //Initialize
        bIsGameMainSceneLoading = false;

        if (currentYipliConfig.onlyMatPlayMode == false)
        {
            // Make the Skip button visible
            SkipMatButton.SetActive(true);
        }
        else
        {
            SkipMatButton.SetActive(false);
        }
    }

    // This function is to be called before Mat tutorial.
    // Mat tutorial requires the mat connection to be established.
    public void EstablishMatConnection()
    {
        Debug.Log("Starting Mat connection flow");
        NoMatPanel.SetActive(false);

#if UNITY_ANDROID
        if (currentYipliConfig.matInfo == null)
        {
            Debug.Log("Filling te current mat Info from Device saved MAT");
            currentYipliConfig.matInfo = UserDataPersistence.GetSavedMat();
        }

        if (currentYipliConfig.matInfo != null)
        {
            Debug.Log("Mac Address : " + currentYipliConfig.matInfo.macAddress);
            //Load Game scene if the mat is already connected.
            if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                StartCoroutine(ConnectMat());
            }                
        }
        else //Current Mat not found in Db.
        {
            loadingPanel.SetActive(false);
            Debug.Log("No Mat found in cache.");
            noMatText.text = ProductMessages.Err_mat_connection_android_phone_register;
            NoMatPanel.SetActive(true);
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
        }
#elif UNITY_STANDALONE_WIN
        if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
        {
            StartCoroutine(ConnectMat());
        }
#endif

        StartCoroutine(MatConnectionCheck());
    }

    // during gamelib scene processes keep checking for mat ble connection in android devices.
    private IEnumerator MatConnectionCheck()
    {
        yield return new WaitForSecondsRealtime(1f);

        while (true)
        {
            yield return new WaitForSecondsRealtime(0.5f);

            if (!YipliHelper.GetMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                NoMatPanel.SetActive(true);
            }
        }
    }

    public void MatConnectionFlow()
    {
        Debug.Log("Starting Mat connection flow");
        bIsMatFlowInitialized = true;
        bIsGameMainSceneLoading = false;
        NoMatPanel.SetActive(false);

#if UNITY_EDITOR
        if (!bIsGameMainSceneLoading)
            StartCoroutine(LoadMainGameScene());
#elif UNITY_ANDROID
        if (currentYipliConfig.matInfo == null)
        {
            Debug.Log("Filling te current mat Info from Device saved MAT");
            currentYipliConfig.matInfo = UserDataPersistence.GetSavedMat();
        }

        if (currentYipliConfig.matInfo != null)
        {
            Debug.Log("Mac Address : " + currentYipliConfig.matInfo.macAddress);
            //Load Game scene if the mat is already connected.
            if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                StartCoroutine(ConnectMat());
            }
        }
        else //Current Mat not found in Db.
        {
            loadingPanel.SetActive(false);
            Debug.Log("No Mat found in cache.");
            noMatText.text = ProductMessages.Err_mat_connection_android_phone_register;
            NoMatPanel.SetActive(true);
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
        }
#elif UNITY_STANDALONE_WIN
        if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
        {
            StartCoroutine(ConnectMat(true));
        }
#endif
    }

    public void ReCheckMatConnection()
    {
        Debug.Log("ReCheckMatConnection() called");
        if (bIsMatFlowInitialized)
            MatConnectionFlow();
        else
            EstablishMatConnection();
    }


    public void Update()
    {
        if (bIsMatFlowInitialized)
        {
            //LoadGameScene if mat connection is established
            if (InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                if (true != bIsGameMainSceneLoading)
                    StartCoroutine(LoadMainGameScene());
            }
        }
    }

    
    private IEnumerator ConnectMat(bool bIsReconnectMatNeeded = false)
    {
        int iTryCount = 0;

        //Initiate the connection with the mat.  
        try
        {
            if (bIsReconnectMatNeeded)
            {
                RetryMatConnectionOnPC();
            }
            else
            {
                InitiateMatConnection();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("mat connection failed : " + e.Message);

            loadingPanel.SetActive(false);
            NoMatPanel.SetActive(true);
            yield break;
        }

        yield return new WaitForSecondsRealtime(0.1f);

        //Turn on the Mat Find Panel, and animate
        loadingPanel.gameObject.GetComponentInChildren<Text>().text = "Finding your mat..";
        loadingPanel.SetActive(true);//Show msg till mat connection is confirmed.

        while (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase)
            && iTryCount < MaxBleCheckCount)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            iTryCount++;
        }

        //Turn off the Mat Find Panel
        loadingPanel.SetActive(false);
        loadingPanel.gameObject.GetComponentInChildren<Text>().text = "Fetching player details...";

        if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
        {
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
            Debug.Log("Mat not reachable.");

#if UNITY_ANDROID
                noMatText.text = ProductMessages.Err_mat_connection_mat_off;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (PortTestings.CheckAvailableComPorts() == 0)
            {
                noMatText.text = ProductMessages.Err_mat_connection_no_ports;
            }
            else
            {
                noMatText.text = ProductMessages.Err_mat_connection_mat_off;
            }

#endif

            NoMatPanel.SetActive(true);
        }
    }

    public void SkipMat()
    {
        NoMatPanel.SetActive(false);
        passwordErrorText.text = "";
        inputPassword.text = "";
        secretEntryPanel.SetActive(true);
    }

    public void OnPlayPress()
    {
        if (inputPassword.text == "123456")
        {
            //load last Scene
            if (!bIsGameMainSceneLoading)
                StartCoroutine(LoadMainGameScene());
        }
        else
        {
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure"); inputPassword.text = "";
            passwordErrorText.text = "Invalid pasword";
            Debug.Log("incorrect password");
        }
    }

    IEnumerator LoadMainGameScene()
    {
        /*
        bIsGameMainSceneLoading = true;
        loadingPanel.gameObject.GetComponentInChildren<Text>().text = "launching game..";
        loadingPanel.SetActive(false);
        NoMatPanel.SetActive(false);
        bleSuccessMsg.text = "Your YIPLI MAT is connected.";

        BluetoothSuccessPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        FindObjectOfType<YipliAudioManager>().Play("BLE_success");
        tick.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        */
        bIsGameMainSceneLoading = true;
        loadingPanel.gameObject.GetComponentInChildren<Text>().text = "launching game..";
        loadingPanel.SetActive(true);

        while (firebaseDBListenersAndHandlers.GetGameDataForCurrenPlayerQueryStatus() != QueryStatus.Completed)
        {
            Debug.Log("waiting to finish new player's game data");
            yield return new WaitForSecondsRealtime(0.1f);
        }


        //yield return null;
        StartCoroutine(LoadSceneAfterDisplayingDriverAndGameVersion());
    }

    IEnumerator LoadSceneAfterDisplayingDriverAndGameVersion()
    {
        //TODO : Comment following lines for production build and uncomment for display game and driver version
        //bleSuccessMsg.text = "FmDriver Version : " + YipliHelper.GetFMDriverVersion() + "\n Game Version : " + Application.version;
        //yield return new WaitForSeconds(1f);

        yield return null;

        // this check has to be false for every game scene load.
        currentYipliConfig.bIsChangePlayerCalled = false;

        //load last Scene
        SceneManager.LoadScene(currentYipliConfig.callbackLevel);
    }

    public void OnBackPress()
    {
        secretEntryPanel.SetActive(false);
        NoMatPanel.SetActive(true);
    }

    private void InitiateMatConnection()
    {
        //Initiate the connection with the mat.
        InitBLE.InitBLEFramework(currentYipliConfig.matInfo?.macAddress ?? "", 0);
    }

    private void RetryMatConnectionOnPC()
    {
        //Initiate the connection with the mat.
        //InitBLE.InitBLEFramework(currentYipliConfig.matInfo?.macAddress ?? "", 0);
        InitBLE.reconnectMat();
    }

    public void OnGoToYipliPress()
    {
        YipliHelper.GoToYipli();
    }
}

//Register the YIPLI fitness mat from Yipli Hub to continue playing.