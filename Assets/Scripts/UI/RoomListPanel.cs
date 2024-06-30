using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.Manager;

namespace DEMO.UI
{
    public class RoomListPanel : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager = null;
        [SerializeField] private RoomCell roomCellPrefab = null;
        [SerializeField] private Transform contentTrans = null;

        private List<RoomCell> roomCells = new List<RoomCell>();

        public void UpdateRoomList(List<SessionInfo> sessionList)
        {
            foreach(Transform child in contentTrans)
            {
                Destroy(child.gameObject);
            }

            roomCells.Clear();

            foreach(var session in sessionList)
            {
                var cell = Instantiate(roomCellPrefab, contentTrans);

                cell.SetInfo(lobbyManager, session.Name);
            }
        }
    }

}