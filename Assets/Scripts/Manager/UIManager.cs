using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DEMO.GamePlay.Inventory;
using DEMO.DB;

namespace DEMO.Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] Slider HPSlider = null;
        [SerializeField] private TMP_Text HPTxt = null;
        [SerializeField] Slider durabilitySlider = null;
        [SerializeField] private TMP_Text durabilityTxt = null;
        [SerializeField] private TMP_Text bulletAmountTxt = null;

        [SerializeField] private GameObject teamListPanel = null;

        [SerializeField] private GameObject inventoryPanel = null;
        [SerializeField] private Transform slotsBackground = null;

        private InventorySlot[] inventorySlots;
        private List<Item> tempItemList;    // To store PlayerNetworkData.itemList


        private void Start()
        {
            inventorySlots = slotsBackground.GetComponentsInChildren<InventorySlot>();
        }

        #region - PlayerNetworkData UI -
        public void UpdateHPSlider(int HP, int maxHP)
        {
            HPSlider.value = HP;
            HPTxt.text = $"HP: {HP}/{maxHP}";
        }

        public void UpdateDurabilitySlider(int durability, int maxDurability)
        {
            durabilitySlider.value = durability;
            durabilityTxt.text = $"Durability: {durability}/{maxDurability}";
        }

        public void UpdateBulletAmountTxt(int bulletAmount, int maxbulletAmount)
        {
            bulletAmountTxt.text = $"Bullet amount: {bulletAmount}/{maxbulletAmount}";
        }
        #endregion

        #region - Buttons -
        public void OnOpenInventoryButton()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }

        public void OnOpenTeamListButton()
        {
            teamListPanel.SetActive(!teamListPanel.activeSelf);
        }

        public void OnOrganizeButton()
        {
            OrganizeInventory(tempItemList);
        }
        #endregion 

        #region - Inventory -
        public void SetItemList(List<Item> items)
        {
            tempItemList = items;
        }

        public void OrganizeInventory(List<Item> items)
        {
            // Create dictionary to store item and amount in stack
            Dictionary<Item.ItemType, int> stackedItems = new Dictionary<Item.ItemType, int>();

            foreach (Item item in items)
            {
                // If item in dict, add amount
                if (stackedItems.ContainsKey(item.itemType))
                {
                    stackedItems[item.itemType] += item.quantity;
                }
                else
                {
                    // Else, add item in dict with its amount
                    stackedItems.Add(item.itemType, item.quantity);
                }
            }

            // Clear items list
            items.Clear();

            // Add stackedItems into items
            foreach (KeyValuePair<Item.ItemType, int> kvp in stackedItems)
            {
                Item.ItemType itemType = kvp.Key;
                int quantity = kvp.Value;

                // Create new item
                // Slightly error here
                Item stackedItem = new Item
                {
                    itemType = itemType,
                    quantity = quantity
                };

                // Add into items
                items.Add(stackedItem);
            }

            // Update UI for organized list
            UpdateInventoryUI(items);
        }

        public void UpdateInventoryUI(List<Item> items)
        {
            // Loop through all the slots
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                // Add item if there is an item to add
                if (i < items.Count)
                {
                    inventorySlots[i].AddItem(items[i]);

                    if (items[i].quantity > 1)
                    {
                        inventorySlots[i].ShowAmountText();
                    }
                    else
                    {
                        inventorySlots[i].itemAmount.text = " ";
                    }
                } 
                else
                {
                    // Otherwise clear the slot
                    inventorySlots[i].ClearSlot();
                }
            }
        }

        #endregion

        // Test for debug
        public string ShowList(List<Item> items)
        {
            string result = "Inventory: ";

            for(int i=0; i < items.Count; i++)
            {
                result += "ItemType: " + items[i].itemType + "; Quantity: " + items[i].quantity;
            }

            return result;
        }
    }
}