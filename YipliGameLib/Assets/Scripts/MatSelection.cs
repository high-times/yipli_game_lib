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
    private const int MaxBleCheckCount=20;
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

    private void Start()
    {
#if UNITY_EDITOR
        currentYipliConfig.onlyMatPlayMode = false;
#endif
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
#if UNITY_EDITOR
        secretEntryPanel.SetActive(false);
        NoMatPanel.SetActive(false);
        StartCoroutine(LoadMainGameScene());
#endif
        Debug.Log("Starting Mat connection flow");

        NoMatPanel.SetActive(false);
        StopCoroutine(ConnectMatAndLoadGameScene());

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
                StartCoroutine(LoadMainGameScene());
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

    private IEnumerator ConnectMatAndLoadGameScene()
    {
        int iTryCount = 0;
        //Initiate the connection with the mat.
        InitiateMatConnection();

        //Turn on the Mat Find Panel, and animate
        loadingPanel.gameObject.GetComponentInChildren<Text>().text = "Finding your mat..";
        loadingPanel.SetActive(true);//Show msg till mat connection is confirmed.

        while (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase)
            && iTryCount < MaxBleCheckCount)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            iTryCount++;

            //Initiate Mat connection after every 2.5 seconds
            if (iTryCount % 10 == 0)
            {
                //Initiate the connection with the mat.
                InitiateMatConnection();
            }

        }

        //Turn off the Mat Find Panel
        loadingPanel.SetActive(false);
        loadingPanel.gameObject.GetComponentInChildren<Text>().text = "Fetching Player details...";

        if (InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
        {
            StartCoroutine(LoadMainGameScene());
        }
        else
        {
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
            Debug.Log("Mat not reachable.");
            noMatText.text = "Make sure that the registered mat is reachable.";
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
            currentYipliConfig.onlyMatPlayMode = false;
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
        loadingPanel.SetActive(false);
        Debug.Log("Your Fitmat is connected.Taking you to the game.");
        bleSuccessMsg.text = "Your Fitmat is connected.\nTaking you to the game.";
        BluetoothSuccessPanel.SetActive(true);
        yield return new WaitForSeconds(2f);

        FindObjectOfType<YipliAudioManager>().Play("BLE_success");
        yield return new WaitForSeconds(0.15f);
        tick.SetActive(true);
        yield return new WaitForSeconds(0.35f);
        Debug.Log("Starting the Coroutine LoadSceneAfterDisplayingDriverAndGameVersion()");
        StartCoroutine(LoadSceneAfterDisplayingDriverAndGameVersion());
    }

    IEnumerator LoadSceneAfterDisplayingDriverAndGameVersion()
    {
        loadingPanel.SetActive(false);
        //TODO : Comment following lines for production build
        Debug.Log("FmDriver Version : " + YipliHelper.GetFMDriverVersion());
        Debug.Log("Game Version: " + Application.version);
        bleSuccessMsg.text = "FmDriver Version : " + YipliHelper.GetFMDriverVersion() + "\n  Game Version : " + Application.version;
        yield return new WaitForSeconds(1.5f);

        BluetoothSuccessPanel.SetActive(false);
 
        //load last Scene
        SceneManager.LoadScene(currentYipliConfig.callbackLevel); 
    }

    public void OnBackPress()
    {
        secretEntryPanel.SetActive(false);
        NoMatPanel.SetActive(true);
    }

    //public void ReCheckMatConnection()
    //{
    //    Debug.Log("Checking Mat.");
    //    NoMatPanel.SetActive(false);
    //    string result = "failure";
    //    //To handle the case of No mats registered
    //    if ((currentYipliConfig.matInfo == null) || (currentYipliConfig.matInfo.macAddress.Length == 0))
    //    {
    //        loadingPanel.SetActive(true);
    //        ValidateAndInitiateMatConnection();
    //        loadingPanel.SetActive(false);
    //    }

    //    string connectionState = "";
    //    if ((currentYipliConfig.matInfo != null) || (result == "success"))
    //    {
    //        try
    //        {
    //            connectionState = InitBLE.getMatConnectionStatus();
    //        }
    //        catch (Exception exp)
    //        {
    //            Debug.Log("Exception occured in ReCheckMatConnection() : " + exp.Message);
    //        }

    //        if (connectionState.Equals("CONNECTED", StringComparison.OrdinalIgnoreCase))
    //        {
    //            //load last Scene
    //            StartCoroutine(LoadMainGameScene());
    //        }
    //        else
    //        {
    //            // If it is > 1, reCheck is clicked atleast once. After ReChecking the status, if the status isnt connected,
    //            //then initiate the Mat connection again, so that, in next reCheck it will get connected.
    //            try
    //            {
    //                loadingPanel.SetActive(true);
    //                string res = ValidateAndInitiateMatConnection();
    //                connectionState = InitBLE.getMatConnectionStatus();
    //                loadingPanel.SetActive(false);

    //                if (res == "success")
    //                {
    //                    if (connectionState.Equals("CONNECTED", StringComparison.OrdinalIgnoreCase))
    //                    {
    //                        //load last Scene
    //                        StartCoroutine(LoadMainGameScene());
    //                    }
    //                }
    //            }
    //            catch (Exception exp)
    //            {
    //                Debug.Log("Exception occured in ReCheckMatConnection() : " + exp.Message);
    //            }
    //            Debug.Log("Mat not reachable.");
    //            noMatText.text = "Make sure that the registered mat is reachable.";

    //            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
    //            NoMatPanel.SetActive(true);
    //        }
    //    }
    //    else //Current Mat not found in Db.
    //    {
    //        FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
    //        Debug.Log("No Mat found in cache.");
    //        if(YipliHelper.checkInternetConnection())
    //            noMatText.text = "Register MAT in the Yipli App to continue playing";
    //        else
    //            noMatText.text = "Register MAT in the Yipli App to continue playing with active network connection";

    //        NoMatPanel.SetActive(true);
    //    }
    //}


    /* This function is responsible for only initiating mat connection 
     * Checking if mat is connected or not, Loading game scene, isnt handled here */
    public void ValidateAndInitiateMatConnection()
    {
        Debug.Log("Starting mat connection");
        if (currentYipliConfig.matInfo != null)
        {
            try
            {
                if (currentYipliConfig.matInfo.macAddress.Length > 1)
                {
                    //Initiate the connection with the mat.
                    InitiateMatConnection();
                }
                else
                {
                    Debug.Log("No valid yipli mat found. Register a YIPLI mat and try again.");
                }
            }
            catch (Exception exp)
            {
                Debug.Log("Exception in InitBLEFramework :" + exp.Message);
            }
        }
        else
        {
            Debug.Log("No valid yipli mat found. Register a YIPLI mat and try again.");
        }
    }

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
