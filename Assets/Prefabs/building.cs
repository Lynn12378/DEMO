using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class building : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private int maxDurability = 100; // 基地最大耐久度
    [SerializeField]private int currentDurability; // 當前基地耐久度
    [SerializeField]private int repairAmount = 20; // 每次修復的耐久度
    [SerializeField]private float durabilityDecreaseInterval = 60f; // 耐久度每隔多少秒减少一次
    [SerializeField]private float durabilityDecreasePercentage = 0.01f; // 每次减少的耐久度百分比

    public Slider durabilitySlider; // 顯示耐久度的UI Slider
    private float timer; // 計時器
}
