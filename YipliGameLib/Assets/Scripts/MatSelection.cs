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
        Debug.Log("Checking Mat.");

        NoMatPanel.SetActive(false);

        string connectionState = "";
        if (currentYipliConfig.matInfo != null && currentYipliConfig.matInfo.macAddress.Length > 0)
        {
            connectionState = InitBLE.getMatConnectionStatus();
            if (currentYipliConfig.onlyMatPlayMode == false)
                connectionState = "connected";

            if (connectionState.Equals("CONNECTED", StringComparison.OrdinalIgnoreCase))
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
        else //Current Mat not found in Db.
        {
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
            Debug.Log("No Mat found in cache.");
            NoMatPanel.SetActive(true);
#if UNITY_EDITOR
            secretEntryPanel.SetActive(false);  
            NoMatPanel.SetActive(false);
            StartCoroutine(LoadMainGameScene());
#endif
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

    public void ReCheckMatConnection()
    {
        Debug.Log("Checking Mat.");
        NoMatPanel.SetActive(false);
        string result = "failure";
        //To handle the case of No mats registered
        if ((currentYipliConfig.matInfo == null) || (currentYipliConfig.matInfo.macAddress.Length == 0))
        {
            loadingPanel.SetActive(true);
            result = ValidateAndConnectMat();
            loadingPanel.SetActive(false);
        }

        string connectionState = "";
        if ((currentYipliConfig.matInfo != null) || (result == "success"))
        {
            try
            {
                connectionState = InitBLE.getMatConnectionStatus();
            }
            catch (Exception exp)
            {
                Debug.Log("Exception occured in ReCheckMatConnection() : " + exp.Message);
            }

            if (connectionState.Equals("CONNECTED", StringComparison.OrdinalIgnoreCase))
            {
                //load last Scene
                StartCoroutine(LoadMainGameScene());
            }
            else
            {
                // If it is > 1, reCheckis clicked atleast once. After ReChecking the status, if the status isnt connected,
                //then initiate the Mat connection again, so that, in next reCheck it will get connected.
                try
                {
                    loadingPanel.SetActive(true);
                    string res = ValidateAndConnectMat();
                    connectionState = InitBLE.getMatConnectionStatus();
                    loadingPanel.SetActive(false);

                    if (res == "success")
                    {
                        if (connectionState.Equals("CONNECTED", StringComparison.OrdinalIgnoreCase))
                        {
                            //load last Scene
                            StartCoroutine(LoadMainGameScene());
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.Log("Exception occured in ReCheckMatConnection() : " + exp.Message);
                }
                Debug.Log("Mat not reachable.");
                noMatText.text = "Make sure that the registered mat is reachable.";

                FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
                NoMatPanel.SetActive(true);
            }
        }
        else //Current Mat not found in Db.
        {
            FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
            Debug.Log("No Mat found in cache.");
            if(YipliHelper.checkInternetConnection())
                noMatText.text = "Register MAT in the Yipli App to continue playing";
            else
                noMatText.text = "Register MAT in the Yipli App to continue playing with active network connection";

            NoMatPanel.SetActive(true);
        }
    }

    public string ValidateAndConnectMat()
    {
        Debug.Log("Starting mat connection");
        try
        {
            if (YipliHelper.checkInternetConnection())
            {
                //Allow backent Get calls only if network is reachable
                if (currentYipliConfig.matInfo != null)
                {
                    UserDataPersistence.SaveMatToDevice(currentYipliConfig.matInfo);
                }
            }
            else
            { 
                //No Network case handling
                Debug.Log("Network not reachable");
                //Take the default mat info stored in the Config
                if (currentYipliConfig.matInfo == null)
                {
                    currentYipliConfig.matInfo = UserDataPersistence.GetSavedMat();
                }
            }

            if (currentYipliConfig.matInfo != null)
            {
                try
                {
                    if (currentYipliConfig.matInfo.macAddress.Length > 1)
                    {
                        Debug.Log("connecting to : " + currentYipliConfig.matInfo.matName);
                        //Initiate the connection with the mat.
                        InitBLE.InitBLEFramework(currentYipliConfig.matInfo.macAddress, 0);
                        return "success";
                    }
                    else
                    {
                        Debug.Log("No valid yipli mat found. Register a YIPLI mat and try again.");
                    }
                }
                catch(Exception exp)
                {
                    Debug.Log("Exception in InitBLEFramework :" + exp.Message);
                }
            }
            else
            {
                Debug.Log("No valid yipli mat found. Register a YIPLI mat and try again.");
            }
        }
        catch (Exception exp)
        {
            Debug.Log("Exception occured in ConnectMat(). Check if the Mat is registered wih Valid Mac ID.");
            Debug.Log(exp.Message);
        }
        return "failure";
    }


    public void OnGoToYipliPress()
    {
        YipliHelper.GoToYipli();
    }
}
