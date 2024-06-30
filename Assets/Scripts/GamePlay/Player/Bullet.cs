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

<<<<<<< HEAD
        public void Init(Vector2 mousePosition)
        {
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);
            this.mousePosition = mousePosition.normalized;
=======
        private PlayerRef shooterPlayerRef;

        public void Init(Vector2 mousePosition, PlayerRef shooter)
        {
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);
            this.mousePosition = mousePosition.normalized;
            shooterPlayerRef = shooter;
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
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
<<<<<<< HEAD
                enemy.TakeDamage(damage);
=======
                enemy.TakeDamage(damage, shooterPlayerRef);
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
                Runner.Despawn(Object);
            }
        }
    }
}