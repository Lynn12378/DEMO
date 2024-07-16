using System;
using UnityEngine;
using Fusion;
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
        [SerializeField] private PlayerOutputData playerOutputData;
        private float surviveTime = 0f;

        private UIManager uIManager;
        private NetworkButtons buttonsPrevious;
        [SerializeField] private Item itemInRange = null;

        [SerializeField] private Shelter shelter;
        private bool isInShelter = false;
        private float shelterTimer = 0f;
        private const float shelterHealInterval = 5f;

        private MapUIController mapUIController;
        private DoorController Door;

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
            mapUIController = FindObjectOfType<MapUIController>();

            if (mapUIController == null)
            {
                Debug.LogError("MapUIController not found in the scene.");
            }
            else
            {
                Debug.Log("MapUIController successfully found.");
            }
        }

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
            if (surviveTime > playerOutputData.surviveTime)
            {
                playerOutputData.SetSurviveTime_RPC(surviveTime);
            }

            if (playerNetworkData.HP <= 0 || playerNetworkData.foodAmount <= 0)
            {
                playerOutputData.AddDeathNo_RPC();
                playerOutputData.SetSurviveTime_RPC(surviveTime);
                Respawn();
            }

            if (GetInput(out NetworkInputData data))
            {
                ApplyInput(data);
            }

            if (playerNetworkData.playerRef == Runner.LocalPlayer)
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

            AudioCheck();
        }

        #region - Input -
        private async void ApplyInput(NetworkInputData data)
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
                else
                {
                    Debug.Log("Not enough bullet!");
                }
            }

            if (pressed.IsSet(InputButtons.PICKUP))
            {
                if (itemInRange == null) { return; }

                var item = itemInRange.GetComponent<Item>();

                if (item.itemType == Item.ItemType.Coin)
                {
                    playerNetworkData.SetPlayerCoin_RPC(playerNetworkData.coinAmount + 10);
                    itemInRange.DespawnItem_RPC();
                }

                if (playerNetworkData.itemList.Count < 12 && item.itemType != Item.ItemType.Coin)
                {
                    playerNetworkData.itemList.Add(item);
                    playerNetworkData.UpdateItemList();

                    itemInRange.DespawnItem_RPC();
                }
                else if (playerNetworkData.itemList.Count >= 12)
                {
                    Debug.Log("Inventory is full, cannot pick up item.");
                }
            }

            if (pressed.IsSet(InputButtons.TALK))
            {
                rec.TransmitEnabled = !rec.TransmitEnabled;
            }

            if (pressed.IsSet(InputButtons.RELOAD) && isInShelter)
            {
                playerNetworkData.SetPlayerBullet_RPC(playerNetworkData.bulletAmount + 5);
            }

            if (pressed.IsSet(InputButtons.MAP) && isInShelter)
            {
                if (mapUIController != null)
                {
                    mapUIController.ToggleMap();
                }
                else
                {
                    Debug.LogError("mapUIController is not assigned.");
                }
            }

            if (pressed.IsSet(InputButtons.DOOR) && Door != null)
            {   
                if (Door != null)
                {
                    Door.ToggleDoor();
                }
                else
                {
                    Debug.LogError("没有可用的 DoorController");
                }
            }
        }
        #endregion

        #region - Microphone -
        private void AudioCheck()
        {
            if (rec != null && rec.IsCurrentlyTransmitting)
            {
                if (playerNetworkData.playerRef == Runner.LocalPlayer)
                {
                    playerNetworkData.uIManager.UpdateMicIconColor(0);
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

            if (collider.CompareTag("Door"))
            {
                Debug.LogError("??");
                Door = collider.GetComponent<DoorController>();
                if (Door != null)
                {
                    Debug.LogError("Good");
                }
                else
                {
                    Debug.LogError("DoorController 组件未找到");
                }
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
            }

            if (collider.CompareTag("Door"))
            {
                Door = null;
                Debug.LogError("NoGood");
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