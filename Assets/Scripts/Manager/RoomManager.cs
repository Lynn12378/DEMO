using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using TMPro;

using DEMO.DB;
using DEMO.UI;

namespace DEMO.Manager
{
    public class RoomManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]  private PlayerCell playerCellPrefab = null;
        [SerializeField] private Transform contentTrans = null;
		[SerializeField] private TMP_InputField roomName = null;
		[SerializeField] private string roomScene = null;
		[SerializeField] private string gameScene = null;
		private NetworkRunner networkInstance = null;
        private GameManager gameManager = null;

        private PlayerInfo playerInfo;
        private List<PlayerCell> playerCells = new List<PlayerCell>();

        private void Start()
        {
            gameManager = GameManager.Instance;
            networkInstance = gameManager.Runner;
            networkInstance.AddCallbacks(this);

            playerInfo = GameManager.playerInfo;
			roomName.text = networkInstance.SessionInfo.Name;

            GameManager.Instance.OnPlayerListUpdated += UpdatePlayerList;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnPlayerListUpdated -= UpdatePlayerList;
        }

        public void UpdatePlayerList()
        {
            var allReady = true;
            foreach(var cell in playerCells)
            {
                Destroy(cell.gameObject);
            }

            playerCells.Clear();

            foreach(var player in GameManager.Instance.playerList)
            {
                var cell = Instantiate(playerCellPrefab, contentTrans);
                var playerInfo = player.Value;

                cell.SetInfo(playerInfo.playerName, playerInfo.isReady);
                playerCells.Add(cell);

                if(!playerInfo.isReady)
                {
                    allReady = false;
                }
            }

            if(allReady)
            {
                StartGamePlay();
            }

        }

		public async void StartGamePlay()
        {
			if (networkInstance.IsSceneAuthority) {
				await networkInstance.UnloadScene(SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(roomScene)));
  				await networkInstance.LoadScene(SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(gameScene)), LoadSceneMode.Additive);
			}
        }

        public void OnReadyBtnClicked()
        {
            if (gameManager.playerList.TryGetValue(networkInstance.LocalPlayer, out PlayerInfo playerInfo))
            {
                playerInfo.SetIsReady_RPC();
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}

		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if (networkInstance.IsServer)
            {
                networkInstance.Despawn(playerInfo.Object);
                GameManager.Instance.UpdatePlayerList();
            }
		}

		#region /-- Unused Function --/
            public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
            public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnInput(NetworkRunner runner, NetworkInput input){}
            public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
            public void OnConnectedToServer(NetworkRunner runner){}
            public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}
            public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,byte[] token){}
            public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
            public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
            public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
            public void OnSceneLoadDone(NetworkRunner runner){}
            public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion
    }
}