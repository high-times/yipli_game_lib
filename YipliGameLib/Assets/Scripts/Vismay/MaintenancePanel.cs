using UnityEngine;
using Unity.RemoteConfig;
using TMPro;

public class MaintenancePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] GameObject maintenancePanel;
    [SerializeField] YipliConfig currentYipliConfig;

    public struct userAttributes { }
    public struct appAtrributes { }

    private bool isGameUnderMaintanance = false;
    private string gameNameThatisUnderMaintenance = "";

    public bool IsGameUnderMaintanance { get => isGameUnderMaintanance; set => isGameUnderMaintanance = value; }
    public string GameNameThatisUnderMaintenance { get => gameNameThatisUnderMaintenance; set => gameNameThatisUnderMaintenance = value; }

    private void Awake()
    {
        ConfigManager.FetchCompleted += ManagePanel;
        ConfigManager.FetchConfigs<userAttributes, appAtrributes>(new userAttributes(), new appAtrributes());
    }

    private void Start()
    {
        maintenancePanel.SetActive(false);
    }

    private void Update()
    {
        ConfigManager.FetchConfigs<userAttributes, appAtrributes>(new userAttributes(), new appAtrributes());
    }

    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= ManagePanel;
    }

    private void ManagePanel(ConfigResponse response)
    {
        IsGameUnderMaintanance = ConfigManager.appConfig.GetBool("isGameUnderMaintenance");
        GameNameThatisUnderMaintenance = ConfigManager.appConfig.GetString("gameNameThatisUndermaintenance");

        if (GameNameThatisUnderMaintenance == currentYipliConfig.gameId)
        {
            message.text = GameNameThatisUnderMaintenance + " is Under Maintance. It will be available soon to play.\nThank You";
            maintenancePanel.SetActive(true);
        }
        else
        {
            maintenancePanel.SetActive(false);
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
