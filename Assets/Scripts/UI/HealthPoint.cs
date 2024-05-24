using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class HealthPoint : NetworkBehaviour
{
    [SerializeField] private Slider healthPointSlider = null;

    [Networked, OnChangedRender(nameof(HealthPointChanged))]
    public int currentHealth { get; set; }

    void Update()
    {
        if(HasStateAuthority && Input.GetKeyDown(KeyCode.Z))
        {
            UpdateHealthPointUI(10);
        }
    }

    public void UpdateHealthPointUI(int damage)
    {
        currentHealth -= damage;
    }

    void HealthPointChanged()
    {
        healthPointSlider.value = currentHealth;
    }
}
