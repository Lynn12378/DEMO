using System;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.DB;
using DEMO.Manager;
using DEMO.GamePlay;
using DEMO.GamePlay.Inventory;
using Photon.Voice.Unity;
using DEMO.Gameplay;
using TMPro.Examples;
using System.Collections.Generic;

namespace DEMO.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerVoiceDetection voiceDetection = null;
        [SerializeField] private PlayerNetworkData playerNetworkData;
        [SerializeField] private PlayerOutputData playerOutputData;
        private float surviveTime = 0f;

        private UIManager uIManager;
        private NetworkButtons buttonsPrevious;
        [SerializeField] private Item itemInRange = null;

        [SerializeField] private Shelter shelter;
        private bool isInShelter = false;
        private float shelterTimer = 0f;
        private const float shelterHealInterval = 5f;


        public override void Spawned()
        {
            uIManager = FindObjectOfType<UIManager>();
            playerNetworkData.SetUIManager(uIManager);
        }

        private void Respawn() 
        {
            transform.position = Vector3.zero;

            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.MaxHP);
            playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.MaxBullet);
            playerNetworkData.SetPlayerFood_RPC(playerNetworkData.MaxFood);

            surviveTime = 0f;
        }

        public override void FixedUpdateNetwork()
        {
            surviveTime += Runner.DeltaTime;
            if(surviveTime > playerOutputData.surviveTime)
            {
                playerOutputData.SetSurviveTime_RPC(surviveTime);
            }

            if(playerNetworkData.HP <= 0 || playerNetworkData.foodAmount <= 0)
            {
                playerOutputData.AddDeathNo_RPC();
                playerOutputData.SetSurviveTime_RPC(surviveTime);
                Respawn();
            }

            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

            if(playerNetworkData.playerRef == Runner.LocalPlayer)
            {
                uIManager.UpdateMinimapArrow(gameObject.transform);
            }

            if (isInShelter)
            {
                shelterTimer += Runner.DeltaTime; // Use Runner.DeltaTime to ensure synchronization

                if (shelterTimer >= shelterHealInterval)
                {
                    HealPlayer(10);
                    shelterTimer = 0f; // Reset timer
                }
            }

            voiceDetection.AudioCheck();
        }

        public PlayerVoiceDetection GetPlayerVoiceDetection()
        {
            return voiceDetection;
        }

        #region - Input -
        /* FIRE, PICKUP, TALK, RELOAD */
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
                voiceDetection.rec.TransmitEnabled = !voiceDetection.rec.TransmitEnabled;
            }

            if (pressed.IsSet(InputButtons.RELOAD) && isInShelter)
            {
                playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + 5);
            }
        }
        #endregion

        #region - On Trigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Item"))
            {
                itemInRange = collider.GetComponent<Item>();
            }

            if (collider.CompareTag("Shelter"))
            {
                isInShelter = true;
                shelter = collider.GetComponent<Shelter>();
                playerNetworkData.SetShelter(shelter);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Item"))
            {
                itemInRange = null;
            }

            if (collider.CompareTag("Shelter"))
            {
                isInShelter = false;
                shelterTimer = 0f;
                shelter = null;
                playerNetworkData.SetShelter(shelter);
            }
        }
        #endregion

        #region - OnCollision -
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("MapCollision"))
            {
                playerOutputData.collisionNo++;
                Debug.Log(playerNetworkData.playerRefString + "'s collision no. is: " + playerOutputData.collisionNo.ToString());
            }
        }
        #endregion

        #region - Player HP -
        private void HealPlayer(int healAmount)
        {
            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP + healAmount);
        }

        public void TakeDamage(int damage)
        {
            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - damage);
        }
        #endregion
    }
}

