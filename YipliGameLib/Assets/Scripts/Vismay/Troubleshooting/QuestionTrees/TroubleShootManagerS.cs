using UnityEngine;

[CreateAssetMenu(menuName = "TroubleShoot/SystemManager")]
public class TroubleShootManagerS : ScriptableObject
{
    public int currentAlgorithmID = -1;

    // process flags
    // game question flags
    private bool osUpdateCheck = false;
    private bool playerFetchingCheckDone = false;
    private bool noMatPanelCheckDone = false;
    private bool internetConnectionTest = false;
    private bool matUsbConnectionTest = false;
    private bool phoneBleTest = false;
    private bool matInYipliAccountCheckDone = false;
    private bool backgroundAppsRunningCheckDone = false;
    private bool gamesAndAppUpdateCheckDone = false;
    private bool sameBehaviourGamesAsked = false;
    private bool sameBehaviourPlatformAsked = false;
    private bool behaviourRondomOrPersistentAsked = false;

    // mat question flags
    private bool matOnCheck = false;
    private bool colorOfLED = false;
    private bool charginglightVisibility = false;
    private bool bleListHasYipliCheckDone = false;
    private bool siliconDriverInstallCheck = false;
    private bool siliconPortAvailability = false;
    private bool matConnectionToOtherDeviceCheckDone = false;
    private bool sameMatFromYipliCheckDone = false;

    public int CurrentAlgorithmID { get => currentAlgorithmID; set => currentAlgorithmID = value; }
    public bool OsUpdateCheck { get => osUpdateCheck; set => osUpdateCheck = value; }
    public bool PlayerFetchingCheckDone { get => playerFetchingCheckDone; set => playerFetchingCheckDone = value; }
    public bool NoMatPanelCheckDone { get => noMatPanelCheckDone; set => noMatPanelCheckDone = value; }
    public bool InternetConnectionTest { get => internetConnectionTest; set => internetConnectionTest = value; }
    public bool MatUsbConnectionTest { get => matUsbConnectionTest; set => matUsbConnectionTest = value; }
    public bool PhoneBleTest { get => phoneBleTest; set => phoneBleTest = value; }
    public bool MatInYipliAccountCheckDone { get => matInYipliAccountCheckDone; set => matInYipliAccountCheckDone = value; }
    public bool BackgroundAppsRunningCheckDone { get => backgroundAppsRunningCheckDone; set => backgroundAppsRunningCheckDone = value; }
    public bool GamesAndAppUpdateCheckDone { get => gamesAndAppUpdateCheckDone; set => gamesAndAppUpdateCheckDone = value; }
    public bool SameBehaviourGamesAsked { get => sameBehaviourGamesAsked; set => sameBehaviourGamesAsked = value; }
    public bool SameBehaviourPlatformAsked { get => sameBehaviourPlatformAsked; set => sameBehaviourPlatformAsked = value; }
    public bool BehaviourRondomOrPersistentAsked { get => behaviourRondomOrPersistentAsked; set => behaviourRondomOrPersistentAsked = value; }
    public bool MatOnCheck { get => matOnCheck; set => matOnCheck = value; }
    public bool ColorOfLED { get => colorOfLED; set => colorOfLED = value; }
    public bool CharginglightVisibility { get => charginglightVisibility; set => charginglightVisibility = value; }
    public bool BleListHasYipliCheckDone { get => bleListHasYipliCheckDone; set => bleListHasYipliCheckDone = value; }
    public bool SiliconDriverInstallCheck { get => siliconDriverInstallCheck; set => siliconDriverInstallCheck = value; }
    public bool SiliconPortAvailability { get => siliconPortAvailability; set => siliconPortAvailability = value; }
    public bool MatConnectionToOtherDeviceCheckDone { get => matConnectionToOtherDeviceCheckDone; set => matConnectionToOtherDeviceCheckDone = value; }
    public bool SameMatFromYipliCheckDone { get => sameMatFromYipliCheckDone; set => sameMatFromYipliCheckDone = value; }
}
