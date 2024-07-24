using System.Collections.Generic;
using UnityEngine;

namespace DEMO.UI
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> panelList = new List<GameObject>();
        [SerializeField] private List<bool> boolList = new List<bool>();

        private void Start()
        {
            for(int i = 0; i < panelList.Count ; i++)
            {
                boolList.Add(panelList[i].activeSelf);
            }
        }

        public void OnActivePanel(List<GameObject> list)
        {
            SetActiveFalse();

            foreach(var obj in list)
            {
                SetActive(list, true);
            }
        }

        private void SetActiveFalse()
        {
            List<GameObject> list = new List<GameObject>();
            
            for( int i = 0; i < boolList.Count; i++)
            {
                if(boolList[i])
                {
                    list.Add(panelList[i]);
                }
            }

            SetActive(list, false);
        }

        private void SetActive(List<GameObject> list, bool _bool)
        {
            foreach(var panel in list)
            {
                int id = panelList.IndexOf(panel);
                boolList[id] = _bool;

                panel.SetActive(_bool);
            }
        }
    }
}