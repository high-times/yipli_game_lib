using UnityEngine;
using TMPro;

public class MaintenancePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;
    //[SerializeField] TextMeshProUGUI title;

    [SerializeField] GameObject maintenancePanel;
    [SerializeField] GameObject skipButton;

    [SerializeField] YipliConfig currentYipliConfig;

    [SerializeField] NewUIManager newUIManager = null;

    private void Start()
    {
        maintenancePanel.SetActive(false);
        newUIManager.TurnOffMainCommonButton();
        skipButton.SetActive(false);
    }

    private void Update()
    {
        ManageManitanenceOrBlocking();
        //BlockIfTroubleShootingIsOn();
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    private void ManageManitanenceOrBlocking()
    {
        if (currentYipliConfig.gameInventoryInfo == null) return;

        /*
        // for testing turn isGameUnderMaintenance to 0, or it has to be 1. Do not change value in firebase, that will immediately block live customers gameplay
        if (currentYipliConfig.gameInventoryInfo.isGameUnderMaintenance == 1) // change to 1 for release
        {
            string[] allOS = currentYipliConfig.gameInventoryInfo.osListForMaintanence.Split(',');

            Debug.LogError("Executing allOS length : " + allOS.Length);

            if (allOS.Length > 0) {
                for (int i = 0; i < allOS.Length; i++) {
                    if (allOS[i] == "a" && Application.platform == RuntimePlatform.Android) {
                        //Debug.LogError("Executing a");
                        ManageMaintanenceMessages();
                        break;
                    } else if (allOS[i] == "atv" && Application.platform == RuntimePlatform.Android && currentYipliConfig.isDeviceAndroidTV) {
                        //Debug.LogError("Executing atv");
                        ManageMaintanenceMessages();
                        break;
                    } else if (allOS[i] == "i" && Application.platform == RuntimePlatform.IPhonePlayer) {
                        //Debug.LogError("Executing i");
                        ManageMaintanenceMessages();
                        break;
                    } else if (allOS[i] == "w" && Application.platform == RuntimePlatform.WindowsPlayer) {
                        // for testing in editor (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                        //Debug.LogError("Executing w");
                        ManageMaintanenceMessages();
                        break;
                    }
                }
            }
            return;
        }
        */

        string[] allOS = currentYipliConfig.gameInventoryInfo.osListForMaintanence.Split(',');

        //Debug.LogError("Executing allOS length : " + allOS.Length);

        if (allOS.Length > 0) {
            for (int i = 0; i < allOS.Length; i++) {
                if (allOS[i] == "a" && Application.platform == RuntimePlatform.Android) {
                    //Debug.LogError("Executing a");
                    ManageMaintanenceMessages();
                    break;
                } else if (allOS[i] == "atv" && Application.platform == RuntimePlatform.Android && currentYipliConfig.isDeviceAndroidTV) {
                    //Debug.LogError("Executing atv");
                    ManageMaintanenceMessages();
                    break;
                } else if (allOS[i] == "i" && Application.platform == RuntimePlatform.IPhonePlayer) {
                    //Debug.LogError("Executing i");
                    ManageMaintanenceMessages();
                    break;
                } else if (allOS[i] == "w" && Application.platform == RuntimePlatform.WindowsPlayer) {
                    // for testing in editor (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                    //Debug.LogError("Executing w");
                    ManageMaintanenceMessages();
                    break;
                }
            }

            return;
        }

#if UNITY_ANDROID
        if (currentYipliConfig.isDeviceAndroidTV) {
            BlockVersionCheck(currentYipliConfig.gameInventoryInfo.androidTVMinVersion, currentYipliConfig.gameInventoryInfo.gameVersion);
        } else {
            BlockVersionCheck(currentYipliConfig.gameInventoryInfo.androidMinVersion, currentYipliConfig.gameInventoryInfo.gameVersion);
        }
#elif UNITY_IOS
        BlockVersionCheck(currentYipliConfig.gameInventoryInfo.iosMinVersion, currentYipliConfig.gameInventoryInfo.iosCurrentVersion);
#elif UNITY_STANDALONE_WIN
        BlockVersionCheck(currentYipliConfig.gameInventoryInfo.winMinVersion, currentYipliConfig.gameInventoryInfo.winCurrentVersion);
#endif
    }

    private void ManageMaintanenceMessages() {
        //message.text = char.ToUpper(currentYipliConfig.gameId[0]) + currentYipliConfig.gameId.Substring(1) + " is under maintanance." + "\n\nStay tuned.";
        message.text = currentYipliConfig.gameInventoryInfo.maintenanceMessage;
        //title.text = "Maintenance Notice";

        newUIManager.UpdateButtonDisplay(maintenancePanel.tag, true);
        FindObjectOfType<NewMatInputController>().MakeSortLayerZero();
        maintenancePanel.SetActive(true);
    }

    private void BlockVersionCheck(string notAllowedVersionString, string currentStoreVersion)
    {
        if (notAllowedVersionString.Equals(",", System.StringComparison.OrdinalIgnoreCase)) return;

        int gameVersionCode = YipliHelper.convertGameVersionToBundleVersionCode(Application.version);
        int notAllowedVersionCode = YipliHelper.convertGameVersionToBundleVersionCode(notAllowedVersionString);
        //int currentStoreCode = YipliHelper.convertGameVersionToBundleVersionCode(currentStoreVersion);

        if (notAllowedVersionCode > gameVersionCode)
        {
            message.text = currentYipliConfig.gameInventoryInfo.versionUpdateMessage;

            maintenancePanel.SetActive(true);
            FindObjectOfType<NewMatInputController>().MakeSortLayerZero();

            newUIManager.UpdateButtonDisplay(maintenancePanel.tag);
        }
        /*
        else if (notAllowedVersionCode < gameVersionCode && currentStoreCode > gameVersionCode) {
            if (currentYipliConfig.skipNormalUpdateClicked) return;

            message.text = "A new version of " + currentYipliConfig.gameInventoryInfo.displayName + " is available.\nUpdate recommended";

            maintenancePanel.SetActive(true);
            FindObjectOfType<NewMatInputController>().MakeSortLayerZero();
            skipButton.SetActive(true);

            newUIManager.UpdateButtonDisplay(maintenancePanel.tag);
        }
        */
        else
        {
            skipButton.SetActive(false);
            maintenancePanel.SetActive(false);
        }
    }

    private void BlockIfTroubleShootingIsOn()
    {
        if (currentYipliConfig.thisUserTicketInfo.ticketStatus == 0) return;

#if UNITY_ANDROID || UNITY_IOS
        TroubleShootBlockCheck(currentYipliConfig.thisUserTicketInfo.bleTest);
#elif UNITY_STANDALONE_WIN
        TroubleShootBlockCheck(currentYipliConfig.thisUserTicketInfo.usbTest);
#endif
    }

    private void TroubleShootBlockCheck(string testStatusToCheck)
    {
        if (testStatusToCheck.Equals("done", System.StringComparison.OrdinalIgnoreCase))
        {
            message.text = "Your environment is currently under troubleshooting. The game is not playable here at this time.";
            //title.text = "Troubleshoot Notice";

            maintenancePanel.SetActive(true);
        }
        else
        {
            skipButton.SetActive(false);
            maintenancePanel.SetActive(false);
            //newUIManager.TurnOffMainCommonButton();
        }
    }

    // button functions
    public void SkipButtonFunction() {
        currentYipliConfig.skipNormalUpdateClicked = true;
        skipButton.SetActive(false);
        maintenancePanel.SetActive(false);

        FindObjectOfType<NewMatInputController>().MakeSortLayerTen();
    }
}