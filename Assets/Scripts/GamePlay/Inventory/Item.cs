using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Fusion;

using DEMO.Manager;
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
    }
}