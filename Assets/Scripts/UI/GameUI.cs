using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.DB;

namespace DEMO.UI
{
    public class GameUI : NetworkBehaviour
    {
        private NetworkObject _playerNetworkObject;
        [SerializeField] private PlayerStatsUI playerStatsUI = null;
        [SerializeField] private InventoryUI inventoryUI = null;

        // Initialize canvas and tie to player
        public void Initialize(NetworkObject playerNetworkObject)
        {
            _playerNetworkObject = playerNetworkObject;
            PlayerNetworkData playerNetworkData = _playerNetworkObject.GetComponent<PlayerNetworkData>();
            //playerStatsUI.Initialize(playerNetworkData);
            //inventoryUI.Initialize(playerNetworkObject);
        }
    }
}
