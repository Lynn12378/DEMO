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
    [SerializeField] private float decreaseRate = 1f;
    [SerializeField] private float lastDecreaseTime;

    //Shelter UI
    public Slider DurabilitySlider;
    public TextMeshProUGUI textMeshPro;

    //Shelter 實體
    private BoxCollider2D boxCollider;
    private bool playerInRange = false;

    private void Awake()
    {
        // 添加BoxCollider2D
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    public override void Spawned()
    {
        Durability = MaxDurability;
        lastDecreaseTime = Time.time;
        UpdateDurabilityUI();
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

    public void Repair(float amount)
    {
        Durability += amount;
        if (Durability > MaxDurability)
        {
            Durability = MaxDurability;
        }
        UpdateDurabilityUI();
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

    public bool IsPlayerInRange()
    {
        return playerInRange;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
