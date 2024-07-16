using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using TMPro;

namespace DEMO.Manager
{
    public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private TMP_InputField roomName = null;
        [SerializeField] private string roomScene = null;
        [SerializeField] private int maxPlayer;

        private GameManager gameManager = null;
        private NetworkRunner runner = null;

        private void Start()
        {
            gameManager = GameManager.Instance;
            runner = gameManager.Runner;
            runner.AddCallbacks(this);
        }

        public void JoinRoom()
        {
            StartGame(GameMode.Shared, roomName.text, roomScene, false);
        }

        public void RandomRoom()
        {
            StartGame(GameMode.Shared, null, roomScene, true);
        }

        private async void StartGame(GameMode mode, string roomName, string sceneName, bool isRandom)
        {
            var startGameArgs = new StartGameArgs()
            {
                GameMode = mode,
                PlayerCount = maxPlayer,
                SessionName = roomName,
                Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(sceneName)),
                ObjectProvider = runner.GetComponent<NetworkObjectProviderDefault>(),
                MatchmakingMode = isRandom ? 0 : null,
            };

            await runner.StartGame(startGameArgs);

            if (runner.IsServer)
            {
                await runner.LoadScene(sceneName);
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
            if (player == runner.LocalPlayer)
            {
			    runner.Spawn(GameManager.playerInfo, Vector3.zero, Quaternion.identity, player);
            }
        }

        #region /-- Unused Function --/
            public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
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
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
            public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion
    }
}