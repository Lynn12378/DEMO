using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.Manager;

namespace DEMO.GamePlay.Inventory
{
    public class ItemSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject item;
        [SerializeField] private int _initialItemCount = 300;
        [SerializeField] private int _itemPerSpawn = 10; // The number of items to spawn after each delay.
        [SerializeField] private float _delayBetweenSpawns = 300.0f;  // The delay between each spawn after the initial spawn. // 5 minutes in seconds
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

        public void StartItemSpawner()
        {
            SpawnInitialItems();
            InvokeRepeating("SpawnDelayedItems", _delayBetweenSpawns, _delayBetweenSpawns);
        }

        private void SpawnInitialItems()
        {
            int itemsPerSpawnPoint = Mathf.CeilToInt((float)_initialItemCount / spawnPoints.Count);

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < itemsPerSpawnPoint; i++)
                {
                    SpawnItemNearSpawnPoint(spawnPoint);
                }
            }

            _initialSpawnCompleted = true;
        }

        private void SpawnDelayedItems()
        {
            if (!_initialSpawnCompleted) return;

            int itemsPerSpawnPoint = Mathf.CeilToInt((float)_itemPerSpawn / spawnPoints.Count / (_delayBetweenSpawns / 60)); // Adjusted for the delay

            foreach (var spawnPoint in spawnPoints)
            {
                for (int i = 0; i < itemsPerSpawnPoint; i++)
                {
                    SpawnItemNearSpawnPoint(spawnPoint);
                }
            }
        }

        private void SpawnItemNearSpawnPoint(Transform spawnPoint)
        {
            int itemID = Random.Range(0, 5);
            Vector3 spawnPosition = GetSpawnPosition(spawnPoint);

            var NO = Runner.Spawn(item, spawnPosition, Quaternion.identity);
            NO.GetComponent<Item>().Init(itemID);
        }

        private Vector3 GetSpawnPosition(Transform spawnPoint)
        {
            // Define the offset range
            float offsetRange = 5.0f;

            // Generate random offsets
            float randomOffsetX = Random.Range(-offsetRange, offsetRange);
            float randomOffsetY = Random.Range(-offsetRange, offsetRange);

            // Return the position near the spawn point
            return new Vector3(
                spawnPoint.position.x + randomOffsetX,
                spawnPoint.position.y + randomOffsetY,
                spawnPoint.position.z
            );
        }
    }
}
