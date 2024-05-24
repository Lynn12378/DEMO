using UnityEngine;
using Fusion;
using System;

public class PlayerNetworkData : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(HandleHealthChanged))]
    public int currentHealth { get; set; }
    [Networked, OnChangedRender(nameof(HandleBulletChanged))]
    public int currentBullet { get; set; }
    //[Networked, OnChangedRender(nameof(HandleInventoryChanged))]
    //public Inventory inventory { get; set; }

    private Action<int> OnHealthChanged = null;
    private Action<int> OnBulletChanged = null;

    void Start()
    {
        // Initialize
        if (Object.HasStateAuthority)
        {
            currentHealth = 100;
            currentBullet = 30;
            //inventory = new Inventory();
            //playerName = "Player " + Object.InputAuthority;
        }
    }

    void Update()           // Test
    {
        if(HasStateAuthority && Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(10);
        }

        if(HasStateAuthority && Input.GetKeyDown(KeyCode.B))
        {
            UseBullet(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            currentHealth -= damage;
        }
    }

    public void UseBullet(int amount)
    {
        if (Object.HasStateAuthority)
        {
            currentBullet -= amount;
        }
    }

    void HandleHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    void HandleBulletChanged()
    {
        OnBulletChanged?.Invoke(currentBullet);
    }

    void HandleInventoryChanged()
    {
        //OnInventoryChanged?.Invoke(currentBullet);
    }


    #region /-- Register & Unregister callbacks --/
    public void RegisterHealthChangedCallback(Action<int> callback)
    {
        OnHealthChanged += callback;
    }

    public void UnregisterHealthChangedCallback(Action<int> callback)
    {
        OnHealthChanged -= callback;
    }

    public void RegisterBulletChangedCallback(Action<int> callback)
    {
        OnBulletChanged += callback;
    }

    public void UnregisterBulletChangedCallback(Action<int> callback)
    {
        OnBulletChanged -= callback;
    }
#endregion
}
