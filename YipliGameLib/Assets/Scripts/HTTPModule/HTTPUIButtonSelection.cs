using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yipli.HttpMpdule
{
    public class HTTPUIButtonSelection : MonoBehaviour
    {
        int currentIndex = 0;

        List<Button> thisPanelButtons = new List<Button>();

        [SerializeField] HTTPMatController mic;

        bool isPlayerSelectionPanel = false;

        private void OnEnable()
        {
            mic.UpdateButtonList(thisPanelButtons, currentIndex, true);
        }
        // PlayerSelectionPanel
    }
}