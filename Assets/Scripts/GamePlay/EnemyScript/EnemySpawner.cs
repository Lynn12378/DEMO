using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace DEMO.GamePlay.EnemyScript
{
    public class EnemySpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject enemy;
        [SerializeField] private int _initialEnemyCount = 50;
        [SerializeField] private int _enemyPerSpawn = 20; // The number of items to spawn after each delay.
        [SerializeField] private float _delayBetweenSpawns = 300.0f;  // The delay between each spawn after the initial spawn. // 5 minutes in seconds
        private bool _initialSpawnCompleted = false; // Flag to indicate whether initial spawning is done.

        public void StartEnemySpawner()
            {
                SpawnInitialEnemy();
                InvokeRepeating("SpawnDelayedEnemy", _delayBetweenSpawns, _delayBetweenSpawns);
            }

            private void SpawnInitialEnemy()
            {
                for (int i = 0; i < _initialEnemyCount; i++)
                {
                    SpawnEnemy();
                }

                _initialSpawnCompleted = true;
            }

            private void SpawnDelayedEnemy()
            {
                if (!_initialSpawnCompleted) return;

                for (int i = 0; i < _enemyPerSpawn; i++)
                {
                    SpawnEnemy();
                }
            }

            private void SpawnEnemy()
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();

                Runner.Spawn(enemy, spawnPosition, Quaternion.identity);
            }

            private Vector3 GetRandomSpawnPosition()
            {
                // Define the boundaries of your map
                float minX = -83f;
                float maxX = 164f;
                float minY = -83f;
                float maxY = 45f;

                // Generate random coordinates within the boundaries
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);

                // Return the random position
                return new Vector3(randomX, randomY, 0);
            }
    }
}
