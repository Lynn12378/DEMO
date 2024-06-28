using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

using DEMO.GamePlay.EnemyScript;

namespace DEMO.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private float bulletSpeed = 1f;
        [SerializeField] private float bulletTime = 1f;
        [SerializeField] private int damage = 10;

        [Networked] private TickTimer life { get; set; }

        public Vector2 mousePosition;

        private PlayerRef shooterPlayerRef;

        public void Init(Vector2 mousePosition, PlayerRef shooter)
        {
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);
            this.mousePosition = mousePosition.normalized;
            shooterPlayerRef = shooter;
            transform.Translate(Vector2.zero);
        }

        public override void FixedUpdateNetwork()
        {
            transform.Translate(Vector2.right * bulletSpeed);
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var enemy = collider.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, shooterPlayerRef);
                Runner.Despawn(Object);
            }
        }
    }
}