using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Fusion;
using Fusion.Addons.Physics;
using Unity.VisualScripting;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D enemyNetworkRigidbody = null;
    private int maxHp = 50;
    private HealthPoint healthPoint = null;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float range;
    [SerializeField] private float maxDistance;
    private Vector2 wayPoint;
    private bool patrolAlongXAxis;
    private float lastDestinationChangeTime;

    // Initialize
    public override void Spawned() 
    {
        healthPoint = GetComponentInChildren<HealthPoint>();
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

    void SetNewDestination()
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
}

