using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMO.UI
{
    public class PanelActiveHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> panelList = new List<GameObject>();
        [SerializeField] private PanelManager panelManager;

        public void OnActivePanel()
        {
            panelManager = FindObjectsOfType<PanelManager>()[^1];
            panelManager.OnActivePanel(panelList);
        }
    }
}