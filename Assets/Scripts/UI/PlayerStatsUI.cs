using System.Collections;
using System.Collections.Generic;
using DEMO.Player;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class PlayerStatsUI : NetworkBehaviour
{
    private PlayerNetworkData _playerNetworkData = null;
    [SerializeField] private Slider healthBarSlider = null;
    public TextMeshProUGUI healthBarText;
    public TextMeshProUGUI bulletAmountText;


    public void Initialize(PlayerNetworkData playerNetworkData)
    {
        _playerNetworkData = playerNetworkData;
        _playerNetworkData.RegisterHealthChangedCallback(UpdateHealthBar);
        _playerNetworkData.RegisterBulletChangedCallback(UpdateBulletAmount);
    }

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
