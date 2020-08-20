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
    private YipliMatInfo yipliMat;
    public GameObject BluetoothSuccessPanel;

    public GameObject NoMatPanel;
    public GameObject secretEntryPanel;
    public YipliConfig currentYipliConfig;
    private string connectionState;
    private int checkMatStatusCount;
    public GameObject tick;

    public void MatConnectionFlow()
    {
        Debug.Log("Checking Mat.");

        NoMatPanel.SetActive(false);

        string connectionState = "";
        if (yipliMat != null && yipliMat.macAddress.Length > 0)
        {
            connectionState = InitBLE.getBLEStatus();

            if (currentYipliConfig.matPlayMode == false)
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
            currentYipliConfig.matPlayMode = false;
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
        bleSuccessMsg.text = "Your Fitmat is connected.\nTaking you to the game.";
        BluetoothSuccessPanel.SetActive(true);
        yield return new WaitForSeconds(2f);

        FindObjectOfType<YipliAudioManager>().Play("BLE_success");
        yield return new WaitForSeconds(0.15f);
        tick.SetActive(true);
        yield return new WaitForSeconds(0.35f);
        StartCoroutine(LoadSceneAfterDisplayingDriverAndGameVersion());
    }

    IEnumerator LoadSceneAfterDisplayingDriverAndGameVersion()
    {
        //TODO : Comment following lines for production build
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

    public async void ReCheckMatConnection()
    {
        Debug.Log("Checking Mat.");
        NoMatPanel.SetActive(false);
        string result = "failure";
        //To handle the case of No mats registered
        if ((yipliMat == null) || (yipliMat.macAddress.Length == 0))
        {
            loadingPanel.SetActive(true);
            result = await ValidateAndConnectMat();
            loadingPanel.SetActive(false);
        }

        string connectionState = "";
        if ((yipliMat != null) || (result == "success"))
        {
            try
            {
                connectionState = InitBLE.getBLEStatus();
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
                    string res = await ValidateAndConnectMat();
                    connectionState = InitBLE.getBLEStatus();
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

    public async Task<string> ValidateAndConnectMat()
    {
        Debug.Log("Starting mat connection");
        try
        {
            if (YipliHelper.checkInternetConnection())
            {
                //Allow backent Get calls only if network is reachable
                yipliMat = await FirebaseDBHandler.GetCurrentMatDetails(currentYipliConfig.userId, () => { Debug.Log("Got the Mat details from db"); });
                if(yipliMat != null)
                {
                    currentYipliConfig.matInfo = yipliMat;
                    UserDataPersistence.SaveMatToDevice(yipliMat);
                }
            }
            else
            { 
                //No Network case handling
                Debug.Log("Network not reachable");
                //Take the default mat info stored in the Config
                if (currentYipliConfig.matInfo != null)
                {
                    yipliMat = currentYipliConfig.matInfo;
                }
                else
                {
                    yipliMat = UserDataPersistence.GetSavedMat();
                }
            }

            if (yipliMat != null)
            {
                try
                {
                    if (yipliMat.macAddress.Length > 1)
                    {
                        Debug.Log("connecting to : " + yipliMat.matName);
                        //Initiate the connection with the mat.
                        InitBLE.InitBLEFramework(yipliMat.macAddress);
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
