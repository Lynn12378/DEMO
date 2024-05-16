using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Fusion;
using Fusion.Addons.Physics;

namespace DEMO.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerStats playerStats = null;
        private bool isPickupKeyPressed = false;


        [Networked] private NetworkButtons buttonsPrevious { get; set; }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
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
                if(!EventSystem.current.IsPointerOverGameObject())
                {
                    attackHandler.Shoot(data.mousePosition);
                }
            }

            if (pressed.IsSet(InputButtons.TESTDAMAGE))
            {
                playerStats.TakeDamage(20);
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

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.CompareTag("ItemsInteractable") && isPickupKeyPressed)
            {
                ItemPickup itemPickup = collider.GetComponent<ItemPickup>();
                ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
                Item item = itemWorld.GetItem();

                if (itemPickup != null)
                {
                    Debug.Log(GameManager.Instance.Runner.LocalPlayer);
                    itemPickup.PickUp(GameManager.Instance.Runner.LocalPlayer, item);
                }
            }
        }
    }
}

