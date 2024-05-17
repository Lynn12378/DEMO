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

    public override void Spawned()
    {
        Durability = MaxDurability; 
        UpdateDurabilityUI();
    }

    public override void FixedUpdateNetwork()
    {
        UpdateDurabilityUI();
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
