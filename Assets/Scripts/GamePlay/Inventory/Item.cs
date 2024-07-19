using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Fusion;

using DEMO.Manager;
using DEMO.DB;
using DEMO.GamePlay.Interactable;

namespace DEMO.GamePlay.Inventory
{
    [System.Serializable]
    public class Item : NetworkBehaviour
    {
        public enum ItemType
        {
            Bullet,
            Coin,
            Food,
            Health,
            Wood,
            Badge_LiberalArts,      // 文院
            Badge_Science,          // 理院
            Badge_Engineer,         // 工院
            Badge_Management,       // 管院
            Badge_EECS,             // 電機資工
            Badge_Earth,            // 地科
            Badge_Hakka,            // 客院
            Badge_HST,              // 生醫理工
            Badge_TeachingCenters,  // 總教學中心
            None,
        }

        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] public SpriteRenderer spriteRenderer;
        [Networked] public int itemID { get; set; }
        [Networked] public int amount { get; set; }
        public ItemType itemType;
        public int itemId;
        public int quantity;

        // Usage of item
        private int bulletAdd = 10;
        private int foodAdd = 20;
        private int boostHealth = 20;

        public override void Spawned()
        {
            var itemWorld = GameObject.Find("itemWorld");
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
            itemId = itemID;

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
                        playerNetworkData.shelter.SetDurability_RPC(playerNetworkData.shelter.durability + 2);
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
                case ItemType.Badge_LiberalArts:
                case ItemType.Badge_Science:
                case ItemType.Badge_Engineer:
                case ItemType.Badge_Management:
                case ItemType.Badge_EECS:
                case ItemType.Badge_Earth:
                case ItemType.Badge_Hakka:
                case ItemType.Badge_HST:
                case ItemType.Badge_TeachingCenters:
                    if (IsPlayerInCorrectLocation(playerNetworkData, itemType, out Building building))
                    {
                        building.AddBadge_RPC();
                        playerNetworkData.GetPlayerOutputData().usePlaceholderNo++;
                        AudioManager.Instance.Play("BadgeUse"); ///////////////////////// Add sound
                    }
                    else
                    {
                        Debug.Log("Cannot use this item here.");
                        validItem = false;
                    }
                    break;
            }

            if(validItem)
            {
                DecreaseQuantityOrRemove(playerNetworkData.itemList);
            }
        }

        private bool IsPlayerInCorrectLocation(PlayerNetworkData playerNetworkData, ItemType itemType, out Building building)
        {
            building = null;

            Building[] buildings = FindObjectsOfType<Building>();

            foreach (Building b in buildings)
            {
                if (b.GetComponent<Collider2D>().OverlapPoint(playerNetworkData.minimapIcon.transform.position))
                {
                    if (itemType == b.badgeType)
                    {
                        building = b;
                        return true;
                    }
                }
            }

            return false;
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