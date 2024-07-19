using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
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
        public enum EnemyType
        {
            HighDamage,
            HighHP,
            HighSpeed,
            Normal,
        }

        [SerializeField] private NetworkRigidbody2D enemyNetworkRigidbody = null;
        [Networked] public int enemyID { get; set;}
        public EnemyType enemyType;
        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] public SpriteRenderer spriteRenderer;
        
        private int directDamage = 10;
        private int damageOverTime = 5;
        private float damageInterval = 3f;  // Interval until next damage
        [Networked] private TickTimer damageTimer { get; set; } // Timer to countdown next damage

        [SerializeField] public Slider hPSlider;
        [Networked] [OnChangedRender(nameof(HandleHpChanged))]
        public int Hp { get; set; }
        private int maxHp = 50;

        [SerializeField] private float moveSpeed;   // 1
        [SerializeField] private float range;       // 0.3
        [SerializeField] private float maxDistance; // 3
        private Vector2 wayPoint;
        private bool patrolAlongXAxis;
        private float patrolInterval = 5f;
        [Networked] private TickTimer patrolTimer { get; set; }

        public PlayerDetection playerDetection;

        [SerializeField] private Spawner spawner;


        #region - Initialize -
        private void Start()
        {
            spawner = FindObjectOfType<Spawner>();
        }

        public override void Spawned() 
        {
            var enemyTransform = GameObject.Find("Enemy");
            transform.SetParent(enemyTransform.transform, false);

            Init(enemyID);
            GamePlayManager.Instance.enemyList.Add(this);

            Hp = maxHp;
            SetEnemyHPSlider_RPC(maxHp);

            damageTimer = TickTimer.CreateFromSeconds(Runner, damageInterval);
            patrolTimer = TickTimer.CreateFromSeconds(Runner, patrolInterval);

            // Pick an axis to patrol
            patrolAlongXAxis = Random.Range(0, 2) == 0 ? true : false;
            SetNewDestination();
        }

        public void Init(int enemyID)
        {
            enemyType = (EnemyType) enemyID;
            spriteResolver.SetCategoryAndLabel("enemy", enemyType.ToString());

            SetEnemyID_RPC(enemyID);
            SetPropertiesBasedOnType();
        }

        private void SetPropertiesBasedOnType()
        {
            switch (enemyType)
            {
                case EnemyType.HighDamage:
                    directDamage = 20;
                    damageOverTime = 10;
                    maxHp = 40;
                    Hp = maxHp;
                    break;
                case EnemyType.HighHP:
                    maxHp = 80;
                    Hp = maxHp;
                    break;
                case EnemyType.HighSpeed:
                    moveSpeed = 1.5f;
                    damageOverTime = 0;
                    maxHp = 30;
                    Hp = maxHp;
                    break;
                case EnemyType.Normal:
                    // Keep default values
                    break;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetEnemyID_RPC(int enemyID)
        {
            this.enemyID = enemyID;
		}
        #endregion

        #region - Patrol & Player Detect -
        public override void FixedUpdateNetwork()
        {
            Vector2 currentPosition = transform.position;
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
                if (Vector2.Distance(currentPosition, wayPoint) <= range || patrolTimer.Expired(Runner))
                {
                    SetNewDestination();
                }

                // Calculate direction from transform to destination
                direction = (wayPoint - currentPosition).normalized;
            }

            // Move enemy only if distance > range
            if (Vector2.Distance(currentPosition, wayPoint) > range)
            {
                enemyNetworkRigidbody.Rigidbody.velocity = direction * moveSpeed;
            }
            else
            {
                enemyNetworkRigidbody.Rigidbody.velocity = Vector2.zero;
            }
        }

        private void SetNewDestination()
        {
            Vector2 currentPosition = transform.position;
            Vector2 newWayPoint;

            do
            {
                if (patrolAlongXAxis)
                {
                    float newX = Random.Range(currentPosition.x - maxDistance, currentPosition.x + maxDistance);
                    newWayPoint = new Vector2(newX, currentPosition.y);
                }
                else
                {
                    float newY = Random.Range(currentPosition.y - maxDistance, currentPosition.y + maxDistance);
                    newWayPoint = new Vector2(currentPosition.x, newY);
                }
            } while (Vector2.Distance(currentPosition, newWayPoint) <= range * 2);

            wayPoint = newWayPoint;
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
            else if (!collision.collider.CompareTag("Player"))
            {
                SetNewDestination();
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

        #region - Hp & Drop -
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
        public void SetEnemyHPSlider_RPC(int maxHp)
        {
            hPSlider.maxValue = maxHp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetEnemyHP_RPC(int hp)
        {
            Hp = hp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DespawnEnemy_RPC(NetworkObject netObj)
        {
            float randomValue = Random.value; // Random float of 0-1
            if (randomValue < 0.6f)
            {
                // 60% prob. to drop 0-3 item when enemy died
                spawner.SpawnItemWhenEnemyDied(netObj.transform);
            }

            Runner.Despawn(netObj);
        }
        #endregion
    }
}