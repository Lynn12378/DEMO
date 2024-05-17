using System.Collections.Generic;
using UnityEngine;
using System;

using Fusion;

using DEMO.Lobby;
using DEMO.Player;

namespace DEMO
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private NetworkRunner runner = null;
        [SerializeField] private NetworkPrefabRef playerPrefab;

        public NetworkRunner Runner
        {
            get
            {
                if (runner == null)
                {
                    runner = gameObject.AddComponent<NetworkRunner>();
                    runner.ProvideInput = true;
                }
                return runner;
            }
        }
    
        public string PlayerName = null;
        public Dictionary<PlayerRef, PlayerNetworkData> playerList = new Dictionary<PlayerRef, PlayerNetworkData>();

        public event Action OnPlayerListUpdated = null;
        private void Awake()
        {
            Runner.ProvideInput = true;

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private bool CheckAllPlayerIsReady()
        {
            if (!Runner.IsServer) return false;

            foreach (var playerData in playerList.Values)
            {
                if (!playerData.IsReady) return false;
            }

            foreach (var playerData in playerList.Values)
            {
                playerData.IsReady = false;
            }

            return true;
        }
    
        public void UpdatePlayerList()
        {
            OnPlayerListUpdated?.Invoke();

            if (CheckAllPlayerIsReady())
            {
                Runner.LoadScene ("GamePlay");
            }
        }

        public void SetPlayerNetworkData()
        {
            if (playerList.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
            {
                playerNetworkData.SetPlayerName_RPC(PlayerName);
            }
        }
    }
}