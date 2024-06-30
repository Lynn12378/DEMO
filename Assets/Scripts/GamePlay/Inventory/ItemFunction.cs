using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using DEMO.DB;
using DEMO.Manager;


namespace DEMO.GamePlay.Inventory
{
    public class ItemFunction : NetworkBehaviour
    {
        private InventorySlot currentSlot;              // Reference to the current InventorySlot
        private PlayerNetworkData playerNetworkData;    // Reference to LocalPlayer.PlayerNetworkData
        [SerializeField] private GameObject GiftPanel = null;
        [SerializeField] private GameObject GiftButton = null;
        [SerializeField] private GameObject GiftPlayerButton = null;
        private List<GameObject> playerButtonList = new List<GameObject>(); 
        
        public void SetSlot(InventorySlot slot)
        {
            currentSlot = slot;

            var pND = Runner.GetComponentInChildren<PlayerNetworkData>();
            var pRef = pND.playerRef;
            if(pRef == Runner.LocalPlayer)
            {
                playerNetworkData = pND;
            }
        }

        private void AfterFunction()
        {
            // Update UI
            playerNetworkData.UpdateItemList();

            // Hide the panel
            gameObject.SetActive(false);
        }

        public void OnUseItem()
        {
            currentSlot.UseItem(playerNetworkData);
            AfterFunction();
        }

        public void OnDiscardItem()
        {
            currentSlot.DiscardItem(playerNetworkData);
            AfterFunction();
        }

        public void OnGiftItem(string playerName)
        {
            currentSlot.GiftItem(playerNetworkData, playerName);
            GiftPanel.SetActive(false);
            AfterFunction();
        }

        public void OnGiftClicked()
        {
            if (!GiftPanel.activeSelf)
            {
                float pos = 150f;
                GiftPanel.SetActive(true);
                var transformParent = GameObject.Find("InventoryPanel");
                GiftPanel.transform.SetParent(transformParent.transform);
                PlayerRef player= GamePlayManager.Instance.Runner.LocalPlayer;

                foreach (GameObject button in playerButtonList)
                {
                    Destroy(button);
                }
                
                foreach (PlayerNetworkData pnd in GamePlayManager.Instance.gamePlayerList.Values)
                {
                    if (GamePlayManager.Instance.gamePlayerList[player].playerName != pnd.playerName)
                    {
                        GameObject newPlayerButton = Instantiate(GiftPlayerButton);
                        playerButtonList.Add(newPlayerButton);
                        newPlayerButton.SetActive(true);
                        newPlayerButton.transform.SetParent(GiftPanel.transform, false);
                        
                        TextMeshProUGUI playerName = newPlayerButton.GetComponentInChildren<TextMeshProUGUI>();
                        playerName.text = pnd.playerName;

                        RectTransform btnPos = newPlayerButton.gameObject.GetComponent<RectTransform>();
                        btnPos.anchoredPosition = new Vector2(btnPos.anchoredPosition.x, pos);
                        pos -= 65;

                        Button buttonComponent = newPlayerButton.GetComponent<Button>();
                        buttonComponent.onClick.AddListener(() => OnGiftItem(pnd.playerName));
                    }
                }
            }
            else
            {
                GiftPanel.SetActive(false);
            }
        }
    }
}
