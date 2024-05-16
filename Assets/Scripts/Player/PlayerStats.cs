using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Fusion;
using Fusion.Addons.Physics;


// Handles stats 
// of a player character
namespace DEMO.Player
{
    public class PlayerStats : NetworkBehaviour
    {
        [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
        [SerializeField] private Slider healthPointSlider = null;
        
        private HealthBar healthBar = null;
        public int maxHealth = 100;
        public int currentHealth;


        // Initialize
        public override void Spawned()
        {
            healthBar = FindFirstObjectByType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.setMaxHealth(maxHealth);
            }
            else
            {
                Debug.LogError("HealthBar not found!");
            }

            currentHealth = maxHealth;

            GameManager.Instance.SetPlayerNetworkHealth(healthPointSlider, maxHealth);
        }

        // When restart
        private void Respawn() 
        {
            playerNetworkRigidbody.transform.position = Vector3.zero;
            //healthPoint.Hp = maxHealth;

            healthBar.setMaxHealth(maxHealth);
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (Object.HasInputAuthority)
            {
                Debug.Log("Take damage once by "+GameManager.Instance.Runner.LocalPlayer);
                currentHealth -= damage;
                healthBar.setHealth(currentHealth);

                GameManager.Instance.SetPlayerNetworkHealth(healthPointSlider, currentHealth);

                if (currentHealth <= 0)
                {
                    //animator.SetTrigger("playerDeath");
                    //FindObjectOfType<GameManager>().EndGame();
                }
            }
        }
    }
}