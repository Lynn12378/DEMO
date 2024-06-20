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
using DEMO.GamePlay.Inventory;
using DEMO.GamePlay.EnemyScript;

namespace DEMO.Manager
{
    public class InGameManager : MonoBehaviour, INetworkRunnerCallbacks
    {
		[SerializeField] private string gameScene = null;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject teamCellPrefab = null;
        [SerializeField] private Transform contentTrans = null;
        private GamePlayManager gamePlayManager = null;
		private NetworkRunner networkInstance = null;

        #region - OnInGamePlayerUpdated -

        private void Start()
        {
            gamePlayManager = GamePlayManager.Instance;
            networkInstance = gamePlayManager.Runner;
            networkInstance.AddCallbacks(this);

            StartShared();

            gamePlayManager.OnTeamListUpdated += UpdatedTeamList;
        }

        private void OnDestroy()
        {
            gamePlayManager.OnTeamListUpdated -= UpdatedTeamList;
        }

        public void UpdatedTeamList()
        {
            foreach(var team in gamePlayManager.teamList)
            {
                team.transform.SetParent(contentTrans, false);
                team.SetInfo(team.teamID);
            }
        }

        #endregion

        public void CreateTeam()
        {
            var cell = networkInstance.Spawn(teamCellPrefab, Vector3.zero, Quaternion.identity);
            cell.GetComponent<TeamCell>().SetPlayerTeamID_RPC(gamePlayManager.newTeamID);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (player == runner.LocalPlayer)
            {
			    var playerObject = networkInstance.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
                runner.SetPlayerObject(player, playerObject);

                Camera.main.transform.SetParent(playerObject.transform);
            }
        }

        #region - start game -
        public void StartShared()
        {
            StartGame(GameMode.Shared, "test123", gameScene);
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

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            var itemSpawner = FindObjectOfType<ItemSpawner>();
            itemSpawner.StartItemSpawner();

            var enemySpawner = FindObjectOfType<EnemySpawner>();
            enemySpawner.StartEnemySpawner();
        }

        #endregion
       
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
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
            public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
            public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion
    }
}