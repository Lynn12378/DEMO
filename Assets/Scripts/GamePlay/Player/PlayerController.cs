using System;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.DB;
using DEMO.Manager;
using DEMO.GamePlay.Inventory;
using DEMO.GamePlay.Interactable;
using DEMO.Gameplay;
using DEMO.UI;

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
        [SerializeField] private IInteractable interactableInRange = null;
        private bool isInteracting = false;

        [SerializeField] private Shelter shelter;
        private bool isInShelter = false;
        private float shelterTimer = 0f;
        private const float shelterHealInterval = 5f;


        public override void Spawned()
        {
            uIManager = FindObjectOfType<UIManager>();
            playerNetworkData.SetUIManager(uIManager);

            attackHandler.Init(this);
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

        #region - Getter -
        public PlayerVoiceDetection GetPlayerVoiceDetection()
        {
            return voiceDetection;
        }

        public PlayerNetworkData GetPlayerNetworkData()
        {
            return playerNetworkData;
        }
        #endregion

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

            if (pressed.IsSet(InputButtons.SPACE)) // Spacebar
            {
                if(itemInRange != null)
                {
                    Pickup();
                }
                else if(interactableInRange != null && isInteracting == false)
                {
                    Interact();
                    isInteracting = true;
                }
                else if (interactableInRange != null && isInteracting)
                {
                    EndInteract();
                }
                else
                {
                    return;
                }          
            }

            if (pressed.IsSet(InputButtons.FEED))
            {
                if (isInteracting && MapInteractionManager.Instance.currentInteraction.interactionType == InteractionType.Feed)
                {
                    MapInteractionManager.Instance.Feed(playerNetworkData, playerOutputData);
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

        #region - Pickup Item -
        private void Pickup()
        {
            var item = itemInRange.GetComponent<Item>();

            // If item is coin, then just add to coinAmount
            if(item.itemType == Item.ItemType.Coin)
            {
                playerNetworkData.SetPlayerCoin_RPC(playerNetworkData.coinAmount + 10);
                AudioManager.Instance.Play("Pickup");
                itemInRange.DespawnItem_RPC();
            }

            // If item not coin and enough space    
            if (playerNetworkData.itemList.Count < 12 && item.itemType != Item.ItemType.Coin)
            {
                playerNetworkData.itemList.Add(item);
                playerNetworkData.UpdateItemList();

                if (item.itemId >= 5 && item.itemId <= 13)
                {
                    playerOutputData.placeholderNo++;
                }

                AudioManager.Instance.Play("Pickup");
                itemInRange.DespawnItem_RPC();
            }
            else if(playerNetworkData.itemList.Count >= 12)
            {
                playerOutputData.fullNo++;
                Debug.Log("Inventory is full, cannot pick up item.");
            }
        }
        #endregion

        #region - Interact -
        private void Interact()
        {
            var interactable = interactableInRange;
            interactable.Interact();
        }

        private void EndInteract()
        {
            MapInteractionManager.Instance.EndInteraction();
            isInteracting = false;
        }
        #endregion

        #region - On Trigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactableInRange = interactable;
            }

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
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactableInRange = null;
            }

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

        public void TakeDamage(int damage, PlayerRef shooter)
        {
            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - damage);
            AudioManager.Instance.Play("Hit");

            foreach (var kvp in GamePlayManager.Instance.playerOutputList)
            {
                PlayerRef playerRefKey = kvp.Key;
                PlayerOutputData playerOutputDataValue = kvp.Value;

                if (shooter == playerRefKey)
                {
                    playerOutputDataValue.bulletCollisionOnLiving++;
                }
            }
        }
        #endregion
    }
}

