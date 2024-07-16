using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.GamePlay.Interactable;

namespace DEMO.GamePlay
{
    public class Spawner : NetworkBehaviour
    {
        [Header("Enemy Spawner Settings")]
        [SerializeField] private NetworkObject enemy;
        [SerializeField] private int initialEnemyCount = 25;
        [SerializeField] private int enemyPerSpawn = 10;
        [SerializeField] private float delayBetweenEnemySpawns = 900.0f; // 15 minutes in seconds

        [Header("Item Spawner Settings")]
        [SerializeField] private NetworkObject item;
        [SerializeField] private int initialItemCount = 350;
        [SerializeField] private int itemPerSpawn = 20;
        [SerializeField] private float delayBetweenItemSpawns = 300.0f; // 5 minutes in seconds

        [Header("Living Spawner Settings")]
        [SerializeField] private NetworkObject living;
        [SerializeField] private int initialLivingCount = 10;
        [SerializeField] private int livingPerSpawn = 5;
        [SerializeField] private float delayBetweenLivingSpawns = 600.0f; // 10 minutes in seconds

        private bool initialEnemySpawnCompleted = false;
        private bool initialItemSpawnCompleted = false;
        private bool initialLivingSpawnCompleted = false;

        private List<Transform> spawnPoints;

        #region - Start -
        private void Start()
        {
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
            spawnPoints = new List<Transform>();

            foreach (var spawnPoint in spawnPointObjects)
            {
                spawnPoints.Add(spawnPoint.transform);
            }
        }

        public void StartSpawners()
        {
            SpawnInitialEnemies();
            SpawnInitialItems();
            SpawnInitialLivings();

            InvokeRepeating("SpawnDelayedEnemies", delayBetweenEnemySpawns, delayBetweenEnemySpawns);
            InvokeRepeating("SpawnDelayedItems", delayBetweenItemSpawns, delayBetweenItemSpawns);
            InvokeRepeating("SpawnDelayedLivings", delayBetweenLivingSpawns, delayBetweenLivingSpawns);
        }
        #endregion

        #region - Spawn Initial -
        private void SpawnInitialEnemies()
        {
            int enemyPerSpawnPoint = Mathf.CeilToInt((float)initialEnemyCount / spawnPoints.Count);

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < enemyPerSpawnPoint; i++)
                {
                    SpawnEnemyNearSpawnPoint(spawnPoint);
                }
            }

            initialEnemySpawnCompleted = true;
        }

        private void SpawnInitialItems()
        {
            int itemsPerSpawnPoint = Mathf.CeilToInt((float)initialItemCount / spawnPoints.Count);

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < itemsPerSpawnPoint; i++)
                {
                    SpawnItemNearSpawnPoint(spawnPoint);
                }
            }

            initialItemSpawnCompleted = true;
        }

        private void SpawnInitialLivings()
        {
            for (int i = 0; i < initialLivingCount; i++)
            {
                SpawnLivingAtRandomPosition();
            }

            initialLivingSpawnCompleted = true;
        }
        #endregion

        #region - Spawn Delayed -
        private void SpawnDelayedEnemies()
        {
            if (!initialEnemySpawnCompleted) return;

            int enemyPerSpawnPoint = Mathf.CeilToInt((float)enemyPerSpawn / spawnPoints.Count / (delayBetweenEnemySpawns / 60));

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < enemyPerSpawnPoint; i++)
                {
                    SpawnEnemyNearSpawnPoint(spawnPoint);
                }
            }
        }

        private void SpawnDelayedItems()
        {
            if (!initialItemSpawnCompleted) return;

            int itemsPerSpawnPoint = Mathf.CeilToInt((float)itemPerSpawn / spawnPoints.Count / (delayBetweenItemSpawns / 60));

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < itemsPerSpawnPoint; i++)
                {
                    SpawnItemNearSpawnPoint(spawnPoint);
                }
            }
        }

        private void SpawnDelayedLivings()
        {
            if (!initialLivingSpawnCompleted) return;

            int livingsPerSpawnPoint = Mathf.CeilToInt((float)livingPerSpawn / spawnPoints.Count / (delayBetweenLivingSpawns / 60));

            for (int i = 0; i < livingsPerSpawnPoint; i++)
            {
                SpawnLivingAtRandomPosition();
            }
        }
        #endregion

        #region - Spawn - 
        private void SpawnEnemyNearSpawnPoint(Transform spawnPoint)
        {
            int enemyID = Random.Range(0, 4);
            Vector3 spawnPosition = GetSpawnPosition(spawnPoint);

            var NO = Runner.Spawn(enemy, spawnPosition, Quaternion.identity);
            NO.GetComponent<EnemyScript.Enemy>().Init(enemyID);
        }

        private void SpawnItemNearSpawnPoint(Transform spawnPoint)
        {
            int itemID = Random.Range(0, 8);
            Vector3 spawnPosition = GetSpawnPosition(spawnPoint);

            var NO = Runner.Spawn(item, spawnPosition, Quaternion.identity);
            NO.GetComponent<Inventory.Item>().Init(itemID);
        }

        private void SpawnLivingAtRandomPosition()
        {
            int livingID = Random.Range(0, 6);
            Vector3 spawnPosition = GetRandomSpawnPosition();

            var NO = Runner.Spawn(living, spawnPosition, Quaternion.identity);
            NO.GetComponent<Livings>().Init(livingID);
        }
        #endregion

        #region - Spawn positions -
        private Vector3 GetSpawnPosition(Transform spawnPoint)
        {
            float offsetRange = 5.0f;
            float randomOffsetX = Random.Range(-offsetRange, offsetRange);
            float randomOffsetY = Random.Range(-offsetRange, offsetRange);

            return new Vector3(
                spawnPoint.position.x + randomOffsetX,
                spawnPoint.position.y + randomOffsetY,
                spawnPoint.position.z
            );
        }

        private Vector3 GetRandomSpawnPosition()
        {
            float offsetX = Random.Range(-81f, 163f);
            float offsetY = Random.Range(-78f, 45f);

            return new Vector3(offsetX, offsetY, 0f);
        }
        #endregion
    }
}