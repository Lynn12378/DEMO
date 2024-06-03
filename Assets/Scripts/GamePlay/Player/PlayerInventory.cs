using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DEMO.Inventory;
using DEMO.Item;
using Fusion;


namespace DEMO.GamePlay.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private InventoryClass inventory;

        private void Start()
        {
            inventory = new InventoryClass();
            Debug.Log("Inventory created.");
        }

        public void PickUpItem(ItemClass item)
        {
            inventory.AddItem(item);
            item.OnPickUp();
        }

        public string GetInventoryItemsAsString()
        {
            string inventoryItemsString = "Inventory items:  ";
            foreach (var item in inventory.GetItems())
            {
                inventoryItemsString += item.GetName() + " x " + item.amount + ", ";
            }
            return inventoryItemsString;
        }
    }
}
