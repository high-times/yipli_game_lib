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
    private string maintenanceMessage = "";

    public bool IsGameUnderMaintanance { get => isGameUnderMaintanance; set => isGameUnderMaintanance = value; }
    public string GameNameThatisUnderMaintenance { get => gameNameThatisUnderMaintenance; set => gameNameThatisUnderMaintenance = value; }
    public string MaintenanceMessage { get => maintenanceMessage; set => maintenanceMessage = value; }

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

        if (!IsGameUnderMaintanance) return;

        GameNameThatisUnderMaintenance = ConfigManager.appConfig.GetString("gameNameThatisUndermaintenance");
        MaintenanceMessage = ConfigManager.appConfig.GetString("maintenanceMessage");

        string[] gameIds = GameNameThatisUnderMaintenance.Split(',');

        foreach (string game in gameIds)
        {
            if (IsGameUnderMaintanance && game.Equals(currentYipliConfig.gameId))
            {
                message.text = char.ToUpper(GameNameThatisUnderMaintenance[0]) + GameNameThatisUnderMaintenance.Substring(1) + " is under maintanance.\n\n" + MaintenanceMessage + "\n\nStay tuned.";
                maintenancePanel.SetActive(true);
            }
            else
            {
                maintenancePanel.SetActive(false);
            }
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
