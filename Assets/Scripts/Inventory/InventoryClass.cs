using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DEMO.Item;

namespace DEMO.Inventory
{
	public class InventoryClass
	{
		private List<ItemClass> items;

        public InventoryClass()
        {
            items = new List<ItemClass>();
        }

        public void AddItem(ItemClass item)
        {
            items.Add(item);
        }

        public void RemoveItem(ItemClass item)
        {
            items.Remove(item);
        }

        public List<ItemClass> GetItems()
        {
            return items;
        }


		/*// Callback which is triggered when an item gets added or removed
		public delegate void OnItemChanged();
		public OnItemChanged onItemChangedCallback;

		public int capacity = 12;	// Amount of slots in inventory

		// Current list of items in inventory
		public List<ItemClass> items = new List<ItemClass>();

		// Add a new item. If there is enough room return true. Else we return false.
		public bool Add (ItemClass item)
		{
			// Don't do anything if it's a default item
			//if (!item.isDefaultItem)
			//{
				// Check if out of space
				if (items.Count >= capacity)
				{
					Debug.Log("Not enough room.");
					return false;
				}

				items.Add(item);   // Add item to list

				// Trigger callback
				if (onItemChangedCallback != null)
					onItemChangedCallback.Invoke();
			//}

			return true;
		}

		// Remove an item
		public void Remove (ItemClass item)
		{
			items.Remove(item);

			// Trigger callback
			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke();
		}

		public void OrganizeInventory()
		{
			// Create dictionary to store item and amount in stack
			Dictionary<ItemClass.ItemType, int> stackedItems = new Dictionary<ItemClass.ItemType, int>();

			foreach (ItemClass item in items)
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
			foreach (KeyValuePair<ItemClass.ItemType, int> kvp in stackedItems)
			{
				ItemClass.ItemType itemType = kvp.Key;
				int amount = kvp.Value;

				// Create new item
				ItemClass stackedItem = new ItemClass();
				stackedItem.itemType = itemType;
				stackedItem.amount = amount;

				// Add into items
				items.Add(stackedItem);
			}

			// Trigger callback
			if (onItemChangedCallback != null)
			{
				onItemChangedCallback.Invoke();
			}
		}

		public void ClearInventory()
		{
			items.Clear();
		}*/
	}
}