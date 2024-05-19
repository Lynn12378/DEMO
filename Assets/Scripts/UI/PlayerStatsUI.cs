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


        public void SetPlayerNameUI(String name)
        {

        }

        public void SetHealthUI(int newValue)
        {
            healthPointSlider.value = newValue;
            canvasUI.SetCanvasHealth(newValue);
        }

        public void SetBulletUI(int amount)
        {
            canvasUI.SetCanvasBullet(amount);
        }
    }
}
