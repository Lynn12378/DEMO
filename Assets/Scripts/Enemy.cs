using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Fusion;
using Fusion.Addons.Physics;

public class Enemy : NetworkBehaviour
{
    private int maxHp = 50;
    private HealthPoint healthPoint = null;

    // Initialize
    public override void Spawned() 
    {
        healthPoint = GetComponentInChildren<HealthPoint>();
        healthPoint.Hp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        healthPoint.Hp -= damage;
        if (healthPoint.Hp <= 0)
        {
            Runner.Despawn(Object);
        }
    }
}

