using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Fusion;
using Fusion.Addons.Physics;
using Unity.VisualScripting;

namespace DEMO
{
    public class Enemy : NetworkBehaviour
    {
        [SerializeField] private NetworkRigidbody2D enemyNetworkRigidbody = null;
        private int maxHp = 50;
        [SerializeField] private EnemyHealthPoint healthPoint = null;

        private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
        private int causeDamage = 20;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float range;
        [SerializeField] private float maxDistance;
        private Vector2 wayPoint;
        private bool patrolAlongXAxis;
        private float lastDestinationChangeTime;

        // Initialize
        public override void Spawned() 
        {
            healthPoint.Hp = maxHp;

            // Pick an axis to patrol
            patrolAlongXAxis = Random.Range(0, 2) == 0 ? true : false;
            SetNewDestination();
            // Set lastDestinationChangeTime
            lastDestinationChangeTime = Time.time;
        }

        public override void FixedUpdateNetwork()
        { 
            if(Vector2.Distance(transform.position, wayPoint) < range)
            {
                if (Time.time - lastDestinationChangeTime > 1f)
                {
                    SetNewDestination();
                    // Update lastDestinationChangeTime
                    lastDestinationChangeTime = Time.time;
                } 
            }

            // Calculate direction from transform to destination
            Vector2 direction = (wayPoint - (Vector2)transform.position).normalized;
            // Set velocity
            enemyNetworkRigidbody.Rigidbody.velocity = direction * moveSpeed;
        }

        private void SetNewDestination()
        {
            if (patrolAlongXAxis)
            {
                // Patrol on X axis
                wayPoint = new Vector2(Random.Range(-maxDistance, maxDistance), transform.position.y);
            }
            else
            {
                // Patrol on Y axis
                wayPoint = new Vector2(transform.position.x, Random.Range(-maxDistance, maxDistance));
            }
        }

        public void TakeDamage(int damage)
        {
            healthPoint.Hp -= damage;
            if (healthPoint.Hp <= 0)
            {
                Runner.Despawn(Object);
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            var netObj = collision.collider.GetComponent<NetworkBehaviour>().Object;

                if (netObj == null) return;

                var damagable = collision.collider.GetComponent<IDamagable>();

                if (damagable != null && netObj.InputAuthority != Object.InputAuthority)
                {
                    if(netObj.CompareTag("Player"))
                    {
                        damagable.TakeDamage(causeDamage);
                    }
                }
        }
    }
}