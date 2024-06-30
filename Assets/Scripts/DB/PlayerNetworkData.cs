using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

using DEMO.Manager;
using DEMO.GamePlay;
using DEMO.GamePlay.Inventory;
using Photon.Voice.Fusion;
using DEMO.Gameplay;

namespace DEMO.DB
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        [SerializeField] public Slider hpSlider;
        [SerializeField] public GameObject minimapIcon;
        [SerializeField] public VoiceNetworkObject voiceObject;
        private GamePlayManager gamePlayManager = null;
        private ChangeDetector changes;
        public UIManager uIManager = null;

        [Networked] public int playerId { get; private set; }
        [Networked] public PlayerRef playerRef { get; private set; }
        [Networked] public string playerRefString { get; private set; }
        [Networked] public string playerName { get; private set; }
        [Networked] public int HP { get; set; }
        [Networked] public int foodAmount { get; set; }
        [Networked] public int bulletAmount { get; set; }
        [Networked] public int coinAmount { get; set; }
        [Networked] public int teamID { get; set; }
        
        public int MaxHP = 100;
        public int MaxFood = 100;
        public int MaxBullet = 50;
        private float foodDecreaseInterval = 30f;
        private float foodDecreaseTimer = 0f;

        public List<Item> itemList = new List<Item>();
        public Shelter shelter; // Reference when in shelter

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(Runner.transform);

            gamePlayManager = FindObjectOfType<GamePlayManager>();
            gamePlayManager.gamePlayerList.Add(Object.InputAuthority, this);       
  
            if (Object.HasStateAuthority)
            {
                //Random player name for testing
                SetPlayerInfo_RPC(0,"Player" + new System.Random().Next(1, 10).ToString());
                SetPlayerHP_RPC(MaxHP);
                SetPlayerBullet_RPC(MaxBullet);
                SetPlayerCoin_RPC(100);
                SetPlayerFood_RPC(MaxFood);
                SetPlayerTeamID_RPC(-1);
            }

            // Set state for LocalPlayer
            if (playerRef == Runner.LocalPlayer)
            {
                // Change color of color code, if failed then color = white
                Color fillColor = ColorUtility.TryParseHtmlString("#00C800", out Color color) ? color : Color.white;
                hpSlider.fillRect.GetComponent<Image>().color = fillColor;
                minimapIcon.GetComponent<SpriteRenderer>().color = fillColor;

                uIManager.InitializeItemSlots(this);
            }

            uIManager.UpdateMicTxt("none");
            
            gamePlayManager.UpdatedGamePlayer();
		}

        private void Update()
        {
            foodDecreaseTimer += Time.deltaTime;

            if (foodDecreaseTimer >= foodDecreaseInterval)
            {
                // Reset timer
                foodDecreaseTimer = 0f;

                // Decrease food
                SetPlayerFood_RPC(foodAmount - 1);
            }
        }

        public void SetUIManager(UIManager uIManager)
        {
            this.uIManager = uIManager;
        }

        public void SetShelter(Shelter shelter)
        {
            this.shelter = shelter;
        }

        #region - Update UI -
        // Small slider above
        public void UpdateHPSlider(int health)
        {
            hpSlider.value = health;
        }

        public void UpdateItemList()
        {
            uIManager.SetItemList(itemList);
            uIManager.UpdateInventoryUI(itemList);
        }
        #endregion

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC(int id, string name)
        {
            playerId = id;
			playerName = name;
            playerRefString = Runner.LocalPlayer.ToString();
            playerRef = Runner.LocalPlayer;
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void SetPlayerHP_RPC(int hp)
        {
            if(hp >= MaxHP)
            {
                HP = MaxHP;
            }
            else
            {
                HP = hp;
            }

            UpdateHPSlider(HP);
		}
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerBullet_RPC(int amount)
        {
            if(amount >= MaxBullet)
            {
                bulletAmount = MaxBullet;
            }
            else
            {
                bulletAmount = amount;
            }
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerCoin_RPC(int amount)
        {
            coinAmount = amount;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerFood_RPC(int amount)
        {
            if(amount >= MaxFood)
            {
                foodAmount = MaxFood;
            }
            else
            {
                foodAmount = amount;
            }
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerTeamID_RPC(int id)
        {
            teamID = id;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void ReceiveGift_RPC(Item.ItemType itemType)
        {
            Item item = new Item
            {
                itemType = itemType,
                quantity = 1
            };
            itemList.Add(item);

            SetPlayerUI_RPC();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerUI_RPC()
        {
            UpdateItemList();
		}

        #endregion

        #region - OnChanged Events -

        public override void Render()
        {
            if(!Object.HasStateAuthority){return;}
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(HP):
                        uIManager.UpdateHPSlider(HP, MaxHP);
                        break;

                    case nameof(bulletAmount):
                        uIManager.UpdateBulletAmountTxt(bulletAmount, MaxBullet);
                        break;

                    case nameof(coinAmount):
                        uIManager.UpdateCoinAmountTxt(coinAmount);
                        break;

                    case nameof(foodAmount):
                        uIManager.UpdateFoodSlider(foodAmount, MaxFood);
                        break;

                    case nameof(teamID):
                        //call UIManager change Team
                        break;
                }
            }
        }
        #endregion

        // Test for debug
        public string ShowList()
        {
            string result = "Inventory: ";

            for(int i=0; i < itemList.Count; i++)
            {
                result += "ItemType: " + itemList[i].itemType + "; Quantity: " + itemList[i].quantity;
            }

            return result;
        }
    }
}