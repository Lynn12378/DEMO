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
        [SerializeField] private int initialEnemyCount = 80;
        [SerializeField] private int enemyPerSpawn = 10;
        [SerializeField] private float delayBetweenEnemySpawns = 900.0f; // 15 minutes in seconds

        [Header("Item Spawner Settings")]
        [SerializeField] private NetworkObject item;
        [SerializeField] private int initialItemCount = 350;
        [SerializeField] private int itemPerSpawn = 20;
        [SerializeField] private float delayBetweenItemSpawns = 300.0f; // 5 minutes in seconds

        [Header("Living Spawner Settings")]
        [SerializeField] private NetworkObject living;
        [SerializeField] private int initialLivingCount = 15;
        [SerializeField] private int livingPerSpawn = 5;
        [SerializeField] private float delayBetweenLivingSpawns = 600.0f; // 10 minutes in seconds
        public BoxCollider2D spawnGooseArea;

        [Header("Shop Spawner Settings")]
        [SerializeField] private NetworkObject shop;
        [SerializeField] private int maxShopCount = 2;
        [SerializeField] private float shopLifetime = 300.0f; // 5 minutes in seconds

        private bool initialEnemySpawnCompleted = false;
        private bool initialItemSpawnCompleted = false;
        private bool initialLivingSpawnCompleted = false;

        private List<Transform> spawnPoints;
        private List<NetworkObject> currentShops = new List<NetworkObject>();

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
            SpawnInitialObjects(enemy, initialEnemyCount, enemyPerSpawn, SpawnEnemyNearSpawnPoint);
            SpawnInitialObjects(item, initialItemCount, itemPerSpawn, SpawnItemNearSpawnPoint);
            SpawnInitialObjects(living, initialLivingCount, livingPerSpawn, SpawnLivingNearSpawnPoint);

            InvokeRepeating(nameof(SpawnDelayedEnemies), delayBetweenEnemySpawns, delayBetweenEnemySpawns);
            InvokeRepeating(nameof(SpawnDelayedItems), delayBetweenItemSpawns, delayBetweenItemSpawns);
            InvokeRepeating(nameof(SpawnDelayedLivings), delayBetweenLivingSpawns, delayBetweenLivingSpawns);
            InvokeRepeating(nameof(SpawnShops), 0f, shopLifetime);
        }
        #endregion

        #region - Spawn Shops -
        private void SpawnShops()
        {
            // Destroy existing shops
            foreach (var shop in currentShops)
            {
                Runner.Despawn(shop);
            }
            currentShops.Clear();

            // Create a temporary list to track available spawn points
            List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

            // Spawn new shops
            for (int i = 0; i < maxShopCount; i++)
            {
                // Select a random spawn point and remove it from the list
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                var spawnPoint = availableSpawnPoints[spawnIndex];
                availableSpawnPoints.RemoveAt(spawnIndex);

                // Spawn the shop
                var newShop = Runner.Spawn(shop, GetSpawnPosition(spawnPoint), Quaternion.identity);
                currentShops.Add(newShop);
            }
        }
        #endregion

        #region - Spawn Initial -
        private void SpawnInitialObjects(NetworkObject prefab, int initialCount, int perSpawn, System.Action<Transform> spawnAction)
        {
            int perSpawnPoint = Mathf.CeilToInt((float)initialCount / spawnPoints.Count);

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < perSpawnPoint; i++)
                {
                    spawnAction(spawnPoint);
                }
            }

            if (prefab == enemy) initialEnemySpawnCompleted = true;
            if (prefab == item) initialItemSpawnCompleted = true;
            if (prefab == living) initialLivingSpawnCompleted = true;
        }
        #endregion

        #region - Spawn Delayed -
        private void SpawnDelayedObjects(NetworkObject prefab, int perSpawn, float delay, System.Action<Transform> spawnAction, bool initialSpawnCompleted)
        {
            if (!initialSpawnCompleted) return;

            int perSpawnPoint = Mathf.CeilToInt((float)perSpawn / spawnPoints.Count / (delay / 60));

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < perSpawnPoint; i++)
                {
                    spawnAction(spawnPoint);
                }
            }
        }

        private void SpawnDelayedEnemies() => SpawnDelayedObjects(enemy, enemyPerSpawn, delayBetweenEnemySpawns, SpawnEnemyNearSpawnPoint, initialEnemySpawnCompleted);
        private void SpawnDelayedItems() => SpawnDelayedObjects(item, itemPerSpawn, delayBetweenItemSpawns, SpawnItemNearSpawnPoint, initialItemSpawnCompleted);
        private void SpawnDelayedLivings() => SpawnDelayedObjects(living, livingPerSpawn, delayBetweenLivingSpawns, SpawnLivingNearSpawnPoint, initialLivingSpawnCompleted);
        #endregion

        #region - Spawn Near Spawn Points- 
        private void SpawnEnemyNearSpawnPoint(Transform spawnPoint)
        {
            int enemyID = Random.Range(0, 4);
            Vector3 spawnPosition = GetSpawnPosition(spawnPoint);

            var NO = Runner.Spawn(enemy, spawnPosition, Quaternion.identity);
            NO.GetComponent<EnemyScript.Enemy>().Init(enemyID);
        }

        private void SpawnItemNearSpawnPoint(Transform spawnPoint)
        {
            int itemID = GetRandomItemID();
            Vector3 spawnPosition = GetSpawnPosition(spawnPoint);

            var NO = Runner.Spawn(item, spawnPosition, Quaternion.identity);
            NO.GetComponent<Inventory.Item>().Init(itemID);
        }

        private void SpawnLivingNearSpawnPoint(Transform spawnPoint)
        {
            Vector3 spawnPosition;

            int livingID = Random.Range(0, 11);

            if(livingID == 8)
            {
                spawnPosition = GetSpawnPositionInArea();
            }
            else
            {
                spawnPosition = GetSpawnPosition(spawnPoint);
            }
            
            var NO = Runner.Spawn(living, spawnPosition, Quaternion.identity);
            NO.GetComponent<Livings>().Init(livingID);
        }

        public void SpawnItemAround(Transform spawnPoint, int itemCount)
        {
            int itemID = GetRandomItemID();

            for (int i = 0; i < itemCount; i++)
            {
                Vector3 spawnPosition = GetCloserSpawnPosition(spawnPoint);

                var NO = Runner.Spawn(item, spawnPosition, Quaternion.identity);
                NO.GetComponent<Inventory.Item>().Init(itemID);
            }
        }

        private int GetRandomItemID()
        {
            float randomValue = Random.value; // Random float of 0-1
            if (randomValue < 0.8f)
            {
                // 80% prob. to spawn item 0-4
                return Random.Range(0, 5);
            }
            else
            {
                // 20% prob. to spawn item 5-13
                return Random.Range(5, 14);
            }
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

        private Vector3 GetCloserSpawnPosition(Transform spawnPoint)
        {
            float offsetRange = 1.0f;
            float randomOffsetX = Random.Range(-offsetRange, offsetRange);
            float randomOffsetY = Random.Range(-offsetRange, offsetRange);

            return new Vector3(
                spawnPoint.position.x + randomOffsetX,
                spawnPoint.position.y + randomOffsetY,
                spawnPoint.position.z
            );
        }

        private Vector3 GetSpawnPositionInArea()
        {
            Bounds bounds = spawnGooseArea.bounds;

            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                transform.position.z
            );

            return randomPosition;
        }
        #endregion
    }
}