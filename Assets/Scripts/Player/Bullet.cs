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

        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private float bulletTime = 0.5f;
        [SerializeField] private int damage = 10;

        [Networked] private TickTimer life { get; set; }

        public Vector2 mousePosition;

        public override void Spawned()
        {
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);

            networkRigidbody.InterpolationTarget.gameObject.SetActive(true);
            
            networkRigidbody.Rigidbody.velocity = Vector2.zero;
        }

        public override void FixedUpdateNetwork()
        {
            Vector2 mouseVector = mousePosition.normalized;
            Debug.Log(mouseVector);
            networkRigidbody.Rigidbody.velocity = mouseVector * bulletSpeed;

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