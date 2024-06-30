using System.Collections;
using System.Collections.Generic;
// using System.Threading.Tasks;
using UnityEngine;

namespace DEMO.Manager{
    public class PanelManager : MonoBehaviour
    {
        public enum PanelSet
        {
            Launch = 0,
            Login = 1,
            SignUp = 2,
            Lobby = 3,
            RoomList = 4,
            FindRoom = 5
        }
    
        [SerializeField] public GameObject LaunchPanel;
        [SerializeField] public GameObject LoginPanel;
        [SerializeField] public GameObject SignUpPanel;
        [SerializeField] public GameObject LobbyPanel;
        [SerializeField] public GameObject RoomListPanel;
        [SerializeField] public GameObject FindRoomPanel;

        public Dictionary<PanelSet, GameObject> panelList = new Dictionary<PanelSet, GameObject>();
        public Dictionary<PanelSet, bool> panelState = new Dictionary<PanelSet, bool>();

        public void Start()
        {
            Init();
        }

        public void ActivePanel()
        {
            var temps = new Dictionary<PanelSet, bool>();
            var changes = new List<PanelSet>();
     
            foreach(var temp  in panelState)
            {
                temps.Add(temp.Key, temp.Value);
            }

            foreach(var panel in panelList)
            {
                if(!panel.Value.activeSelf == temps[panel.Key])
                {
                    changes.Add(panel.Key);
                }
            }
            
            foreach(var change in changes)
            {
                panelList[change].SetActive(panelState[change]);
            }
        }

        public void SetFalse()
        { 
            var keys = new List<PanelSet>();
            foreach(var key in panelState.Keys)
            {
                keys.Add(key);
            }

            foreach(var key in keys)
            {
                panelState[key] = false;
            }
        }

        public void Init()
        {
            panelList.Add(PanelSet.Launch, LaunchPanel);
            panelList.Add(PanelSet.Login, LoginPanel);
            panelList.Add(PanelSet.SignUp, SignUpPanel);
            panelList.Add(PanelSet.Lobby, LobbyPanel);
            panelList.Add(PanelSet.RoomList, RoomListPanel);
            panelList.Add(PanelSet.FindRoom, FindRoomPanel);

            panelState.Add(PanelSet.Launch, false);
            panelState.Add(PanelSet.Login, false);
            panelState.Add(PanelSet.SignUp, false);
            panelState.Add(PanelSet.Lobby, false);
            panelState.Add(PanelSet.RoomList, false);
            panelState.Add(PanelSet.FindRoom, false);
        }

        #region - Button Action -
            public void OnActiveLaunchPanel()
            {
                SetFalse();
                panelState[PanelSet.Launch] = true;
                ActivePanel();
            }

            public void OnActiveLoginPanel()
            {
                SetFalse();
                panelState[PanelSet.Launch] = true;
                panelState[PanelSet.Login] = true;
                ActivePanel();
            }

            public void OnActiveSignUpPanel()
            {
                SetFalse();
                panelState[PanelSet.Launch] = true;
                panelState[PanelSet.SignUp] = true;
                ActivePanel();
            }

            public void OnActiveLobbyPanel()
            {
                SetFalse();
                panelState[PanelSet.Lobby] = true;
                panelState[PanelSet.RoomList] = true;
                ActivePanel();
            }

            public void OnActiveRoomListPanel()
            {
                SetFalse();
                panelState[PanelSet.Lobby] = true;
                panelState[PanelSet.RoomList] = true;
                ActivePanel();
            }

            public void OnActiveFindRoomPanel()
            {
                SetFalse();
                panelState[PanelSet.Lobby] = true;
                panelState[PanelSet.RoomList] = true;
                panelState[PanelSet.FindRoom] = true;
                ActivePanel();
            }
        #endregion
    }
}