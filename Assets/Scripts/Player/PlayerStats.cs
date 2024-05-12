using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Addons.Physics;


// Handles stats 
// of a player character
public class PlayerStats : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;

    private HealthBar healthBar = null;
    private HealthPoint healthPoint = null;
    public int maxHealth = 100;
    public int currentHealth;


    Rigidbody2D rb;
    Animator animator;

    void Start()
    {
        healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
        {
            healthBar.setMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogError("HealthBar not found!");
        }

        currentHealth = maxHealth;
    }

    private void Update() //////////////////////////////////// Test
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }

    // Initialize healthPoint
    public override void Spawned() 
    {
        if (HasStateAuthority)
        {
            healthPoint =  GetComponentInChildren<HealthPoint>();
            healthPoint.Hp = maxHealth;
        }
    }

    // When restart
    private void Respawn() 
    {
        playerNetworkRigidbody.transform.position = Vector3.zero;
        healthPoint.Hp = maxHealth;

        healthBar.setMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }

    public override void FixedUpdateNetwork()
    {
        if (healthPoint.Hp <= 0)
        {
            Respawn();
        }
    }

    public void TakeDamage(int damage)
    {
        if (HasInputAuthority)
        {
            currentHealth -= damage;
            healthBar.setHealth(currentHealth);

            healthPoint.Hp -= damage;

            if (currentHealth <= 0)
            {
                //animator.SetTrigger("playerDeath");
                //FindObjectOfType<GameManager>().EndGame();
            }
        }
    }
}
