using System;
<<<<<<< HEAD
using System.Collections.Generic;
=======
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
<<<<<<< HEAD
=======
using TMPro;
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9

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
<<<<<<< HEAD
        [SerializeField] private GameObject teamPlayerCellPrefab = null;
        [SerializeField] private Transform contentTrans = null;
        private GamePlayManager gamePlayManager = null;
		private NetworkRunner networkInstance = null;   

        #region - OnInGamePlayerUpdated -
=======
        [SerializeField] private Transform contentTrans = null;
        private GamePlayManager gamePlayManager = null;
		private NetworkRunner networkInstance = null;

>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9

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

<<<<<<< HEAD
=======
        #region - OnTeamUpdated -
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
        public void UpdatedTeamList()
        {
            foreach(var team in gamePlayManager.teamList)
            {
<<<<<<< HEAD
                if (team != null)
                {
                    team.transform.SetParent(contentTrans, false);
                    team.SetInfo(team.teamID);  
                }
            }
        }

        #endregion

        public void CreateTeam()
        {
            if (GamePlayManager.Instance.gamePlayerList.TryGetValue(networkInstance.LocalPlayer, out PlayerNetworkData playerNetworkData))
            {
                if (playerNetworkData.teamID == -1)
                {
                    var cell = networkInstance.Spawn(teamCellPrefab, Vector3.zero, Quaternion.identity);
                    TeamCell teamCell = cell.GetComponent<TeamCell>();

                    bool notEmpty = false;
                    int newTeamId = 1;

                    while (true)
                    {
                        notEmpty = false;
                        foreach (var pnd in GamePlayManager.Instance.gamePlayerList.Values)
                        {
                            if (pnd.teamID == newTeamId)
                            {
                                notEmpty = true;
                                break;
                            }
                        }
                        if (!notEmpty)
                        {
                            break;
                        }
                        newTeamId++;
                    }

                    GamePlayManager.Instance.newTeamID = newTeamId;
                    teamCell.SetPlayerTeamID_RPC(gamePlayManager.newTeamID);            
                    playerNetworkData.SetPlayerTeamID_RPC(gamePlayManager.newTeamID);
                    teamCell.getTeamBtnTxt().text = "quit";
                    GamePlayManager.Instance.UpdatedTeamList();
                } 
                else
                {
                    Debug.Log("Player already in another team!");                    
                }
            }
        }

        public void toggleExpand(TeamCell currentTeamCell)
        {
            int index = 0; 
            
            if (!currentTeamCell.isExpanded)
            {
                foreach (var team in GamePlayManager.Instance.teamList)
                {
                    if (team != null && team == currentTeamCell)
                    {
                        index = team.GetComponent<RectTransform>().GetSiblingIndex();
                    } 
                }

                foreach (var pnd in GamePlayManager.Instance.gamePlayerList.Values)
                {
                    if(pnd.teamID == currentTeamCell.teamID)
                    {
                        var teamPlayerCell = networkInstance.Spawn(teamPlayerCellPrefab, Vector3.zero, Quaternion.identity);
                        teamPlayerCell.GetComponent<TeamPlayerCell>().getTeamPlayerCellTxt().text = pnd.playerName;
                        teamPlayerCell.transform.SetParent(contentTrans, false);
                        teamPlayerCell.transform.SetSiblingIndex(index + 1);

                        index ++;
                    }
                }
                currentTeamCell.isExpanded = true;
            }
            else if (currentTeamCell.isExpanded)
            {
                TeamPlayerCell[] tpcs = FindObjectsOfType<TeamPlayerCell>();
                foreach(TeamPlayerCell tpc in tpcs)
                {
                    Destroy(tpc.gameObject);
                }

                currentTeamCell.isExpanded = false;
            }
                        
        }

=======
                team.transform.SetParent(contentTrans, false);
                team.SetInfo(team.teamID);
            }
        }

        public void CreateTeam()
        {
            var cell = networkInstance.Spawn(teamCellPrefab, Vector3.zero, Quaternion.identity);
            cell.GetComponent<TeamCell>().SetPlayerTeamID_RPC(gamePlayManager.newTeamID);
        }
        #endregion

        #region - Start Game -
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (player == runner.LocalPlayer)
            {
			    var playerObject = networkInstance.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
                runner.SetPlayerObject(player, playerObject);

                Camera.main.transform.SetParent(playerObject.transform);
            }
        }

<<<<<<< HEAD
        #region - start game -
        public void StartShared()
        {
            StartGame(GameMode.Shared, "test123", gameScene);
=======
        public void StartShared()
        {
            StartGame(GameMode.Shared, "test111", gameScene);
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
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