using System.Collections;
using System.Collections.Generic;
using DEMO.Player;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class CanvasUI : NetworkBehaviour
{
    [SerializeField] private Slider healthBarSlider = null;
    public TextMeshProUGUI healthBarText;
    public TextMeshProUGUI bulletAmountText;

    public void UpdateUI(PlayerRef player, PlayerNetworkData playerData)
    {
        UpdateCanvasHealth(playerData.currentHealth);
        UpdateCanvasBullet(playerData.currentBullet);
    }
    
    public void UpdateCanvasHealth(int health)
    {
        healthBarSlider.value = health;
        healthBarText.text = health.ToString();
    }

    public void UpdateCanvasBullet(int bullet)
    {
        bulletAmountText.text = bullet.ToString();
    }
}
