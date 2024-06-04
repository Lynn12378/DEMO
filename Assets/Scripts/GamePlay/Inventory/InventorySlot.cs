using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using DEMO;

namespace DEMO.GamePlay.Inventory
{
    public class InventorySlot : MonoBehaviour 
    {
        public Button slotButton;               // Reference to the button component
        public Image itemImage;					// Reference to the icon image
        public TextMeshProUGUI itemAmount;	    // Reference to the amount text
        public bool occupied = false;           // Check if slot is occupied

        Item item;  // Current item in the slot

        // Add item to the slot
        public void AddItem (Item newItem)
        {
            item = newItem;

            itemImage.sprite = item.sprite.sprite;
            itemImage.enabled = true;
            itemAmount.enabled = false;
            slotButton.interactable = true;

            occupied = true;
        }

        public void ShowAmountText()
        {
            itemAmount.SetText(item.amount.ToString());
            itemAmount.enabled = true;
        }

        // Clear the slot
        public void ClearSlot ()
        {
            item = null;

            itemImage.sprite = null;
            itemImage.enabled = false;
            itemAmount.enabled = false;
            slotButton.interactable = false;

            occupied = false;
        }

        // Check if the slot is empty
        public bool IsEmpty()
        {
            return !occupied;
        }

        // Called when the item is pressed
        /*public void UseItem ()
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
                    inventory.Remove(item);
                }
            }
        }*/
    }
}