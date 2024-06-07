using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using DEMO.DB;


namespace DEMO.GamePlay.Inventory
{
    public class ItemFunction : NetworkBehaviour
    {
        private InventorySlot currentSlot;              // Reference to the current InventorySlot
        private PlayerNetworkData playerNetworkData;    // Reference to LocalPlayer.PlayerNetworkData

        public void SetSlot(InventorySlot slot)
        {
            currentSlot = slot;

            var pND = Runner.GetComponentInChildren<PlayerNetworkData>();
            var pRef = pND.playerRef;
            if(pRef == Runner.LocalPlayer)
            {
                playerNetworkData = pND;
            }
        }

        private void HidePanel()
        {
            // Hide the panel
            gameObject.SetActive(false);
        }

        public void OnUseItem()
        {
            currentSlot.UseItem(playerNetworkData);
            HidePanel();
        }

        public void OnDiscardItem()
        {
            currentSlot.DiscardItem(playerNetworkData);
            HidePanel();
        }

        public void OnGiftItem()
        {
            currentSlot.GiftItem(playerNetworkData);
            HidePanel();
        }
    }
}
