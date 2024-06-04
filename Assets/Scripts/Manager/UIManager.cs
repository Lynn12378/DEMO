using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DEMO.GamePlay.Inventory;

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

        private void Start()
        {
            inventorySlots = slotsBackground.GetComponentsInChildren<InventorySlot>();
        }
        
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

        #region - Inventory -

        public void UpdateInventoryUI(Item newItem)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.IsEmpty())
                {
                    slot.AddItem(newItem);
                    break;
                }
            }
        }

        public void OnOrganizeButton()
        {

        }

        public void OnOpenInventoryButton()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }

        #endregion

        public void OnOpenTeamListButton()
        {
            teamListPanel.SetActive(!teamListPanel.activeSelf);
        }
    }
}