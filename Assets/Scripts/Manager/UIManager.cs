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
        private Dictionary<PlayerRef, PlayerUIComponents> playerGameUIs = new Dictionary<PlayerRef, PlayerUIComponents>();
        

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

        public void InitializeGameUI(PlayerRef playerRef)
        {
            GameObject playerGameUI = Instantiate(gameUIPrefab);

            // Get references to PlayerStatsUI and InventoryUI
            PlayerStatsUI playerStatsUI = playerGameUI.GetComponentInChildren<PlayerStatsUI>();
            //InventoryUI inventoryUI = playerGameUI.GetComponentInChildren<InventoryUI>();

            // Store the references in a dictionary for each player
            playerGameUIs[playerRef] = new PlayerUIComponents(playerGameUI, playerStatsUI, inventoryUI);

            /*// Method 1: Subscribe to event
            // Subscribe to the OnHealthSliderUpdated and OnBulletAmountUpdated events
            OnHealthSliderUpdated += (PlayerRef refPlayer, int health) => 
            {
                if (refPlayer == playerRef)
                {
                    playerStatsUI.UpdateHealthBar(health);
                }
            };

            OnBulletAmountUpdated += (PlayerRef refPlayer, int amount) => 
            {
                if (refPlayer == playerRef)
                {
                    playerStatsUI.UpdateBulletAmount(amount);
                }
            };*/
        }

        /*// Method 1
        private void OnDestroy()
        {
            // Unsubscribe events to prevent memory leaks
            foreach (var entry in playerGameUIs)
            {
                var playerRef = entry.Key;
                var uiComponents = entry.Value;
                if (uiComponents.PlayerStatsUI != null)
                {
                    OnHealthSliderUpdated -= (PlayerRef refPlayer, int health) => 
                    {
                        if (refPlayer == playerRef)
                        {
                            uiComponents.PlayerStatsUI.UpdateHealthBar(health);
                        }
                    };

                    OnBulletAmountUpdated -= (PlayerRef refPlayer, int amount) => 
                    {
                        if (refPlayer == playerRef)
                        {
                            uiComponents.PlayerStatsUI.UpdateBulletAmount(amount);
                        }
                    };
                }
            }
        }*/


        #region - playerNetworkData -
            
        /*// Method 1
        public event Action<PlayerRef, int> OnHealthSliderUpdated = null;
        public void UpdateHealthSlider(PlayerRef playerRef, int health)
        {
            OnHealthSliderUpdated?.Invoke(playerRef, health);
        }

        public event Action<PlayerRef, int> OnBulletAmountUpdated = null;
        public void UpdateBulletAmount(PlayerRef playerRef, int amount)
        {
            OnBulletAmountUpdated?.Invoke(playerRef, amount);
        }*/

        // Method 2: Direct update for specific player's UI component
        public void UpdateHealthSlider(PlayerRef playerRef, int health)
        {
            if (playerGameUIs.TryGetValue(playerRef, out PlayerUIComponents uiComponents))
            {
                uiComponents.PlayerStatsUI.UpdateHealthBar(health);
            }
        }

        public void UpdateBulletAmount(PlayerRef playerRef, int amount)
        {
            if (playerGameUIs.TryGetValue(playerRef, out PlayerUIComponents uiComponents))
            {
                uiComponents.PlayerStatsUI.UpdateBulletAmount(amount);
            }
        }

        #endregion
        
        private class PlayerUIComponents
        {
            public GameObject PlayerGameUI { get; }
            public PlayerStatsUI PlayerStatsUI { get; }
            public InventoryUI InventoryUI { get; }

            public PlayerUIComponents(GameObject playerGameUI, PlayerStatsUI playerStatsUI, InventoryUI inventoryUI)
            {
                PlayerGameUI = playerGameUI;
                PlayerStatsUI = playerStatsUI;
                InventoryUI = inventoryUI;
            }
        }
    }
}