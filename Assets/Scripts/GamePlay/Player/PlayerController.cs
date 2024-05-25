using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.DB;

namespace DEMO.GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movementHandler = null;
        [SerializeField] private PlayerAttackHandler attackHandler = null;
        [SerializeField] private PlayerNetworkData playerNetworkDataPrefab;
        private PlayerNetworkData playerNetworkData;
        private HealthPoint healthPoint = null;
        private GameObject obj;
        private NetworkButtons buttonsPrevious;

        public override void Spawned()
        {
            playerNetworkData = playerNetworkDataPrefab;

            // obj = GameObject.Find("LocalPlayer");
            // if(obj != null)
            // {
            //     playerNetworkData = obj.GetComponent<PlayerNetworkData>();
            // }

            // Debug.Log("Find:" + playerNetworkData.HP);
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

            if (pressed.IsSet(InputButtons.TESTDAMAGE))
            {
                Debug.Log($"TESTDAMAGE");
                playerNetworkData.SetPlayerHP_RPC(playerNetworkData.HP - 10);
                Debug.Log(playerNetworkData.HP);
            }
        }
    }
}

