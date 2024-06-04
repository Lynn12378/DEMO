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
        [SerializeField] private int _itemsPerSpawn = 20; // The number of items to spawn after each delay.
        [SerializeField] private float _delayBetweenSpawns = 300.0f;  // The delay between each spawn after the initial spawn. // 5 minutes in seconds
        private bool _initialSpawnCompleted = false; // Flag to indicate whether initial spawning is done.

        public void StartItemSpawner()
        {
            SpawnInitialItems();
            InvokeRepeating("SpawnDelayedItems", _delayBetweenSpawns, _delayBetweenSpawns);
        }

        private void SpawnInitialItems()
        {
            for (int i = 0; i < _initialItemCount; i++)
            {
                SpawnRandomItem();
            }

            _initialSpawnCompleted = true;
        }

        private void SpawnDelayedItems()
        {
            if (!_initialSpawnCompleted) return;

            for (int i = 0; i < _itemsPerSpawn; i++)
            {
                SpawnRandomItem();
            }
        }

        private void SpawnRandomItem()
        {
            int itemID = Random.Range(0, 5);
            Vector3 spawnPosition = GetRandomSpawnPosition();

            var NO = Runner.Spawn(item, spawnPosition, Quaternion.identity);
            NO.GetComponent<Item>().Init(itemID);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            // Define the boundaries of your map
            float minX = -83f;
            float maxX = 164f;
            float minY = -83f;
            float maxY = 45f;

            // Generate random coordinates within the boundaries
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomY = UnityEngine.Random.Range(minY, maxY);

            // Return the random position
            return new Vector3(randomX, randomY, 0);
        }
    }
}