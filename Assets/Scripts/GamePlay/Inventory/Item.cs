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
//using UnityEditor.U2D.Animation;

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
            switch (itemType)
            {
                default:
                case ItemType.Bullet:
                    playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + bulletAdd);
                    break;
                case ItemType.Food:
                    playerNetworkData.SetPlayerFood_RPC(playerNetworkData.foodAmount + foodAdd);
                    break;
                case ItemType.Health:
                    playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP + boostHealth);
                    break;
                case ItemType.Wood:
                    if(playerNetworkData.shelter != null)
                    {
                        playerNetworkData.shelter.RepairDurability_RPC();
                    }
                    else
                    {
                        Debug.Log("Shelter not found!");
                    }
                    break;
            }

            DecreaseQuantityOrRemove(playerNetworkData.itemList);
        }

        // Discard Item 
        public void Discard(PlayerNetworkData playerNetworkData)
        {
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