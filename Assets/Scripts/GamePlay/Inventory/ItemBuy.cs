using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.DB;

namespace DEMO.GamePlay.Inventory
{
    public class ItemBuy : NetworkBehaviour
    {
        private ItemSlot currentSlot;              // Reference to the current InventorySlot
        private PlayerNetworkData playerNetworkData;    // Reference to LocalPlayer.PlayerNetworkData

        public void SetPLD(ItemSlot slot)
        {
            currentSlot = slot;

            var pND = Runner.GetComponentInChildren<PlayerNetworkData>();
            var pRef = pND.playerRef;
            if(pRef == Runner.LocalPlayer)
            {
                playerNetworkData = pND;
            }
        }

        public void OnUseItem()
        {
            //currentSlot.UseItem(playerNetworkData);
        }
    }
}
