using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;
using DEMO.GamePlay.Player;
using Unity.Jobs.LowLevel.Unsafe;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D enemyNetworkRigidbody = null;
    private int directDamage = 20;
    private int damageOverTime = 5;
    private float damageInterval = 3f;  // Interval until next damage
    private float damageTimer;  // Timer to countdown next damage

    [SerializeField] public Slider hPSlider;

    [Networked] [OnChangedRender(nameof(HandleHpChanged))]
    public int Hp { get; set; }
    private int maxHp = 50;

    [SerializeField] private float moveSpeed;   // 1
    [SerializeField] private float range;       // 1
    [SerializeField] private float maxDistance; // 4
    private Vector2 wayPoint;
    private bool patrolAlongXAxis;
    private float lastDestinationChangeTime;

    // Initialize
    public override void Spawned() 
    {
        Hp = maxHp;

        damageTimer = damageInterval;

        // Pick an axis to patrol
        patrolAlongXAxis = Random.Range(0, 2) == 0 ? true : false;
        SetNewDestination();
        // Set lastDestinationChangeTime
        lastDestinationChangeTime = Time.time;
    }

    public override void FixedUpdateNetwork()
    { 
        if(Vector2.Distance(transform.position, wayPoint) < range || Time.time - lastDestinationChangeTime > 2f)
        {
            SetNewDestination();
            // Update lastDestinationChangeTime
            lastDestinationChangeTime = Time.time;
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


    #region - Damage to Player - 
    public void OnCollisionEnter2D(Collision2D collision)
    {
        var netObj = collision.collider.GetComponent<NetworkBehaviour>().Object;

        if (netObj == null) return;

        var player = collision.collider.GetComponent<PlayerController>();

        if (player != null && netObj.InputAuthority != Object.InputAuthority)
        {
            if(netObj.CompareTag("Player"))
            {
                player.TakeDamage(directDamage);
            }
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        var netObj = collision.collider.GetComponent<NetworkBehaviour>().Object;

        if (netObj == null) return;

        var player = collision.collider.GetComponent<PlayerController>();

        if (player != null && netObj.InputAuthority != Object.InputAuthority)
        {
            if(netObj.CompareTag("Player"))
            {
                damageTimer -= Time.deltaTime;
                if(damageTimer <= 0f)
                {
                    player.TakeDamage(damageOverTime);

                    // Reset timer
                    damageTimer = damageInterval;
                }
                
            }
        }
    }
    #endregion

    #region - Hp -
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        SetEnemyHP_RPC(Hp);
        if (Hp <= 0)
        {
            DespawnEnemy_RPC(Object);
        }
    }

    public void HandleHpChanged()
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
