using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

using DEMO.Item;

namespace DEMO.Manager
{
    public class ItemSpawnerManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner runner;
        [SerializeField] private int itemCount = 500; // Number of items to spawn
        [SerializeField] private GameObject itemsContainer; // Container for spawned items

        private void Start()
        {
            runner.AddCallbacks(this);
            Debug.Log("Runner Add Callbacks.");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("Runner in sceneload.");
            // Ensure items are only spawned by the shared mode master
            if (runner.IsServer)
            {
                Debug.Log("Runner is server.");
                SpawnItems();
            }
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
            ItemClass.ItemType randomItemType = (ItemClass.ItemType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ItemClass.ItemType)).Length);
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
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomY = UnityEngine.Random.Range(minY, maxY);

            // Return the random position
            return new Vector3(randomX, randomY, 0);
        }

        #region /-- Unused Function --/
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
            public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
            public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
            public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
            public void OnInput(NetworkRunner runner, NetworkInput input){}
            public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
            public void OnConnectedToServer(NetworkRunner runner){}
            public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}
            public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,byte[] token){}
            public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
            public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
            public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
            public void OnSceneLoadDone(NetworkRunner runner){}
        #endregion
    }
}