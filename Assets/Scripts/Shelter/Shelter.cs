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

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private bool playerInRange = false;

    private void Awake()
    {
        // 添加和配置 BoxCollider2D
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        boxCollider.size = new Vector2(25f, 25f); // 设置为你需要的大小
        boxCollider.offset = Vector2.zero;
        boxCollider.isTrigger = true;

        // 确保添加了 Rigidbody2D 并设置为 Kinematic
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic; // 设置为 Kinematic
        rb.simulated = true;
    }

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

    public void Repair(float amount)
    {
        Durability += amount;
        if (Durability > MaxDurability)
        {
            Durability = MaxDurability;
        }
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

    public bool IsPlayerInRange()
    {
        return playerInRange;
    }
}
