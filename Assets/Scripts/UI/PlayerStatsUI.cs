using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DEMO.Player
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private Slider healthPointSlider = null;


        public void SetPlayerNameUI(String name)
        {

        }

        public void SetHealthUI(int newValue)
        {
            healthPointSlider.value = newValue;
        }
    }
}
