using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager instance;
    [SerializeField] private InventoryUI inventoryUI = null;
    [SerializeField] private InventorySlot inventorySlot = null;

    private Dictionary<PlayerRef, Inventory> playerInventories = new Dictionary<PlayerRef, Inventory>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeInventory()
    {
        inventoryUI.Initialize();
        inventorySlot.Initialize();
    }

    public void SetPlayerInventory(PlayerRef player, Inventory inventory)
    {
        if (!playerInventories.ContainsKey(player))
        {
            playerInventories.Add(player, inventory);
        }
        else
        {
            playerInventories[player] = inventory;
        }
    }

    public Inventory GetPlayerInventory(PlayerRef player)
    {
        if (playerInventories.ContainsKey(player))
        {
            return playerInventories[player];
        }
        else
        {
            Debug.LogWarning("Player inventory not found for player: " + player);
            return null;
        }
    }
}
