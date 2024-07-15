using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using TMPro;

using DEMO.Manager;
using DEMO.UI;

namespace DEMO.Manager
{
    public class InGameManager1 : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private GameObject playerPrefab;
        private GameManager gameManager = null;
		private NetworkRunner runner = null;

        [SerializeField] private Transform contentTrans = null;
        [SerializeField] private TMP_Text messageTxt = null;
        [SerializeField] private GameObject messageCellPrefab = null;

        public void UpdatedMessages()
        {
            foreach(var message in gameManager.messages)
            {
                message.transform.SetParent(contentTrans, false);
            }
        }

        public void CreateMessage()
        {
            var cell = runner.Spawn(messageCellPrefab, Vector3.zero, Quaternion.identity);
            cell.GetComponent<MessageCell>().SetMessage_RPC(runner.LocalPlayer.ToString(), messageTxt.text);
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            runner = gameManager.Runner;
            runner.AddCallbacks(this);
            
            gameManager.OnMessagesUpdated += UpdatedMessages;
        }

        private void OnDestroy()
        {
            gameManager.OnMessagesUpdated -= UpdatedMessages;
        }

        private void Init(PlayerRef player)
        {
            var PIF = gameManager.playerList[player];
            var PND = gameManager.gamePlayerList[player];
            var i = 0;

            PND.SetPlayerInfo_RPC(PIF.playerId, PIF.playerName);
            PND.SetColorList(PIF.colorList);
            PND.SetOutfits(PIF.outfits);

            PIF.Despawned();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            var player = runner.LocalPlayer;
            var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            
            runner.SetPlayerObject(player, playerObject);
            Camera.main.transform.SetParent(playerObject.transform);
            Init(player);
        }

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