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
        [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerStatsUI playerStatsUI = null;
        [SerializeField] private PlayerHealthPoint healthPoint = null;
        private bool isPickupKeyPressed = false;

        private int maxHealth = 100;
        private int currentHealth;


        [Networked] private NetworkButtons buttonsPrevious { get; set; }

        public override void Spawned()
        {
            currentHealth = maxHealth;
            healthPoint.Subscribe(OnHPChanged);
            healthPoint.HP = maxHealth;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            healthPoint.Unsubscribe(OnHPChanged);
        }

        // When restart
        private void Respawn() 
        {
            playerNetworkRigidbody.transform.position = Vector3.zero;

            currentHealth = maxHealth;
            healthPoint.Subscribe(OnHPChanged);
            healthPoint.HP = maxHealth;

            attackHandler.SetMaxBullet();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

            if(currentHealth <= 0)
            {
                Respawn();
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

        private void OnHPChanged(int value)
        {
            currentHealth = value;
            playerStatsUI.SetHealthUI(value);
        }
    }
}

