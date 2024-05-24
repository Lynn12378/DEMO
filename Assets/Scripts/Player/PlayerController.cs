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
        private Shelter currentShelter = null; // 当前接触到的 Shelter 对象

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
                if (!EventSystem.current.IsPointerOverGameObject())
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

            if (pressed.IsSet(InputButtons.REPAIR) && currentShelter != null && currentShelter.IsPlayerInRange())
            {
                currentShelter.Repair(20);
            }

            if (pressed.IsSet(InputButtons.REBLOOD) && currentShelter != null && currentShelter.IsPlayerInRange())
            {
                HealPlayer(20); // 调用恢复血量的方法
            }

        }

        private void HealPlayer(int amount)
        {
            // 确保 playerStats 和 health 逻辑的正确性
            playerStats.Heal(amount);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Shelter"))
            {
                currentShelter = other.GetComponent<Shelter>();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Shelter"))
            {
                currentShelter = null;
            }
        }
    }
}
