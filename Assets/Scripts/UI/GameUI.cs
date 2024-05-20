using System.Collections;
using System.Collections.Generic;
using DEMO.Player;
using DEMO;
using UnityEngine;
using Fusion;
using ExitGames.Client.Photon.StructWrapping;

public class GameUI : NetworkBehaviour
{
    [SerializeField] private CanvasUI canvasUI = null;
    [SerializeField] private InventoryUI inventoryUI = null;
    private NetworkObject playerObject = null;
    
    public void Initialize()
    {
        playerObject = Runner.GetPlayerObject(Runner.LocalPlayer);
        GameManager.Instance.OnPlayerUIUpdated += UpdatePlayerUI;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerUIUpdated -= UpdatePlayerUI;
    }

    public void UpdatePlayerUI()
    {
        foreach(var player in GameManager.Instance.playerList)
        {
            if(player.Key == playerObject.InputAuthority)
            {
                var playerRef = player.Key;
                var playerData = player.Value;

                canvasUI.UpdateUI(playerRef, playerData);
                Debug.Log("Update UI for "+ playerRef + " with playerData: " + playerData);
            }
        }
    }

    /*public void Initialize()
    {
        if(HasStateAuthority)
        {
            playerObject = Runner.GetPlayerObject(Runner.LocalPlayer);
            if (playerObject != null)
            {
                var playerHealth = playerObject.GetComponent<PlayerController>().currentHealth;
                var playerBullet = playerObject.GetComponent<PlayerAttackHandler>().currentBullet;

                canvasUI.SetCanvasHealth(playerHealth);
                canvasUI.SetCanvasBullet(playerBullet);
                canvasUI.gameObject.SetActive(true);

                inventoryUI.gameObject.SetActive(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if(HasStateAuthority)
        {
            if (playerObject != null)
            {
                var playerHealth = playerObject.GetComponent<PlayerController>().currentHealth;
                canvasUI.SetCanvasHealth(playerHealth);

                var playerBullet = playerObject.GetComponent<PlayerAttackHandler>().currentBullet;
                canvasUI.SetCanvasBullet(playerBullet);
            }
        }
    }*/
}
