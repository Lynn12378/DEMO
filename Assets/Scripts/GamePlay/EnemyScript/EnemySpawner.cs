using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace DEMO.GamePlay.EnemyScript
{
    public class EnemySpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject enemy;
        [SerializeField] private int _initialEnemyCount = 20;
        [SerializeField] private int _enemyPerSpawn = 10; // The number of items to spawn after each delay.
        [SerializeField] private float _delayBetweenSpawns = 900.0f;  // The delay between each spawn after the initial spawn. // 15 minutes in seconds
        private bool _initialSpawnCompleted = false; // Flag to indicate whether initial spawning is done.

        private List<Transform> spawnPoints;

        private void Start()
        {
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
            spawnPoints = new List<Transform>();

            foreach (var spawnPoint in spawnPointObjects)
            {
                spawnPoints.Add(spawnPoint.transform);
            }
        }

        public void StartEnemySpawner()
        {
            SpawnInitialEnemy();
            InvokeRepeating("SpawnDelayedEnemy", _delayBetweenSpawns, _delayBetweenSpawns);
        }

        private void SpawnInitialEnemy()
        {
            int enemyPerSpawnPoint = Mathf.CeilToInt((float)_initialEnemyCount / spawnPoints.Count);

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < enemyPerSpawnPoint; i++)
                {
                    SpawnItemNearSpawnPoint(spawnPoint);
                }
            }

            _initialSpawnCompleted = true;
        }

        private void SpawnDelayedEnemy()
        {
            if (!_initialSpawnCompleted) return;

            int enemyPerSpawnPoint = Mathf.CeilToInt((float)_enemyPerSpawn / spawnPoints.Count / (_delayBetweenSpawns / 60)); // Adjusted for the delay

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < enemyPerSpawnPoint; i++)
                {
                    SpawnItemNearSpawnPoint(spawnPoint);
                }
            }
        }

        private void SpawnItemNearSpawnPoint(Transform spawnPoint)
        {
            Vector3 spawnPosition = GetSpawnPosition(spawnPoint);

            Runner.Spawn(enemy, spawnPosition, Quaternion.identity);
        }

        private Vector3 GetSpawnPosition(Transform spawnPoint)
        {
            // Define the offset range
            float offsetRange = 3.0f;

            // Generate random offsets
            float randomOffsetX = Random.Range(-offsetRange, offsetRange);
            float randomOffsetY = Random.Range(-offsetRange, offsetRange);

<<<<<<< HEAD
            // Apply a fixed upward offset for enemies
            float upwardOffset = 31.0f; // Add this because enemy's Transform always goes wrong
            randomOffsetY += upwardOffset;

=======
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
            // Return the position near the spawn point
            return new Vector3(
                spawnPoint.position.x + randomOffsetX,
                spawnPoint.position.y + randomOffsetY,
                spawnPoint.position.z
            );
        }
    }
}
