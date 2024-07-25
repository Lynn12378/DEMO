using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMO.UI
{
    public class PanelActiveHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> panelList = new List<GameObject>();
        [SerializeField] private PanelManager panelManager = null;

        public void OnActivePanel()
        {
            if(panelManager == null)
            {
                panelManager = FindObjectOfType<PanelManager>();
            }
            panelManager.OnActivePanel(panelList);
        }
    }
}