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

    private bool bIsMatFlowInitialized = false;
    private void Start()
    {
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

    public void MatConnectionFlow()
    {
        Debug.Log("Starting Mat connection flow");
        bIsMatFlowInitialized = true;
        NoMatPanel.SetActive(false);
        StopCoroutine(ConnectMatAndLoadGameScene());

        //Bypass mat connection in Unity Editor
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (!bIsGameMainSceneLoading)
                StartCoroutine(LoadMainGameScene());
        }

        if (currentYipliConfig.matInfo == null)
        {
            Debug.Log("Filling te current mat Info from Device saved MAT");
            currentYipliConfig.matInfo = UserDataPersistence.GetSavedMat();
        }

        if (currentYipliConfig.matInfo != null)
        {
            Debug.Log("Mac Address : " + currentYipliConfig.matInfo.macAddress);
            //Load Game scene if the mat is already connected.
            if (InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                if (!bIsGameMainSceneLoading)
                    StartCoroutine(LoadMainGameScene());
            }
            else
                StartCoroutine(ConnectMatAndLoadGameScene());
        }
        else //Current Mat not found in Db.
        {
            Debug.Log("No Mat found in cache.");
            noMatText.text = "Register the YIPLI fitness mat from Yipli Hub to continue playing.";
            NoMatPanel.SetActive(true);
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
        }
    }

    public void ReCheckMatConnection()
    {
        Debug.Log("ReCheckMatConnection() called");
        MatConnectionFlow();
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


    private IEnumerator ConnectMatAndLoadGameScene()
    {
        int iTryCount = 0;

        //Initiate the connection with the mat.
        InitiateMatConnection();
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
            noMatText.text = "Make sure that your active Yipli mat is not very far from your device.";
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
        bIsGameMainSceneLoading = true;
        loadingPanel.SetActive(false);
        NoMatPanel.SetActive(false);
        bleSuccessMsg.text = "Your Fitmat is connected.\nTaking you to the game.";
        BluetoothSuccessPanel.SetActive(true);
        yield return new WaitForSeconds(1f);

        FindObjectOfType<YipliAudioManager>().Play("BLE_success");
        tick.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LoadSceneAfterDisplayingDriverAndGameVersion());
    }

    IEnumerator LoadSceneAfterDisplayingDriverAndGameVersion()
    {
        //TODO : Comment following lines for production build
        bleSuccessMsg.text = "FmDriver Version : " + YipliHelper.GetFMDriverVersion() + "\n Game Version : " + Application.version;
        yield return new WaitForSeconds(1f);

        //load last Scene
        SceneManager.LoadScene(currentYipliConfig.callbackLevel);
    }

    public void OnBackPress()
    {
        secretEntryPanel.SetActive(false);
        NoMatPanel.SetActive(true);
    }

    /* This function is responsible for only initiating mat connection 
     * Checking if mat is connected or not, Loading game scene, isnt handled here */
    //public void ValidateAndInitiateMatConnection()
    //{
    //    Debug.Log("Starting mat connection");
    //    if (currentYipliConfig.matInfo != null)
    //    {
    //        try
    //        {
    //            if (currentYipliConfig.matInfo.macAddress.Length > 1)
    //            {
    //                //Initiate the connection with the mat.
    //                InitiateMatConnection();
    //            }
    //            else
    //            {
    //                Debug.Log("No valid yipli mat found. Register a YIPLI mat and try again.");
    //            }
    //        }
    //        catch (Exception exp)
    //        {
    //            Debug.Log("Exception in InitBLEFramework :" + exp.Message);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("No valid yipli mat found. Register a YIPLI mat and try again.");
    //    }
    //}

    private void InitiateMatConnection()
    {
        Debug.Log("connecting to : " + currentYipliConfig.matInfo.matName);

        //Initiate the connection with the mat.
        InitBLE.InitBLEFramework(currentYipliConfig.matInfo.macAddress, 0);
    }

    public void OnGoToYipliPress()
    {
        YipliHelper.GoToYipli();
    }
}
