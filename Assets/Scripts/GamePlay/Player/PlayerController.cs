using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.DB;
using DEMO.Manager;

namespace DEMO.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerNetworkData playerNetworkData;
        [SerializeField] private Shelter shelter;

        private UIManager uIManager;
        private GameObject obj;
        private NetworkButtons buttonsPrevious;

        private bool isInShelter = false;
        private float shelterTimer = 0f;
        private const float SHELTER_HEAL_INTERVAL = 5f;
        private const int HEAL_AMOUNT = 10;
        private const int MAX_HP = 100; 

        public override void Spawned()
        {
            uIManager = FindObjectOfType<UIManager>();
            if (uIManager == null)
            {
                Debug.LogError("UIManager not found!");
            }
            else
            {
                playerNetworkData.SetUIManager(uIManager);
            }
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

            if (isInShelter)
            {
                shelterTimer += Runner.DeltaTime; // 使用 Runner.DeltaTime 來計算時間，確保網路同步

                if (shelterTimer >= SHELTER_HEAL_INTERVAL)
                {
                    HealPlayer();
                    shelterTimer = 0f; // 重置計時器
                }
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
                if (playerNetworkData.bulletAmount > 0)
                {
                    attackHandler.Shoot(data.mousePosition);
                    playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount - 1);
                }
            }

            if (pressed.IsSet(InputButtons.TESTDAMAGE))
            {
                playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - 10);
                Debug.Log(playerNetworkData.HP);
            }

            if (pressed.IsSet(InputButtons.REPAIR) && isInShelter)
            {
                Debug.Log("Repairing...");
                if (shelter != null)
                {
                    shelter.RepairDurability_RPC();
                }
            }
        }

        private void HealPlayer()
        {
            int newHP = Mathf.Min(playerNetworkData.HP + HEAL_AMOUNT, MAX_HP); // 確保血量不超過最大值
            playerNetworkData.SetPlayerHP_RPC(newHP);
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Shelter"))
            {
                Debug.Log("Entered Shelter");
                isInShelter = true;
                shelter = collider.GetComponent<Shelter>();
                if (shelter == null)
                {
                    Debug.LogError("Shelter component not found on the collided object!");
                }
            }
        }

        public void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Shelter"))
            {
                Debug.Log("Exited Shelter");
                isInShelter = false;
                shelterTimer = 0f;
                shelter = null;
            }
        }
    }
}
