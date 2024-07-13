using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Fusion;

using DEMO.Manager;
using DEMO.DB;
using DEMO.GamePlay;
using DEMO.Gameplay;
using TMPro.Examples;

namespace DEMO.GamePlay.Inventory
{
    public class Item : NetworkBehaviour
    {
        public enum ItemType
        {
            Bullet,
            Coin,
            Food,
            Health,
            Wood,
            Placeholder1,
            Placeholder2,
            Placeholder3,
            None,
        }

        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] public SpriteRenderer spriteRenderer;
        [Networked] public int itemID { get; set; }
        [Networked] public int amount { get; set; }
        public ItemType itemType;
        public int quantity;

        // Usage of item
        private int bulletAdd = 10;
        private int foodAdd = 20;
        private int boostHealth = 20;

        public override void Spawned()
        {
            var itemWorld = Runner.transform.Find("itemWorld");
            transform.SetParent(itemWorld.transform, false);

            Init(itemID);
            GamePlayManager.Instance.itemList.Add(this);
        }

        #region - Initialize Item - 
        public void Init(int itemID)
        {
            this.itemType = (ItemType) itemID;
            spriteResolver.SetCategoryAndLabel("item", this.itemType.ToString());
            quantity = 1;

            SetItemID_RPC(itemID);
            SetAmount_RPC(1);
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetItemID_RPC(int itemID)
        {
            this.itemID = itemID;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetAmount_RPC(int amount)
        {
            this.amount = amount;
		}

        #endregion 

        #region - Item Functions -
        // Pick up item
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void DespawnItem_RPC()
        {
            Runner.Despawn(Object);
		}

        // Use Item
        public void Use(PlayerNetworkData playerNetworkData)
        {
            bool validItem = true;

            switch (itemType)
            {
                default:
                case ItemType.Bullet:
                    playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + bulletAdd);
                    AudioManager.Instance.Play("Use");
                    break;
                case ItemType.Food:
                    playerNetworkData.SetPlayerFood_RPC(playerNetworkData.foodAmount + foodAdd);
                    AudioManager.Instance.Play("Eat");
                    break;
                case ItemType.Health:
                    playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP + boostHealth);
                    AudioManager.Instance.Play("Heal");
                    break;
                case ItemType.Wood:
                    if(playerNetworkData.shelter != null)
                    {
                        playerNetworkData.shelter.RepairDurability_RPC();
                        playerNetworkData.GetPlayerOutputData().repairQuantity++;
                        Debug.Log(playerNetworkData.playerRefString + " use wood for " + playerNetworkData.GetPlayerOutputData().repairQuantity + " times.");
                        AudioManager.Instance.Play("Use");
                    }
                    else
                    {
                        Debug.Log("Shelter not found!");
                        validItem = false;
                    }
                    break;
                case ItemType.Placeholder1:
                case ItemType.Placeholder2:
                case ItemType.Placeholder3:
                    ////////////////////// Warning message box
                    Debug.Log("Cannot use this item.");
                    validItem = false;
                    break;
            }

            if(validItem)
            {
                DecreaseQuantityOrRemove(playerNetworkData.itemList);
            }
        }

        // Discard Item 
        public void Discard(PlayerNetworkData playerNetworkData)
        {
            AudioManager.Instance.Play("Discard");
            DecreaseQuantityOrRemove(playerNetworkData.itemList);
        }

        // Gift Item
        public void Gift(PlayerNetworkData playerNetworkData, Item.ItemType itemType, PlayerRef targetPlayerRef)
        {
            foreach (PlayerNetworkData pnd in GamePlayManager.Instance.gamePlayerList.Values)
            {
                if (pnd.playerRef == targetPlayerRef)
                {
                    pnd.ReceiveGift_RPC(itemType);
                    break;
                }
            }

            AudioManager.Instance.Play("Gift");
            DecreaseQuantityOrRemove(playerNetworkData.itemList);
        }

        // Decrease quantity or remove item after use
        private void DecreaseQuantityOrRemove(List<Item> itemList)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].itemType == itemType)
                {
                    if (itemList[i].quantity > 1)
                    {
                        itemList[i].quantity--;
                    }
                    else
                    {
                        itemList.RemoveAt(i);
                    }
                    break;
                }
            }
        }
        #endregion
    }
}