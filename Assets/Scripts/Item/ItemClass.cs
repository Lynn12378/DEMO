using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace DEMO.Item
{
    public class ItemClass : NetworkBehaviour
    {
        public enum ItemType
        {
            Bullet,
            Coin,
            Food,
            Health,
            Wood,
        }

        public ItemType itemType;
        public int amount = 1;
        public Sprite sprite;

        public string GetName()
        {
            switch(itemType)
            {
                default:
                case ItemType.Bullet:       return "Bullet";
                case ItemType.Coin:         return "Coin";
                case ItemType.Food:         return "Food";
                case ItemType.Health:       return "Health";
                case ItemType.Wood:         return "Wood";
            }
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        #region - Pick Up Item -

        // Method to handle when the item is picked up
        public void OnPickUp()
        {
            DespawnItem_RPC();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void DespawnItem_RPC()
        {
            Runner.Despawn(Object);
		}

        #endregion

        /*public void RemoveFromInventory()
        {
            Inventory.instance.Remove(this);
        }*/

        
    }
}