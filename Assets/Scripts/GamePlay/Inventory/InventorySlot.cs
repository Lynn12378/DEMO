using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using DEMO;
using System.ComponentModel;
using UnityEngine.U2D.Animation;
using DEMO.DB;

namespace DEMO.GamePlay.Inventory
{
    public class InventorySlot : MonoBehaviour 
    {
        public Button slotButton;               // Reference to the button component
        public Image itemImage;					// Reference to the icon image
        public TextMeshProUGUI itemAmount;	    // Reference to the amount text
        public bool occupied = false;           // Check if slot is occupied

        Item item;                  // Current item
        Item.ItemType itemType;     // Current ItemType in slot
        public SpriteLibraryAsset spriteLibraryAsset;   // To get sprite

        [SerializeField] private GameObject onClickPanel = null;


        // Add item to the slot
        public void AddItem (Item newItem)
        {
            item = newItem;
            itemType = newItem.itemType;

            itemImage.sprite = spriteLibraryAsset.GetSprite("item", itemType.ToString());
            itemImage.enabled = true;
            itemAmount.enabled = false;
            slotButton.interactable = true;

            occupied = true;
        }

        public void ShowAmountText()
        {
            itemAmount.SetText(item.quantity.ToString());
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

        // Show panel to delete, use, or give away item
        public void OnSlotClicked()
        {
            if (occupied)
            {
                // Change color of color code, if failed then color = white
                Color slotColor = ColorUtility.TryParseHtmlString("#B6B6B6", out Color color) ? color : Color.white;
                slotButton.GetComponent<Image>().color = slotColor;

                onClickPanel.SetActive(!onClickPanel.activeSelf);
                var transformParent = GameObject.Find("InventoryPanel");
                onClickPanel.transform.SetParent(transformParent.transform);
                onClickPanel.GetComponent<ItemFunction>().SetSlot(this);
            }
        }

        public void UseItem(PlayerNetworkData playerNetworkData)
        {
            item.Use(playerNetworkData);

            // Reset slot color
            slotButton.GetComponent<Image>().color = Color.white;

            //DecreaseItemQuantity();
        }

        public void DiscardItem(PlayerNetworkData playerNetworkData)
        {
            Debug.Log("Discard Item: " + item.itemType);

            // Reset slot color
            slotButton.GetComponent<Image>().color = Color.white;

            //DecreaseItemQuantity();
        }

        public void GiftItem(PlayerNetworkData playerNetworkData)
        {
            Debug.Log("Gift Item: " + item.itemType);

            // Reset slot color
            slotButton.GetComponent<Image>().color = Color.white;

            //DecreaseItemQuantity();
        }

        private void DecreaseItemQuantity()
        {
            if (item.quantity > 1)
            {
                item.quantity--;
                itemAmount.SetText(item.quantity.ToString());
            }
            else
            {
                ClearSlot();
            }

            // Reset slot color
            slotButton.GetComponent<Image>().color = Color.white;
        }
    }
}