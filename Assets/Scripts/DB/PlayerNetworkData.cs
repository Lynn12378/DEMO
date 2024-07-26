using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

using DEMO.GamePlay.Inventory;
using DEMO.Gameplay;
using DEMO.UI;

namespace DEMO.DB
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        private ChangeDetector changes;
        public Slider hpSlider;
        public GameObject minimapIcon;
        [SerializeField] private PlayerOutputData playerOutputData;
        private GamePlayManager gamePlayManager = null;
        public UIManager uIManager = null;

        [SerializeField] public PlayerOutfitsHandler playerOutfitsHandler = null;

        [Networked] public int playerId { get; private set; }
        [Networked] public PlayerRef playerRef { get; private set; }
        [Networked] public string playerRefString { get; private set; }
        [Networked] public string playerName { get; private set; }
        [Networked] public int HP { get; set; }
        [Networked] public int foodAmount { get; set; }
        [Networked] public int bulletAmount { get; set; }
        [Networked] public int coinAmount { get; set; }
        [Networked] public int teamID { get; private set; }
        [Networked][Capacity(2)] public NetworkArray<Color> colorList => default;
        [Networked][Capacity(10)] public NetworkArray<string> outfits => default;
        
        public int MaxHP = 100;
        public int MaxFood = 100;
        public int MaxBullet = 50;
        private TickTimer foodDecreaseTimer;

        public List<Item> itemList = new List<Item>();
        public Shelter shelter; // Reference when in shelter
        Color localColor;

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(Runner.transform);

            gamePlayManager = GamePlayManager.Instance;
            gamePlayManager.gamePlayerList.Add(Object.InputAuthority, this); 
  
            if (Object.HasStateAuthority)
            {
                SetPlayerRef_RPC();
                SetPlayerHP_RPC(MaxHP);
                SetPlayerBullet_RPC(MaxBullet);
                SetPlayerCoin_RPC(100);
                SetPlayerFood_RPC(MaxFood);
                SetPlayerTeamID_RPC(-1);
            }

            playerOutfitsHandler.Init();

            if(outfits.Get(0) != ""){UpdatedOutfits();}
            playerOutfitsHandler.SetSkinColor(colorList[0]);
            playerOutfitsHandler.SetHairColor(colorList[1]);

            // Set state for LocalPlayer
            if (Object.HasInputAuthority)
            {
                // Change color of color code, if failed then color = white
                localColor = ColorUtility.TryParseHtmlString("#00C800", out Color color) ? color : Color.white;
                hpSlider.fillRect.GetComponent<Image>().color = localColor;
                minimapIcon.GetComponent<SpriteRenderer>().color = localColor;

                uIManager.InitializeItemSlots(this);
            }
            else
            {
                minimapIcon.SetActive(false);
            }

            uIManager.UpdateMicTxt("none");
            uIManager.SetPlayerRef(playerRef);

            foodDecreaseTimer = TickTimer.CreateFromSeconds(Runner, 20);
		}

        public PlayerOutputData GetPlayerOutputData()
        {
            return playerOutputData;
        }

        public override void FixedUpdateNetwork()
        {
            if (foodDecreaseTimer.Expired(Runner))
            {
                SetPlayerFood_RPC(foodAmount - 1);
                foodDecreaseTimer = TickTimer.CreateFromSeconds(Runner, 20);
            }
        }

        #region - Restart -
        public void Restart()
        {
            SetPlayerHP_RPC(MaxHP);
            SetPlayerBullet_RPC(MaxBullet);
            SetPlayerFood_RPC(MaxFood);
            SetPlayerCoin_RPC(0);
            SetPlayerTeamID_RPC(-1);

            itemList.Clear();
            UpdateItemList();
        }
        #endregion

        #region - Setter -
        public void SetUIManager(UIManager uIManager)
        {
            this.uIManager = uIManager;
        }

        public void SetShelter(Shelter shelter)
        {
            this.shelter = shelter;
        }
        #endregion

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

        public void UpdatedOutfits()
        {
            var i = 0;
            foreach(var resolver in playerOutfitsHandler.resolverList)
            {
                playerOutfitsHandler.ChangeOutfit(resolver.GetCategory(),outfits[i]);
                i+=1;
            }
        }
        #endregion

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerRef_RPC()
        {
            playerRefString = Runner.LocalPlayer.ToString();
            playerRef = Runner.LocalPlayer;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC(int id, string name)
        {
            playerId = id;
			playerName = name;

            uIManager.UpdatePlayerName(playerName);
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void SetPlayerHP_RPC(int hp)
        {
            if(hp > HP && shelter == null)
            {
                playerOutputData.remainHP.Add(HP);
            }

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
            if(amount > bulletAmount)
            {
                playerOutputData.remainBullet.Add(bulletAmount);
            }

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

            UpdateItemList();
        }

        public void SetColorList(List<Color> colors)
        {
            colorList.Clear();
            colorList.Set(0, colors[0]);
            colorList.Set(1, colors[1]);
		}

        public void SetOutfits(List<string> outfits)
        {
            this.outfits.Clear();

            for(int i = 0; i < outfits.Count; i++)
            {
                this.outfits.Set(i, outfits[i]);
            }
		}

        #endregion

        #region - OnChanged Events -
        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(teamID):
                        gamePlayManager.UpdatedGamePlayer();
                        break;
                    case nameof(outfits):
                            UpdatedOutfits();
                            break;
                    case nameof(colorList):
                        playerOutfitsHandler.SetSkinColor(colorList[0]);
                        playerOutfitsHandler.SetHairColor(colorList[1]);
                        break;
                }

                if(!Object.HasStateAuthority){return;}if(!Object.HasStateAuthority){return;}
                switch (change)
                {
                    case nameof(teamID):
                        gamePlayManager.UpdatedGamePlayer();
                        break;
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
                }
            }
        }
        #endregion

        public void ShowList()
        {
            Debug.Log("Item List Contents:");
            foreach (var item in itemList)
            {
                Debug.Log($"Item Type: {item.itemType}, Item ID: {item.itemId}");
            }
        }
    }
}