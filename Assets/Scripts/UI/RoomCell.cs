using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DEMO.Manager;

namespace DEMO.UI
{
    public class RoomCell : MonoBehaviour
    {
        private string roomName = null;

        private LobbyManager lobbyManager = null;

        [SerializeField] private TMP_Text roomNameTxt = null;
        //[SerializeField] private Button joinBtn = null;

        public void SetInfo(LobbyManager lobbyManager, string roomName)
        {
            this.lobbyManager = lobbyManager;
            roomNameTxt.text = roomName;
        }

        public void OnJoinBtnClicked()
        {
            lobbyManager.StartShared(roomName);
        }
    }
}

