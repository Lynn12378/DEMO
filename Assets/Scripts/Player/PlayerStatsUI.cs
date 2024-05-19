using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DEMO.Player
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private CanvasUI canvasUI = null;
        [SerializeField] private Slider healthPointSlider = null;
        public TextMeshProUGUI bulletAmountText;

        public void SetPlayerNameUI(String name)
        {

        }

        public void SetHPBarValue(int newValue)
        {
            healthPointSlider.value = newValue;
        }

        public void UpdateBulletAmount(int amount)
        {
            if (bulletAmountText != null)
            {
                bulletAmountText.text = amount.ToString();
            }
        }
    }
}
