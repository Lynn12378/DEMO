using UnityEngine;
using Fusion;

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
        private MapInteractionManager mapInteractionManager;
        private NetworkButtons buttonsPrevious;
        [SerializeField] private Item itemInRange = null;
        [SerializeField] private IInteractable interactableInRange = null;
        private bool isInteracting = false;
        [SerializeField] private bool shopInRange = false;

        [SerializeField] private Shelter shelter;
        [Networked] private TickTimer shelterTimer { get; set; }
        private GamePlayManager gamePlayManager;

        public override void Spawned()
        {
            gamePlayManager = GamePlayManager.Instance;
            mapInteractionManager = FindObjectOfType<MapInteractionManager>();

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

        public void Restart()
        {
            playerOutputData.restartNo++;
            playerOutputData.AddDeathNo_RPC();

            transform.position = Vector3.zero;

            playerNetworkData.Restart();
            playerOutputData.Restart();
            
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

            if (shelter != null)
            {
                if (shelterTimer.Expired(Runner))
                {
                    playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP + 10);
                    shelterTimer = TickTimer.CreateFromSeconds(Runner, 5);
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
                    gamePlayManager.ShowWarningBox("請填充子彈。");
                }
            }

            if (pressed.IsSet(InputButtons.SPACE)) // Spacebar
            {
                if(itemInRange != null)
                {
                    Pickup();
                }
                else if(shopInRange)
                {
                    uIManager.OnOpenShopButton();
                }
                else if(interactableInRange != null && isInteracting == false)
                {
                    Interact();
                    isInteracting = true;
                }
                else if ( (interactableInRange != null && isInteracting) || interactableInRange == null)
                {
                    EndInteract();
                }
                else
                {
                    return;
                }          
            }

            if (pressed.IsSet(InputButtons.PET))
            {
                if (isInteracting && mapInteractionManager.currentInteraction.interactionType == InteractionType.Pet)
                {
                    mapInteractionManager.Pet(gameObject);
                    playerOutputData.petNo++;
                }
            }

            if (pressed.IsSet(InputButtons.TALK))
            {
                voiceDetection.rec.TransmitEnabled = !voiceDetection.rec.TransmitEnabled;
            }

            if (pressed.IsSet(InputButtons.RELOAD) && shelter != null)
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
                gamePlayManager.ShowWarningBox("背包已滿，不能撿起物品。");
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
            mapInteractionManager.EndInteraction();
            isInteracting = false;
        }
        #endregion

        #region - On Trigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            // Check for interactable objects
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactableInRange = interactable;
            }

            // Check for items
            Item item = collider.GetComponent<Item>();
            if (item != null)
            {
                itemInRange = item;
            }

            // Check for shelter
            Shelter shelterCollider = collider.GetComponent<Shelter>();
            if (shelterCollider != null)
            {
                shelterTimer = TickTimer.CreateFromSeconds(Runner, 0);
                shelter = shelterCollider;
                playerNetworkData.SetShelter(shelter);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            // Check for interactable objects
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable == interactableInRange)
            {
                interactableInRange = null;
            }

            // Check for items
            Item item = collider.GetComponent<Item>();
            if (item != null && item == itemInRange)
            {
                itemInRange = null;
            }

            // Check for shelter
            Shelter shelterCollider = collider.GetComponent<Shelter>();
            if (shelterCollider != null && shelterCollider == shelter)
            {
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

            if(collision.collider.CompareTag("Shop"))
            {
                shopInRange = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.collider.CompareTag("Shop"))
            {
                shopInRange = false;
                uIManager.CloseShopPanel();
            }
        }
        #endregion

        #region - Player HP -
        public void TakeDamage(int damage)
        {
            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - damage);
        }

        public void TakeDamage(int damage, PlayerRef shooter)
        {
            playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - damage);
            AudioManager.Instance.Play("Hit");

            foreach (var kvp in gamePlayManager.playerOutputList)
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