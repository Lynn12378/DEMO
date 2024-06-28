using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;
using DEMO.GamePlay.Player;
using DEMO.Manager;
using DEMO.DB;

namespace DEMO.GamePlay.EnemyScript
{
    public class Enemy : NetworkBehaviour
    {
        [SerializeField] private NetworkRigidbody2D enemyNetworkRigidbody = null;
        private int directDamage = 10;
        private int damageOverTime = 5;
        private float damageInterval = 3f;  // Interval until next damage
        [Networked] private TickTimer damageTimer { get; set; } // Timer to countdown next damage

        [SerializeField] public Slider hPSlider;
        [Networked] [OnChangedRender(nameof(HandleHpChanged))]
        public int Hp { get; set; }
        private int maxHp = 50;

        [SerializeField] private float moveSpeed;   // 2f
        [SerializeField] private float range;       // 1
        [SerializeField] private float maxDistance; // 4
        private Vector2 wayPoint;
        private bool patrolAlongXAxis;
        private float patrolInterval = 5f;
        [Networked] private TickTimer patrolTimer { get; set; }

        public PlayerDetection playerDetection;

        // Initialize
        public override void Spawned() 
        {
            var enemyTransform = GameObject.Find("Enemy");
            transform.SetParent(enemyTransform.transform, false);

            GamePlayManager.Instance.enemyList.Add(this);

            Hp = maxHp;

            damageTimer = TickTimer.CreateFromSeconds(Runner, damageInterval);
            patrolTimer = TickTimer.CreateFromSeconds(Runner, patrolInterval);

            // Pick an axis to patrol
            patrolAlongXAxis = Random.Range(0, 2) == 0 ? true : false;
            SetNewDestination();
        }

        #region - Patrol & Player Detect -
        public override void FixedUpdateNetwork()
        {
            Vector2 direction = Vector2.zero;

            if(playerDetection.detectedObjs.Count > 0)
            {
                // Calculate direction to detected player
                Vector2 targetPosition = playerDetection.detectedObjs[0].transform.position;
                direction = (targetPosition - (Vector2)transform.position).normalized;
                direction.y += -0.5f; // Offset
                direction.Normalize();
            }
            else
            {
                // If position too close or patrol timer runs out, set new destination
                if (Vector2.Distance(transform.position, wayPoint) < range || patrolTimer.Expired(Runner))
                {
                    // Pick an axis to patrol
                    patrolAlongXAxis = Random.Range(0, 2) == 0 ? true : false;
                    //SetNewDestination();
                }

                // Calculate direction from transform to destination
                direction = (wayPoint - (Vector2)transform.position).normalized;
            }

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

            patrolTimer = TickTimer.CreateFromSeconds(Runner, patrolInterval);
        }
        #endregion

        #region - Damage to Player - 
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                var player = collision.collider.GetComponent<PlayerController>();
                if (player != null && player.Object.InputAuthority != Object.InputAuthority)
                {
                    player.TakeDamage(directDamage);
                }
            }
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                var player = collision.collider.GetComponent<PlayerController>();
                if (player != null && player.Object.InputAuthority != Object.InputAuthority)
                {
                    if(damageTimer.Expired(Runner))
                    {
                        player.TakeDamage(damageOverTime);

                        // Reset timer
                        damageTimer = TickTimer.CreateFromSeconds(Runner, damageInterval);
                    }
                }
            }
        }
        #endregion

        #region - Hp -
        public void TakeDamage(int damage, PlayerRef shooter)
        {
            Hp -= damage;
            SetEnemyHP_RPC(Hp);
            if (Hp <= 0)
            {
                foreach (var kvp in GamePlayManager.Instance.playerOutputList)
                {
                    PlayerRef playerRefKey = kvp.Key;
                    PlayerOutputData playerOutputDataValue = kvp.Value;

                    if (shooter == playerRefKey)
                    {
                        playerOutputDataValue.AddKillNo_RPC();
                    }
                }

                DespawnEnemy_RPC(Object);
            }
        }

        private void HandleHpChanged()
        {
            hPSlider.value = Hp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetEnemyHP_RPC(int hp)
        {
            Hp = hp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DespawnEnemy_RPC(NetworkObject netObj)
        {
            Runner.Despawn(netObj);
        }
        #endregion
    }
}