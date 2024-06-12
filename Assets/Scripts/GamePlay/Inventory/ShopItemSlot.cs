using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using TMPro;
using Fusion;

using DEMO.DB;

namespace DEMO.GamePlay.Inventory
{
    public class ItemSlot : NetworkBehaviour
    {
        Item item;                                          // To set item for each slot
        [SerializeField] private Item.ItemType itemType;    // To set itemType for each slot
        public Image itemImage;
        public SpriteLibraryAsset spriteLibraryAsset;       // To get sprite

        public Button leftButton;
        public Button rightButton;
        public Button buyButton;
        public TMP_InputField inputField;
        public TextMeshProUGUI costTxt;
        private int buyQuantity = 1;
        [SerializeField] private int buyCost;
        private int totalCost;

        private PlayerNetworkData playerNetworkData;


        private void Start()
        {
            if(itemType != Item.ItemType.None)
            {
                itemImage.sprite = spriteLibraryAsset.GetSprite("item", itemType.ToString());
                inputField.text = buyQuantity.ToString(); // Initialize inputField text
                UpdateTotalCost(); // Initialize total cost
            }
            else
            {
                itemImage.gameObject.SetActive(false);
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(false);
                inputField.gameObject.SetActive(false);
            }

            SetPND();
        }

        private void SetPND()
        {
            var pND = Runner.GetComponentInChildren<PlayerNetworkData>();
            var pRef = pND.playerRef;
            if(pRef == Runner.LocalPlayer)
            {
                playerNetworkData = pND;
            }
        }

        public void OnInputFieldValueChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                buyQuantity = result;
                UpdateTotalCost();
            }
            else
            {
                inputField.text = buyQuantity.ToString(); // If input not valid, back to latest buyQuantity
            }
        }

        public void OnInputFieldEndEdit(string value)
        {
            if (!int.TryParse(value, out int result) || result <= 0)
            {
                inputField.text = buyQuantity.ToString(); // Ensure input valid
            }
            else
            {
                UpdateTotalCost(); // Ensure total cost is updated when editing end
            }
        }

        public void OnLeftButton()
        {
            if (buyQuantity > 1)
            {
                buyQuantity--;
                inputField.text = buyQuantity.ToString();
                UpdateTotalCost();
            }
        }

        public void OnRightButton()
        {
            buyQuantity++;
            inputField.text = buyQuantity.ToString();
            UpdateTotalCost();
        }

        private void UpdateTotalCost()
        {
            totalCost = buyQuantity * buyCost;
            costTxt.text = $"$ {totalCost}";
        }

        public void OnBuyButton()
        {
            if(playerNetworkData.coinAmount >= totalCost)
            {
                // Create new item
                // Slightly error
                item = new Item
                {
                    itemType = itemType,
                    quantity = buyQuantity
                };

                // Add to player inventory
                playerNetworkData.itemList.Add(item);
                playerNetworkData.UpdateItemList();
                
                // Decrease player coin
                playerNetworkData.SetPlayerCoin_RPC(playerNetworkData.coinAmount - totalCost);
            }
            else
            {
                Debug.Log("Not enough coin.");
            }
        }
    }
}
