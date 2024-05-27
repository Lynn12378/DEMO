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
        [SerializeField] private NetworkRunner runner;
        [SerializeField] private GameObject gameUIPrefab = null;
        private Dictionary<PlayerRef, PlayerUIComponents> playerGameUIs = new Dictionary<PlayerRef, PlayerUIComponents>();
    

        private void Awake()
        {
            runner.ProvideInput = true;

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
            playerGameUIs[playerRef] = new PlayerUIComponents(playerGameUI, playerStatsUI/*, inventoryUI*/);
        }


        #region - playerNetworkData -

        // Direct update for specific player's UI component
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

            public PlayerUIComponents(GameObject playerGameUI, PlayerStatsUI playerStatsUI/*, InventoryUI inventoryUI*/)
            {
                PlayerGameUI = playerGameUI;
                PlayerStatsUI = playerStatsUI;
                //InventoryUI = inventoryUI;
            }
        }
    }
}