using UnityEngine;
using DEMO.Item;
using Fusion;

namespace DEMO.Manager
{
    public class ItemSpawnerManager : MonoBehaviour
    {
        [SerializeField] private NetworkRunner runner;
        [SerializeField] private int itemCount = 500; // Number of items to spawn
        [SerializeField] private GameObject itemsContainer; // Container for spawned items

        // Random seed for generating random numbers
        private int randomSeed = 23;

        private void Awake()
        {
            runner = GetComponent<NetworkRunner>();
            // Set random seed
            Random.InitState(randomSeed);
        }

        public void SpawnItems()
        {
            for (int i = 0; i < itemCount; i++)
            {
                SpawnRandomItem();
            }
        }

        private void SpawnRandomItem()
        {
            // Generate a random item type
            ItemClass.ItemType randomItemType = (ItemClass.ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemClass.ItemType)).Length);
            ItemClass newItem = new ItemClass { itemType = randomItemType, amount = 1 };

            // Select the item prefab from ItemAssets
            GameObject itemPrefab = ItemAssets.Instance.pfItemWorld;
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // Spawn the item and synchronize it across all clients
            NetworkObject networkObject = runner.Spawn(itemPrefab, spawnPosition, Quaternion.identity);

            // Initialize the ItemWorld with the new item
            ItemWorld itemWorld = networkObject.GetComponent<ItemWorld>();
            itemWorld.SetItem(newItem);

            // Parent the spawned item to the container GameObject
            networkObject.transform.SetParent(itemsContainer.transform);
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