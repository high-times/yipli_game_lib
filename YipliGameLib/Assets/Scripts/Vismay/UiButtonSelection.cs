using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonSelection : MonoBehaviour
{
    int currentIndex = 0;

    List<Button> thisPanelButtons = new List<Button>();

    [SerializeField] MatInputController mic;

    bool isPlayerSelectionPanel = false;

    private void OnEnable()
    {
        thisPanelButtons = new List<Button>();
        foreach (var item in GetComponentsInChildren<Button>())
        {
            if (!item.navigation.Equals(Navigation.Mode.None))
            {
                thisPanelButtons.Add(item);
            }
        }

        if (gameObject.CompareTag("PlayerSelectionPanel"))
        {
            isPlayerSelectionPanel = true;
        }
        else
        {
            isPlayerSelectionPanel = false;      
        }

        mic.UpdateButtonList(thisPanelButtons, currentIndex, isPlayerSelectionPanel);
    }
}
// PlayerSelectionPanel