using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DEMO.GamePlay.Inventory;
using DEMO.DB;
using Fusion;

namespace DEMO.Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] Slider HPSlider = null;
        [SerializeField] private TMP_Text HPTxt = null;
        [SerializeField] Slider foodSlider = null;
        [SerializeField] private TMP_Text foodTxt = null;
        [SerializeField] Slider durabilitySlider = null;
        [SerializeField] private TMP_Text durabilityTxt = null;
        [SerializeField] private TMP_Text bulletAmountTxt = null;

        [SerializeField] private GameObject warningBoxPanel = null;
        [SerializeField] private TextMeshProUGUI warningText = null;

        [SerializeField] private GameObject micIcon;
        [SerializeField] private TMP_Text micTxt = null;

        [SerializeField] private GameObject shopPanel = null;
        [SerializeField] private TextMeshProUGUI playerCoinAmount = null;
        private ShopItemSlot[] itemSlots;

        [SerializeField] private GameObject teamListPanel = null;

        [SerializeField] private GameObject rankPanel = null;

        [SerializeField] private GameObject inventoryPanel = null;
        [SerializeField] private Transform slotsBackground = null;
        private InventorySlot[] inventorySlots;
        private List<Item> tempItemList;

        public Transform baseTransform;
        public RectTransform arrowRectTransform;
        public float initialAngleOffset = 90f;

        private PlayerRef playerRef;


        private void Start()
        {
            inventorySlots = slotsBackground.GetComponentsInChildren<InventorySlot>();
        }

        public void InitializeItemSlots(PlayerNetworkData playerNetworkData)
        {
            itemSlots = shopPanel.GetComponentsInChildren<ShopItemSlot>();
            foreach (var slot in itemSlots)
            {
                slot.Initialize(playerNetworkData);
            }
        }

        public void SetPlayerRef(PlayerRef playerRef)
        {
            this.playerRef = playerRef;
        }

        #region  - Warning Box -
        public void ShowWarningBox(string text)
        {
            warningBoxPanel.SetActive(true);
            warningText.SetText(text);
        }

        public void HideWarningBox()
        {
            warningBoxPanel.SetActive(false);
        }
        #endregion

        #region - Minimap -
        public void UpdateMinimapArrow(Transform playerTransform)
        {
            Vector3 direction = playerTransform.position - baseTransform.position;
        
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - initialAngleOffset;

            arrowRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        #endregion

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

        public void UpdateFoodSlider(int food, int maxFood)
        {
            foodSlider.value = food;
            foodTxt.text = $"Food: {food}/{maxFood}";
        }

        public void UpdateBulletAmountTxt(int bulletAmount, int maxbulletAmount)
        {
            bulletAmountTxt.text = $"Bullet amount: {bulletAmount}/{maxbulletAmount}";
        }

        public void UpdateCoinAmountTxt(int coinAmount)
        {
            playerCoinAmount.SetText(coinAmount.ToString());
        }

        public void UpdateMicIconColor(int isSpeaking)
        {
            Image micIconImage = micIcon.GetComponent<Image>();

            if (isSpeaking == 0)
            {
                micIconImage.color = Color.green; // Local player speaking
            }
            else if (isSpeaking == 1)
            {
                micIconImage.color = Color.blue; // Other player speaking
            }
            else
            {
                micIconImage.color = Color.gray; // Default color when not speaking
            }
        }

        public void UpdateMicTxt(string playerName)
        {
            if(playerName == "none")
            {
                micTxt.text = " ";
            }
            else
            {
                micTxt.text = $"Speaking : {playerName}";
            }
        }
        #endregion

        #region - Buttons -
        public void OnOpenInventoryButton()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }

        public void OnOpenShopButton()
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }

        public void OnOpenTeamListButton()
        {
            teamListPanel.SetActive(!teamListPanel.activeSelf);
        }

        public void OnOpenRankButton()
        {
            rankPanel.SetActive(!rankPanel.activeSelf);

            if(rankPanel.activeInHierarchy)
            {
                GamePlayManager.Instance.AddRankNo(playerRef);
            }
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
            List<Item> placeholderItems = new List<Item>();

            foreach (Item item in items)
            {
                // If item is a Placeholder, add it to the placeholderItems list
                if (item.itemId >= 5 && item.itemId <= 13)
                {
                    placeholderItems.Add(item);
                }
                else
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
                    itemId = (int)itemType,
                    quantity = quantity
                };

                // Add into items
                items.Add(stackedItem);
            }

            // Add placeholder items to the final items list
            foreach (Item placeholderItem in placeholderItems)
            {
                items.Add(placeholderItem);
            }

            UpdateInventoryUI(items);

            GamePlayManager.Instance.AddOrganizeNo(playerRef);
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

        #endregion
    }
}