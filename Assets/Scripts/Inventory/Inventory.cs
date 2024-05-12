using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	#region Singleton

	public static Inventory instance;

	void Awake ()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Inventory found!");
			return;
		}

		instance = this;
	}

	#endregion

	// Callback which is triggered when an item gets added or removed
	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public int space = 10;	// Amount of slots in inventory

	// Current list of items in inventory
	public List<Item> items = new List<Item>();

	// Add a new item. If there is enough room return true. Else we return false.
	public bool Add (Item item)
	{
		// Don't do anything if it's a default item
		//if (!item.isDefaultItem)
		//{
			// Check if out of space
			if (items.Count >= space)
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
	public void Remove (Item item)
	{
		items.Remove(item);

		// Trigger callback
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
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

		// Trigger callback
		if (onItemChangedCallback != null)
		{
			onItemChangedCallback.Invoke();
		}
	}

	public void ClearInventory()
	{
		items.Clear();
	}
}