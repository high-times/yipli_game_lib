using System;
using System.Collections;
using UnityEngine;

namespace Yipli.HttpMpdule
{
    public class HTTPMatManager : MonoBehaviour
    {
        // required variables
        // consta values
        private const int MaxBleCheckCount = 20;


        [Header("Scriptable Objects")]
        [SerializeField] private HTTPYipliConfig currentYipliConfig = null;
        [SerializeField] private NewMatInputController newMatInputController = null;
        [SerializeField] private HTTPPlayerSelection playerSelection = null;

        [Header("UI Panel")]
        [SerializeField] private GameObject NoMatPanel = null;
        [SerializeField] private GameObject loadingPanel = null;

        // private variables
        private bool bIsMatFlowInitialized = false;
        private int retriesDone = 0;

        // Custom Operations
        public void LoadMainGameSceneDirectly()
        {
            if (currentYipliConfig.SceneLoadedDirectly) return;

            currentYipliConfig.SceneLoadedDirectly = true;

            Debug.LogError("onlyMatPlayMode : From LoadMainGameSceneDirectly");
            //StartCoroutine(LoadMainGameScene());
            playerSelection.DataManagementIsFinished();
        }

        public void MatConnectionFlow()
        {
            Debug.Log("Starting Mat connection flow");
            NoMatPanel.SetActive(false);
            newMatInputController.MakeSortLayerTen();

            if (currentYipliConfig.CurrentActiveMatData != null || currentYipliConfig.IsDeviceAndroidTV)
            {
                //Load Game scene if the mat is already connected.
                if (!InitBLE.getMatConnectionStatus().Equals("connected", System.StringComparison.OrdinalIgnoreCase))
                {
                    ConnectMat();
                }
            }
            else //Current Mat not found in Db.
            {
                loadingPanel.SetActive(false);
                Debug.Log("No Mat found in cache.");
                //noMatText.text = ProductMessages.Err_mat_connection_android_phone_register;
                //newUIManager.UpdateButtonDisplay(NoMatPanel.tag);
                newMatInputController.MakeSortLayerZero();
                NoMatPanel.SetActive(true);
                //newUIManager.TurnOffMainCommonButton();
                FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
            }
        }

        private IEnumerator ConnectMat() {
            int iTryCount = 0;

            //Initiate the connection with the mat.  
            try
            {
                 InitiateMatConnection();
            }
            catch (Exception e)
            {
                Debug.LogError("mat connection failed : " + e.Message);

                loadingPanel.SetActive(false);
                //newUIManager.UpdateButtonDisplay(NoMatPanel.tag);
                newMatInputController.MakeSortLayerZero();
                NoMatPanel.SetActive(true);
                yield break;
            }

            yield return new WaitForSecondsRealtime(0.1f);

            //Turn on the Mat Find Panel, and animate
            //loadingPanel.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Finding your mat..";
            loadingPanel.SetActive(true);//Show msg till mat connection is confirmed.

            while (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase)
                && iTryCount < MaxBleCheckCount)
            {
    #if UNITY_IOS
                yield return new WaitForSecondsRealtime(1f);
    #else
                yield return new WaitForSecondsRealtime(0.25f);
    #endif
                iTryCount++;
            }

            //Turn off the Mat Find Panel
            loadingPanel.SetActive(false);
            //loadingPanel.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Fetching player details...";

            if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
                Debug.Log("Mat not reachable.");
                //newUIManager.UpdateButtonDisplay(NoMatPanel.tag);
                newMatInputController.MakeSortLayerZero();
                NoMatPanel.SetActive(true);
            }
        }

        private void InitiateMatConnection()
        {
            //Initiate the connection with the mat.
    #if UNITY_IOS
            // connection part for ios
            InitBLE.InitBLEFramework(currentYipliConfig.CurrentActiveMatData.MacAddress ?? "", 0, currentYipliConfig.CurrentActiveMatData.MacName ?? LibConsts.MatTempAdvertisingNameOnlyForNonIOS);
    #elif UNITY_ANDROID
            InitBLE.InitBLEFramework(currentYipliConfig.CurrentActiveMatData.MacAddress ?? "", 0, currentYipliConfig.CurrentActiveMatData.MacName ?? LibConsts.MatTempAdvertisingNameOnlyForNonIOS, currentYipliConfig.IsDeviceAndroidTV);
    #else
            InitBLE.InitBLEFramework(currentYipliConfig.CurrentActiveMatData.MacAddress ?? "", 0);
    #endif
        }

        public void LoadMainGameSceneIfMatIsConnected()
        {
            if (HTTPHelper.GetMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase) || !currentYipliConfig.OnlyMatPlayMode)
            {
                LoadMainGameScene();
            }
            else
            {
                MatConnectionFlow();
            }
        }

        private void LoadMainGameScene()
        {
            loadingPanel.SetActive(true);

            //load last Scene
            playerSelection.DataManagementIsFinished();
        }

        // recheck Mat connection
        public void ReCheckMatConnection()
        {
            //newUIManager.TurnOffMainCommonButton();

            Debug.Log("ReCheckMatConnection() called");
            if (bIsMatFlowInitialized)
                MatConnectionFlow();
            else
                EstablishMatConnection();

            retriesDone++;
        }

        public void EstablishMatConnection()
        {
            NoMatPanel.SetActive(false);
            newMatInputController.MakeSortLayerTen();

        #if UNITY_ANDROID
            if (currentYipliConfig.CurrentActiveMatData != null || currentYipliConfig.IsDeviceAndroidTV)
            {
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
                newMatInputController.MakeSortLayerZero();
                NoMatPanel.SetActive(true);
                FindObjectOfType<YipliAudioManager>().Play("BLE_failure");
            }
        #elif UNITY_STANDALONE_WIN
            if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                StartCoroutine(ConnectMat());
            }
        #elif UNITY_IOS
            if (!InitBLE.getMatConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                StartCoroutine(ConnectMat());
            }
        #endif
        }
    }
}