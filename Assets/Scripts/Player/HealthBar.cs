using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DEMO.Player
{
    public class HealthBar : MonoBehaviour
    {
        public Slider healthSlider;
        public TextMeshProUGUI textMeshPro;

        public void setMaxHealth(int maxHealth)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;

            UpdateText();
        }

        public void setHealth(int health)
        {
            healthSlider.value = health;

            UpdateText();
        }

        private void UpdateText()
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = healthSlider.value.ToString();
            }
        }
    }
}
