using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

using Fusion;
using Fusion.Sockets;

using DEMO;
using DEMO.Player;
using FusionHelpers;

namespace DEMO.Lobby
{
    public enum PairState
    {
        Lobby,
        CreatingRoom,
        InRoom
    }

    public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        private GameManager gameManager = null;
        private PairState pairState = PairState.Lobby;

        [SerializeField] private PlayerNetworkData playerNetworkDataPrefab = null;

        [SerializeField] private RoomListPanel roomListPanel = null;
        [SerializeField] private CreateRoomPanel createRoomPanel = null;
        [SerializeField] private InRoomPanel inRoomPanel = null;
    
        public GameManager getGameManager()
        {
            return gameManager;
        }

        private async void Start()
        {
            SetPairState(PairState.Lobby);

            gameManager = GameManager.Instance;

            gameManager.Runner.AddCallbacks(this);


            await JoinLobby(gameManager.Runner);
        }
        public NetworkSceneInfo getScene()
        {
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid) {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            return scene;
        }

        #region - Create Lobby & Room -
        public async Task JoinLobby(NetworkRunner runner)
        {
            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

            if (!result.Ok)
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }

        public async Task CreateRoom(string roomName)
        {
            var result = await gameManager.Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = roomName,
                PlayerCount = 5,
                Scene = getScene(),
                SceneManager = gameManager.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                ObjectProvider = gameManager.gameObject.AddComponent<FusionObjectPoolRoot>()
            });

            if (result.Ok)
                SetPairState(PairState.InRoom);
            else
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }

        public async Task JoinRoom(string roomName)
        {
            var result = await gameManager.Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = roomName,
                Scene = getScene(),
                SceneManager = gameManager.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                ObjectProvider = gameManager.gameObject.AddComponent<FusionObjectPoolRoot>()
            });

            if(result.Ok)
                SetPairState(PairState.InRoom);
            else
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        #endregion

        #region - Set State -
        public void SetPairState(PairState newState)
        {
            pairState = newState;
            
            switch (pairState)
            {
                case PairState.Lobby:
                    SetPanel(roomListPanel);
                    break;
                case PairState.CreatingRoom:
                    SetPanel(createRoomPanel);
                    break;
                case PairState.InRoom:
                    SetPanel(inRoomPanel);
                    break;
            }
        }

        private void SetPanel(IPanel panel)
        {
            roomListPanel.DisplayPanel(false);
            createRoomPanel.DisplayPanel(false);
            inRoomPanel.DisplayPanel(false);

            panel.DisplayPanel(true);
        }
        #endregion

        #region - Used Callbacks -
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            roomListPanel.UpdateRoomList(sessionList);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            runner.Spawn(playerNetworkDataPrefab, Vector3.zero, Quaternion.identity, player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (gameManager.playerList.TryGetValue(player, out PlayerNetworkData networkPlayerData))
            {
                runner.Despawn(networkPlayerData.Object);

                gameManager.playerList.Remove(player);
                gameManager.UpdatePlayerList();
            }
        }
        
        #endregion

        #region - Unused callbacks -
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        #endregion
    }
}