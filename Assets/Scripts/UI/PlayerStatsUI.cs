using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

namespace DEMO.UI
{
    public class PlayerStatsUI : NetworkBehaviour
    {
        [SerializeField] private Slider healthBarSlider = null;
        public TextMeshProUGUI healthBarText;
        public TextMeshProUGUI bulletAmountText;

        public void UpdateHealthBar(int health)
        {
            healthBarSlider.value = health;
            healthBarText.text = health.ToString();
        }

        public void UpdateBulletAmount(int amount)
        {
            bulletAmountText.text = amount.ToString();
        }
    }
}
