using UnityEngine;

namespace GL.UI.PanelManagers
{
    public class NoInternetPanelManager : MonoBehaviour
    {
        // required variables
        [SerializeField] YipliConfig currentYipliConfig = null;
        [SerializeField] GameObject noInternetPanel = null;

        public void ManageNoInternetPanel()
        {
            if (currentYipliConfig.bIsInternetConnected == false)
            {
                noInternetPanel.SetActive(true);
            }
            else
            {
                noInternetPanel.SetActive(false);
            }
        }
    }
}
