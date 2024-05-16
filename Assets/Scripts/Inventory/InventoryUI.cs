using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Fusion;
using DEMO;

public class InventoryUI : MonoBehaviour {

	public Transform itemsParent;	// The parent object of all the items
	public GameObject inventoryUI;	// The entire UI

	Inventory inventory;	// Current inventory
	ItemUseManager itemUseManager;

	InventorySlot[] slots;	// List of all the slots

	public void Initialize()
	{
		inventory = PlayerInventoryManager.instance.GetPlayerInventory(GameManager.Instance.Runner.LocalPlayer);
		inventory.onItemChangedCallback += UpdateUI;	// Subscribe to the onItemChanged callback

		itemUseManager = ItemUseManager.instance;

		// Populate our slots array
		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
	}

	// Using a delegate on the Inventory.
	void UpdateUI ()
	{
		// Loop through all the slots
		for (int i = 0; i < slots.Length; i++)
		{
            // Add item if there is an item to add
			if (i < inventory.items.Count)
			{
				slots[i].AddItem(inventory.items[i]);

				if (inventory.items[i].amount > 1)
				{
					slots[i].ShowAmountText();
				}
				else
				{
					slots[i].HideAmountText();
				}
			} 
            else
			{
				// Otherwise clear the slot
				slots[i].ClearSlot();
			}
		}
	}

	// Stack the items with amount > 1
	public void OnOrganizeButton()
	{
		inventory.OrganizeInventory();
	}

	public void OnInventoryButton()
	{
		inventoryUI.SetActive(!inventoryUI.activeSelf);
	}
}