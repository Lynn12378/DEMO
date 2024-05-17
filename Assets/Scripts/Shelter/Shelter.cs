using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class Shelter : NetworkBehaviour
{
    [Networked] public float Durability { get; set; } 
    [SerializeField] private int MaxDurability = 100; 
    public Slider DurabilitySlider;
    public TextMeshProUGUI textMeshPro;

    [SerializeField] private float decreaseRate = 1f;
    [SerializeField] private float lastDecreaseTime;

    public override void Spawned()
    {
        Durability = MaxDurability; 
        UpdateDurabilityUI();
        lastDecreaseTime = Time.time;
    }

    public override void FixedUpdateNetwork()
    {
        if (Time.time - lastDecreaseTime >= 1f)
        {
            Durability -= decreaseRate;
            lastDecreaseTime = Time.time;

            if (Durability <= 0)
            {
                Durability = 0;
                //EndGame();
            }
        UpdateDurabilityUI();
        }
    }

    public void SetMaxDurability(int maxDurability)
    {
        DurabilitySlider.maxValue = maxDurability;
        DurabilitySlider.value = maxDurability;
        UpdateText();
    }

    public void SetDurability(int durability)
    {
        DurabilitySlider.value = durability;
        UpdateText();
    }

    private void UpdateText()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = DurabilitySlider.value.ToString();
        }
    }

    private void UpdateDurabilityUI()
    {
        if (DurabilitySlider != null)
        {
            DurabilitySlider.value = Durability;
            UpdateText();
        }
    }
}
