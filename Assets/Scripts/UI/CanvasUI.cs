using System.Collections;
using System.Collections.Generic;
using DEMO.Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasUI : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider = null;
    public TextMeshProUGUI healthBarText;
    public TextMeshProUGUI bulletAmountText;

    public void SetCanvasHealth(int health)
    {
        healthBarSlider.value = health;
        healthBarText.text = health.ToString();
    }

    public void SetCanvasBullet(int bullet)
    {
        bulletAmountText.text = bullet.ToString();
    }
}
