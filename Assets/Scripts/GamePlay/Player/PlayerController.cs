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

namespace DEMO.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerNetworkData playerNetworkData;
<<<<<<< HEAD
=======
        [SerializeField] private PlayerOutputData playerOutputData;
        private float surviveTime = 0f;
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9

        private UIManager uIManager;
        private NetworkButtons buttonsPrevious;
        [SerializeField] private Item itemInRange = null;

        [SerializeField] private Shelter shelter;
        private bool isInShelter = false;
        private float shelterTimer = 0f;
        private const float shelterHealInterval = 5f;

<<<<<<< HEAD
=======
        AudioSource speakerSource;
        Recorder rec;

        private void Start()
        {
            if (playerNetworkData.playerRef == Runner.LocalPlayer)
            {
                rec = playerNetworkData.voiceObject.RecorderInUse;
                rec.TransmitEnabled = false;
            }
            
            speakerSource = playerNetworkData.voiceObject.SpeakerInUse.GetComponent<AudioSource>();
        }

>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
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
<<<<<<< HEAD
=======

            surviveTime = 0f;
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
        }

        public override void FixedUpdateNetwork()
        {
<<<<<<< HEAD
            if(playerNetworkData.playerRef == Runner.LocalPlayer)
            {
                uIManager.UpdateMinimapArrow(gameObject.transform);
=======
            surviveTime += Runner.DeltaTime;
            if(surviveTime > playerOutputData.surviveTime)
            {
                playerOutputData.SetSurviveTime_RPC(surviveTime);
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
            }

            if(playerNetworkData.HP <= 0 || playerNetworkData.foodAmount <= 0)
            {
<<<<<<< HEAD
=======
                playerOutputData.AddDeathNo_RPC();
                playerOutputData.SetSurviveTime_RPC(surviveTime);
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
                Respawn();
            }

            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

<<<<<<< HEAD
=======
            if(playerNetworkData.playerRef == Runner.LocalPlayer)
            {
                uIManager.UpdateMinimapArrow(gameObject.transform);
            }

>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
            if (isInShelter)
            {
                shelterTimer += Runner.DeltaTime; // Use Runner.DeltaTime to ensure synchronization

                if (shelterTimer >= shelterHealInterval)
                {
                    HealPlayer(10);
                    shelterTimer = 0f; // Reset timer
                }
            }
<<<<<<< HEAD
        }

        #region - Input - 
=======

            AudioCheck();
        }

        #region - Input -
        /* FIRE, PICKUP, TALK, RELOAD */
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
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
<<<<<<< HEAD
                ToggleTransmit();
=======
                rec.TransmitEnabled = !rec.TransmitEnabled;
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
            }

            if (pressed.IsSet(InputButtons.RELOAD) && isInShelter)
            {
                playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + 5);
            }
        }
        #endregion

        #region - Microphone -
<<<<<<< HEAD
        private void ToggleTransmit()
        {
            /*Recorder recorder = playerNetworkData.voiceObject.RecorderInUse;
            recorder.TransmitEnabled = !recorder.TransmitEnabled;*/
        }
        #endregion

        #region - On Trigger -
=======
        private void AudioCheck()
        {
            if(rec != null && rec.IsCurrentlyTransmitting)
            {
                if(playerNetworkData.playerRef == Runner.LocalPlayer)
                {
                    playerNetworkData.uIManager.UpdateMicIconColor(0);
                    // Use playerRef to test
                    playerNetworkData.uIManager.UpdateMicTxt(playerNetworkData.playerRefString);
                }
                else 
                {
                    playerNetworkData.uIManager.UpdateMicIconColor(-1);
                    playerNetworkData.uIManager.UpdateMicTxt("none");
                }
            }
            else
            {
                foreach (var kvp in GamePlayManager.Instance.gamePlayerList)
                {
                    PlayerNetworkData playerNetworkDataValue = kvp.Value;

                    if (playerNetworkDataValue.voiceObject.IsSpeaking)
                    {
                        playerNetworkData.uIManager.UpdateMicIconColor(1);
                        playerNetworkData.uIManager.UpdateMicTxt(playerNetworkDataValue.playerRefString);
                    }
                    else
                    {
                        playerNetworkData.uIManager.UpdateMicIconColor(-1);
                        playerNetworkData.uIManager.UpdateMicTxt("none");
                    }
                }
            }
        }
        #endregion

        #region - On Trigger : Item & Shelter -
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
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

