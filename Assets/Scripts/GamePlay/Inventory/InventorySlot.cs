using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using DEMO;
using System.ComponentModel;
using UnityEngine.U2D.Animation;
using DEMO.DB;
using Unity.VisualScripting;

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

        private void Start()
        {
            itemType = Item.ItemType.None;
        }


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
            itemType = Item.ItemType.None;

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
            if (occupied && !onClickPanel.activeSelf)
            {
                // Change color of color code, if failed then color = white
                Color slotColor = UnityEngine.ColorUtility.TryParseHtmlString("#B6B6B6", out Color color) ? color : Color.white;
                slotButton.GetComponent<Image>().color = slotColor;

                onClickPanel.SetActive(true);
                var transformParent = GameObject.Find("InventoryPanel");
                onClickPanel.transform.SetParent(transformParent.transform);
                onClickPanel.GetComponent<ItemFunction>().SetSlot(this);
            }
            else if(occupied && onClickPanel.activeSelf)
            {
                ResetSlotColor();
                onClickPanel.SetActive(false);

                return;
            }
        }

        public void UseItem(PlayerNetworkData playerNetworkData)
        {
            item.Use(playerNetworkData);

            ResetSlotColor();
        }

        public void DiscardItem(PlayerNetworkData playerNetworkData)
        {
            item.Discard(playerNetworkData);

            ResetSlotColor();
        }

        public void GiftItem(PlayerNetworkData playerNetworkData, string targetPlayerName)
        {
            item.Gift(playerNetworkData, item.itemType, targetPlayerName);

            ResetSlotColor();
        }

        private void ResetSlotColor()
        {
            slotButton.GetComponent<Image>().color = Color.white;
        }
    }
}