using System.Collections.Generic;
using System;
using UnityEngine;

using Fusion;
using Fusion.Sockets;
using DEMO.Player;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public GameObject GameUIPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            NetworkObject networkPlayerObject = Runner.Spawn(PlayerPrefab, Vector3.zero, Quaternion.identity);
            Runner.SetPlayerObject(player, networkPlayerObject);

            // Tie GameUI to player, without changing view of canvas
            GameObject playerCanvas = Instantiate(GameUIPrefab);
            GameUI gameUI = playerCanvas.GetComponent<GameUI>();
            gameUI.Initialize(networkPlayerObject);
        }
    }
}