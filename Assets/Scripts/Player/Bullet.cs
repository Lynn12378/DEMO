using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

namespace DEMO.Player
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private NetworkRigidbody2D networkRigidbody = null;

        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private float bulletTime = 0.5f;
        [SerializeField] private int damage = 10;

        [Networked] private TickTimer life { get; set; }

        public Vector2 mousePosition;

        public void Init(Vector2 mousePosition)
        {
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);
            this.mousePosition = mousePosition.normalized;

            //Debug.Log($"Init Bullet");
            // networkRigidbody.InterpolationTarget.gameObject.SetActive(true);
            networkRigidbody.Rigidbody.velocity = Vector2.zero;
        }

        public override void FixedUpdateNetwork()
        {
            networkRigidbody.Rigidbody.velocity = mousePosition * bulletSpeed;
            //Debug.Log($"mousePosition from bullet{mousePosition}");
            
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Runner.Despawn(Object);
            }
        }
    }
}