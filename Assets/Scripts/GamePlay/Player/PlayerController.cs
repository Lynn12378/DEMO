using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.DB;
using DEMO.Manager;
using DEMO.GamePlay.Inventory;

namespace DEMO.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerNetworkData playerNetworkData;

        private UIManager uIManager;
        private NetworkButtons buttonsPrevious;
        [SerializeField] private Item itemInRange = null;

        public override void Spawned()
        {
            uIManager = FindObjectOfType<UIManager>();
            playerNetworkData.SetUIManager(uIManager);
            uIManager.InitializeMinimapArrow(transform);
        }

        private void Respawn() 
        {
            transform.position = Vector3.zero;

            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.MaxHP);
            playerNetworkData.bulletAmount = playerNetworkData.MaxBullet;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }
        }

        private async void ApplyInput(NetworkInputData data)
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

            if (pressed.IsSet(InputButtons.PICKUP))
            {
                if(itemInRange == null){return;}

                var item = itemInRange.GetComponent<Item>();

                // If item is coin, then just add to coinAmount
                if(item.itemType == Item.ItemType.Coin)
                {
                    playerNetworkData.SetPlayerCoin_RPC(playerNetworkData.coinAmount + 10);
                    itemInRange.DespawnItem_RPC();
                }

                // If item not coin and enough space    
                if (playerNetworkData.itemList.Count < 12 && item.itemType != Item.ItemType.Coin)
                {
                    playerNetworkData.itemList.Add(item);
                    playerNetworkData.UpdateItemList();

                    itemInRange.DespawnItem_RPC();
                }
                else if(playerNetworkData.itemList.Count >= 12)
                {
                    Debug.Log("Inventory is full, cannot pick up item.");
                }
            }

            if (pressed.IsSet(InputButtons.TALK))
            {
                
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Item"))
            {
                itemInRange = collider.GetComponent<Item>();
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Item"))
            {
                itemInRange = null;
            }
        }

        public void TakeDamage(int damage)
        {
            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - damage);
            if(playerNetworkData.HP <= 0)
            {
                Respawn();
            }
        }
    }
}

