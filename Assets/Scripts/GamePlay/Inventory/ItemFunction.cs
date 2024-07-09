using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Fusion;

using DEMO.DB;
using DEMO.Manager;


namespace DEMO.GamePlay.Inventory
{
    public class ItemFunction : NetworkBehaviour
    {
        private InventorySlot currentSlot;              // Reference to the current InventorySlot
        private PlayerNetworkData playerNetworkData;    // Reference to LocalPlayer.PlayerNetworkData
        private PlayerOutputData playerOutputData;      // Reference to LocalPlayer.PlayerOutputData
        
        [SerializeField] private GameObject GiftPanel = null;
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

            var pOD = Runner.GetComponentInChildren<PlayerOutputData>();
            var pODRef = pOD.playerRef;
            if(pODRef == Runner.LocalPlayer)
            {
                playerOutputData = pOD;
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

        public void OnGiftItem(PlayerRef playerRef)
        {
            playerOutputData.giftNo++;
            Debug.Log(playerOutputData.playerRef.ToString() + " gift no. is " + playerOutputData.giftNo);

            currentSlot.GiftItem(playerNetworkData, playerRef);
            GiftPanel.SetActive(false);
            AfterFunction();
        }

        public void OnGiftClicked()
        {
            if (!GiftPanel.activeSelf)
            {
                float buttonHeight = 50f;
                float buttonSpacing = 15f;
                float initialPosY = -45f;

                GiftPanel.SetActive(true);
                PlayerRef player = GamePlayManager.Instance.Runner.LocalPlayer;

                foreach (GameObject button in playerButtonList)
                {
                    Destroy(button);
                }
                playerButtonList.Clear();

                int buttonCount = 0;

                foreach (PlayerNetworkData pnd in GamePlayManager.Instance.gamePlayerList.Values)
                {
                    if (GamePlayManager.Instance.gamePlayerList[player].playerRef != pnd.playerRef)
                    {
                        GameObject newPlayerButton = Instantiate(GiftPlayerButton);
                        playerButtonList.Add(newPlayerButton);
                        newPlayerButton.SetActive(true);
                        newPlayerButton.transform.SetParent(GiftPanel.transform, false);

                        TextMeshProUGUI playerName = newPlayerButton.GetComponentInChildren<TextMeshProUGUI>();
                        playerName.text = pnd.playerName;

                        RectTransform btnPos = newPlayerButton.gameObject.GetComponent<RectTransform>();
                        btnPos.anchoredPosition = new Vector2(btnPos.anchoredPosition.x, initialPosY - (buttonHeight + buttonSpacing) * buttonCount);
                        buttonCount++;

                        Button buttonComponent = newPlayerButton.GetComponent<Button>();
                        buttonComponent.onClick.AddListener(() => OnGiftItem(pnd.playerRef));
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
