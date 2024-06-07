using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Fusion;

using DEMO.Manager;
using DEMO.DB;
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
        }

        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] public SpriteRenderer spriteRenderer;
        [Networked] public int itemID { get; set; }
        [Networked] public int amount { get; set; }
        public ItemType itemType;
        public int quantity;

        // Usage of item
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

        /*public void SetType()
        {
            this.itemType = (ItemType) itemID;
            spriteResolver.SetCategoryAndLabel("item", this.itemType.ToString());
        }*/

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

        #region - Pick Up Item -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void DespawnItem_RPC()
        {
            Runner.Despawn(Object);
		}
        #endregion

        #region - Use Item - 
        public void Use(PlayerNetworkData playerNetworkData)
        {
            Debug.Log("Item in use: " + this.itemType);

            switch (itemType)
            {
                default:
                case ItemType.Bullet:
                    playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + 1);
                    break;
                case ItemType.Coin:
                    // Add coin
                    break;
                case ItemType.Food:
                    // Add food
                    break;
                case ItemType.Health:
                    playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP + boostHealth);
                    break;
                case ItemType.Wood:
                    // Add durability
                    break;
            }

            foreach (Item item in playerNetworkData.itemList)
            {
                if (item.itemType == this.itemType)
                {
                    if (item.quantity > 1)
                    {
                        item.quantity--;
                    }
                    else
                    {
                        playerNetworkData.itemList.Remove(item);
                    }
                    break;
                }
            }

            playerNetworkData.UpdateItemList();
            Debug.Log(playerNetworkData.ShowList());
        }
        #endregion
    }
}