using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DEMO.Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] Slider HPSlider = null;
        [SerializeField] private TMP_Text HPTxt = null;
        [SerializeField] Slider durabilitySlider = null;
        [SerializeField] private TMP_Text durabilityTxt = null;
        [SerializeField] private TMP_Text bulletAmountTxt = null;
        
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
    }
}