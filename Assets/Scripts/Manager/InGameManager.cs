using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;

using DEMO.DB;
using DEMO.UI;
using DEMO.GamePlay;

namespace DEMO.Manager
{
    public class InGameManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject teamCellPrefab = null;
        [SerializeField] private GameObject teamPlayerCellPrefab = null;
        [SerializeField] private Transform contentTrans = null;
        [SerializeField] private Transform messageContentTrans = null;

        private GameManager gameManager = null;
        private GamePlayManager gamePlayManager = null;
		private NetworkRunner networkInstance = null;
        private PlayerRef localPlayer;

        [SerializeField] private TMP_Text messageTxt = null;
        [SerializeField] private GameObject messageCellPrefab = null;


        private void Start()
        {
            gameManager = GameManager.Instance;
            gamePlayManager = GamePlayManager.Instance;
            networkInstance = gameManager.Runner;
            networkInstance.AddCallbacks(this);

            gamePlayManager.OnTeamListUpdated += UpdatedTeamList;
            gamePlayManager.OnInGamePlayerUpdated += UpdatedPlayerMinimap;
            gameManager.OnMessagesUpdated += UpdatedMessages;
        }

        private void OnDestroy()
        {
            gamePlayManager.OnTeamListUpdated -= UpdatedTeamList;
            gamePlayManager.OnInGamePlayerUpdated -= UpdatedPlayerMinimap;
            gameManager.OnMessagesUpdated += UpdatedMessages;
        }

        #region - Messages - 
        public void CreateMessage()
        {
            var cell = networkInstance.Spawn(messageCellPrefab, Vector3.zero, Quaternion.identity);
            cell.GetComponent<MessageCell>().SetMessage_RPC(networkInstance.LocalPlayer.ToString(), messageTxt.text);

            foreach(var player in gamePlayManager.playerOutputList)
            {
                if(player.Key == networkInstance.LocalPlayer) player.Value.sendMessageNo++;
            }

            // Reset message text
            messageTxt.text = "";
        }

        public void UpdatedMessages()
        {
            foreach(var message in gameManager.messages)
            {
                message.transform.SetParent(messageContentTrans, false);
            }
        }
        #endregion

        #region - Minimap -
        public void UpdatedPlayerMinimap()
        {
            var localPND = gamePlayManager.gamePlayerList[localPlayer];
            foreach (var gamePlayer in gamePlayManager.gamePlayerList.Values)
            {
                if(gamePlayer == localPND || localPND.teamID == -1) continue;

                if (gamePlayer.teamID == localPND.teamID)
                {
                    gamePlayer.minimapIcon.SetActive(true);
                }
                else
                {
                    gamePlayer.minimapIcon.SetActive(false);
                }
            }
        }
        #endregion

        #region - OnTeamUpdated -
        public void UpdatedTeamList()
        {
            foreach(var team in gamePlayManager.teamList)
            {
                team.transform.SetParent(contentTrans, false);
                team.SetInfo(team.teamID);
            }
        }

        public void CreateTeam()
        {
            if (gamePlayManager.gamePlayerList.TryGetValue(networkInstance.LocalPlayer, out PlayerNetworkData playerNetworkData))
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
                        foreach (var pnd in gamePlayManager.gamePlayerList.Values)
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

                    if (gamePlayManager.playerOutputList.TryGetValue(networkInstance.LocalPlayer, out PlayerOutputData playerOutputData))
                    {
                        playerOutputData.createTeamNo++;
                    }

                    gamePlayManager.newTeamID = newTeamId;
                    teamCell.SetPlayerTeamID_RPC(gamePlayManager.newTeamID);            
                    playerNetworkData.SetPlayerTeamID_RPC(gamePlayManager.newTeamID);
                    teamCell.getTeamBtnTxt().text = "退出";
                    gamePlayManager.UpdatedTeamList();
                } 
                else
                {
                    gamePlayManager.ShowWarningBox("你已經在另一個隊伍了。");                 
                }
            }
        }

        public void toggleExpand(TeamCell currentTeamCell)
        {
            int index = 0; 
            
            if (!currentTeamCell.isExpanded)
            {
                foreach (var team in gamePlayManager.teamList)
                {
                    if (team != null && team == currentTeamCell)
                    {
                        index = team.GetComponent<RectTransform>().GetSiblingIndex();
                    } 
                }

                foreach (var pnd in gamePlayManager.gamePlayerList.Values)
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
        #endregion

        #region - Start Game -
        private void Init(PlayerRef player)
        {
            var PIF = gameManager.playerList[player];
            var PND = gamePlayManager.gamePlayerList[player];
            var POD = gamePlayManager.playerOutputList[player];

            PND.SetPlayerInfo_RPC(PIF.playerId, PIF.playerName);
            PND.SetColorList(PIF.colorList);
            PND.SetOutfits(PIF.outfits);
            POD.SetPlayerId_RPC(PIF.playerId);

            PIF.Despawned();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            var player = runner.LocalPlayer;
            var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            
            runner.SetPlayerObject(player, playerObject);
            Camera.main.transform.SetParent(playerObject.transform);
            Init(player);

            localPlayer = runner.LocalPlayer;

            var spawner = FindObjectOfType<Spawner>();
            spawner.StartSpawners();
        }

        #endregion
       
		#region /-- Unused Function --/
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
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