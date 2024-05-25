using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.DB;
using DEMO.UI;

namespace DEMO.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [SerializeField] private NetworkRunner runner = null;
        [SerializeField] private GameObject gameUIPrefab = null;
        private GameObject gameUIObject;
        private PlayerStatsUI playerStatsUI;
        private InventoryUI inventoryUI;

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

        public void ShowGameUI(PlayerRef playerRef)
        {
            NetworkObject playerObject = Runner.GetPlayerObject(playerRef);
            if (playerObject != null)
            {
                GameObject gameUIObject = Instantiate(gameUIPrefab);

                // Get references to PlayerStatsUI and InventoryUI
                playerStatsUI = gameUIObject.GetComponentInChildren<PlayerStatsUI>();
                inventoryUI = gameUIObject.GetComponentInChildren<InventoryUI>();

                // Subscribe to the OnHealthSliderUpdated event
                OnHealthSliderUpdated += playerStatsUI.UpdateHealthBar;
                OnBulletAmountUpdated += playerStatsUI.UpdateBulletAmount;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe events to prevent memory leaks
            if (playerStatsUI != null)
            {
                OnHealthSliderUpdated -= playerStatsUI.UpdateHealthBar;
                OnBulletAmountUpdated -= playerStatsUI.UpdateBulletAmount;
            }
        }


        #region - playerNetworkData -
            public Dictionary<PlayerRef, PlayerNetworkData> playerList = new Dictionary<PlayerRef, PlayerNetworkData>();
            
            public event Action<int> OnHealthSliderUpdated = null;
            public void UpdateHealthSlider(int health)
            {
                OnHealthSliderUpdated?.Invoke(health);
            }

            public event Action<int> OnBulletAmountUpdated = null;
            public void UpdateBulletAmount(int amount)
            {
                OnBulletAmountUpdated?.Invoke(amount);
            }

        #endregion
        
    }
}