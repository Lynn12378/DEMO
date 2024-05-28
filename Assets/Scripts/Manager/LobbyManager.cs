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
    public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] RoomListPanel roomListPanel = null;
        [SerializeField] private TMP_InputField roomName = null;
        [SerializeField] private string roomScene = null;

        private NetworkRunner networkInstance = null;
        private GameManager gameManager = null;

        private void Start()
        {
            gameManager = GameManager.Instance;
            networkInstance = gameManager.Runner;
            networkInstance.AddCallbacks(this);
        }

        // FindRoom
        public void StartShared()
        {
            StartGame(GameMode.Shared, roomName.text, roomScene);
        }

        // JoinRoom
        public void StartShared(string roomName)
        {
            StartGame(GameMode.Shared, roomName, roomScene);
        }

        private async void StartGame(GameMode mode, string roomName, string sceneName)
        {
            var startGameArgs = new StartGameArgs()
            {
                GameMode = mode,
                PlayerCount = 10,
                SessionName = roomName,
                Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(sceneName)),
                ObjectProvider = networkInstance.GetComponent<NetworkObjectProviderDefault>(),
            };

            await networkInstance.StartGame(startGameArgs);

            if (networkInstance.IsServer)
            {
                await networkInstance.LoadScene(sceneName);
            }
        }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            roomListPanel.UpdateRoomList(sessionList);
            Debug.Log(sessionList);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
            if (player == runner.LocalPlayer)
            {
			    networkInstance.Spawn(GameManager.playerInfo, Vector3.zero, Quaternion.identity, player);
            }
        }

		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if (networkInstance.IsServer)
            {
                networkInstance.Despawn(GameManager.playerInfo.Object);
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
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
            public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
            public void OnSceneLoadDone(NetworkRunner runner){}
            public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion
    }
}