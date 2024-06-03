using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.DB;
using DEMO.Manager;
using DEMO.Item;

namespace DEMO.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerInventory playerInventory = null;
        [SerializeField] private PlayerNetworkData playerNetworkData;

        private UIManager uIManager;
        private GameObject obj;
        private NetworkButtons buttonsPrevious;
        private bool isPickupKeyPressed = false;
        private ItemClass itemInRange = null;

        public override void Spawned()
        {
            uIManager = FindObjectOfType<UIManager>();
            playerNetworkData.SetUIManager(uIManager);
        }

        private void Respawn() 
        {
            transform.position = Vector3.zero;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

            if(playerNetworkData.HP <= 0)
            {
                Respawn();
            }

            // Handle pickup in here, otherwise not synchronized
            if (isPickupKeyPressed && itemInRange != null)
            {
                playerInventory.PickUpItem(itemInRange);
                itemInRange = null;
                isPickupKeyPressed = false;

                Debug.Log("Inventory for player " + playerNetworkData.playerRef + " : " + playerInventory.GetInventoryItemsAsString());
            }
        }

        private void ApplyInput(NetworkInputData data)
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(buttonsPrevious);
            buttonsPrevious = buttons;

            movementHandler.Move(data);
            movementHandler.SetRotation(data.mousePosition);

            if (pressed.IsSet(InputButtons.FIRE))
            {
                if(playerNetworkData.bulletAmount > 0)
                {
                    attackHandler.Shoot(data.mousePosition);
                    playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount - 1);
                }
                else
                {
                    Debug.Log("Not enough bullet!");
                }
            }

            if (pressed.IsSet(InputButtons.TESTDAMAGE))
            {
                playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - 10);
            }

            if (pressed.IsSet(InputButtons.PICKUP))
            {
                isPickupKeyPressed = true;
            }
            else
            {
                // Reset state
                isPickupKeyPressed = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("ItemsInteractable"))
            {
                ItemClass item = collider.GetComponent<ItemClass>();
                if (item != null)
                {
                    itemInRange = item; // Set item in range
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("ItemsInteractable"))
            {
                ItemClass item = collider.GetComponent<ItemClass>();
                if (item != null && item == itemInRange)
                {
                    itemInRange = null; // Reset item in range
                }
            }
        }
    }
}

