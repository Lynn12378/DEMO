using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DEMO.GamePlay.Inventory
{
    // Currently using PlayerNetworkData.itemList
	/*public class InventoryClass
	{
		private List<Item> items;
        [SerializeField] private int capacity = 10;

        public InventoryClass()
        {
            items = new List<Item>();
        }

        public void Add(Item item)
        {
            // Check if out of space
			if (items.Count >= capacity)
			{
				Debug.Log("Not enough room.");
				return;
			}

            items.Add(item);
        }

        public void Remove(Item item)
        {
            items.Remove(item);
        }

        public void OrganizeInventory()
        {
            // Create dictionary to store item and amount in stack
            Dictionary<Item.ItemType, int> stackedItems = new Dictionary<Item.ItemType, int>();

            foreach (Item item in items)
            {
                if (item != null)
                {
                    // If item in dict, add amount
                    if (stackedItems.ContainsKey(item.itemType))
                    {
                        stackedItems[item.itemType] += item.amount;
                    }
                    else
                    {
                        // else, add item in dict with its amount
                        stackedItems.Add(item.itemType, item.amount);
                    }
                }
            }

            // clear items list
            items.Clear();

            // add stackedItems into items
            foreach (KeyValuePair<Item.ItemType, int> kvp in stackedItems)
            {
                Item.ItemType itemType = kvp.Key;
                int amount = kvp.Value;

                // Create new item
                Item stackedItem = new Item();
                stackedItem.itemType = itemType;
                stackedItem.amount = amount;

                // Add into items
                items.Add(stackedItem);
            }
        }
    }*/
}