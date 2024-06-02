using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace DEMO.Item
{
    public class ItemSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkRunner runner = null;

        [SerializeField] private NetworkPrefabRef _itemBullet = NetworkPrefabRef.Empty;
        [SerializeField] private NetworkPrefabRef _itemCoin = NetworkPrefabRef.Empty;
        [SerializeField] private NetworkPrefabRef _itemFood = NetworkPrefabRef.Empty;
        [SerializeField] private NetworkPrefabRef _itemHealth = NetworkPrefabRef.Empty;
        [SerializeField] private NetworkPrefabRef _itemWood = NetworkPrefabRef.Empty; 

        // The initial number of items to spawn.
        [SerializeField] private int _initialItemCount = 300;

        // The number of items to spawn after each delay.
        [SerializeField] private int _itemsPerSpawn = 20;

        // The delay between each spawn after the initial spawn.
        [SerializeField] private float _delayBetweenSpawns = 300.0f; // 5 minutes in seconds


        private List<NetworkId> _itemSpawned = new List<NetworkId>();

        // Flag to indicate whether initial spawning is done.
        private bool _initialSpawnCompleted = false;

        // The spawner is started when the GameStateController switches to GameState.Running.
        public void StartItemSpawner()
        {
            // Start initial spawning
            SpawnInitialItems();

            // Start delayed spawning
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
            Vector3 spawnPosition = GetRandomSpawnPosition();

            NetworkPrefabRef randomItemPrefab = ChooseRandomItemPrefab();

            var itemSpawned = runner.Spawn(randomItemPrefab, spawnPosition, Quaternion.identity, PlayerRef.None);
            _itemSpawned.Add(itemSpawned.Id);
        }

        private NetworkPrefabRef ChooseRandomItemPrefab()
        {
            // Array of available item prefabs
            NetworkPrefabRef[] itemPrefabs = new NetworkPrefabRef[] { _itemHealth, _itemWood, _itemBullet, _itemCoin, _itemFood };

            // Randomly choose an index
            int randomIndex = Random.Range(0, itemPrefabs.Length);

            // Return the chosen item prefab
            return itemPrefabs[randomIndex];
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