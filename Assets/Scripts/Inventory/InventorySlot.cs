using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* Sits on all InventorySlots. */

public class InventorySlot : MonoBehaviour {

	public Image icon;					// Reference to the icon image
	public TextMeshProUGUI amountText;	// Reference to the amount text
	public Button removeButton;			// Reference to the remove button

	Item item;  // Current item in the slot

	// Add item to the slot
	public void AddItem (Item newItem)
	{
		item = newItem;

		icon.sprite = item.GetSprite();
		icon.enabled = true;
		removeButton.interactable = true;
	}

	public void ShowAmountText()
	{
		amountText.SetText(item.amount.ToString());
		amountText.enabled = true;
	}

	public void HideAmountText()
	{
		amountText.enabled = false;
	}


	// Clear the slot
	public void ClearSlot ()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		removeButton.interactable = false;

		HideAmountText();
	}

	// Called when the remove button is pressed
	public void OnRemoveButton ()
	{
		if(item.amount > 1)
			{
				item.amount -= 1;
				amountText.SetText(item.amount.ToString());
				if(item.amount == 1)
				{
					amountText.SetText("");
				}
			}
			else
			{
				Inventory.instance.Remove(item);
			}
	}

	// Called when the item is pressed
	public void UseItem ()
	{
		if (item != null && ItemUseManager.instance != null)
		{
			ItemUseManager.instance.UseItemByManager(item.GetName());

			if(item.amount > 1)
			{
				item.amount -= 1;
				amountText.SetText(item.amount.ToString());
				if(item.amount == 1)
				{
					amountText.SetText("");
				}
			}
			else
			{
				item.RemoveFromInventory();
			}
		}
	}
}