using UnityEngine;
using Fusion;

using DEMO.Manager;
using DEMO.UI;
using System.Linq;

namespace DEMO.GamePlay.Player
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField] public GameObject playerPrefab;
        [SerializeField] public ItemSpawnerManager itemSpawnerManager;

        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                SpawnPlayer(player);
            }
        }

        private void SpawnPlayer(PlayerRef player)
        {
            var spawnPosition = Vector3.zero;

            var playerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            Runner.SetPlayerObject(player, playerObject);
            
            // Use UIManager to handle the GameUI instantiation and display
            if (UIManager.Instance != null)
            {
                UIManager.Instance.InitializeGameUI(player);
            }

            if (player == Runner.LocalPlayer)
            {
                Camera.main.transform.SetParent(playerObject.transform);
            }
        }
    }
}