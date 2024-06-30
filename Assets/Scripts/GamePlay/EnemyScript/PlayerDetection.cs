using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace DEMO.GamePlay.EnemyScript
{
    public class PlayerDetection : NetworkBehaviour
    {
        // A list to keep track of detected player NetworkObjects
        public List<NetworkObject> detectedObjs = new List<NetworkObject>();

        // Detect when enter zone
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                var networkObject = collider.GetComponent<NetworkObject>();
                if (networkObject != null && !detectedObjs.Contains(networkObject))
                {
                    AddDetectedObj_RPC(networkObject);
                }
            }
        }

        // Detect when leave zone
        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                var networkObject = collider.GetComponent<NetworkObject>();
                if (networkObject != null && detectedObjs.Contains(networkObject))
                {
                    RemoveDetectedObj_RPC(networkObject);
                }
            }
        }

        #region - RPCs -
        // RPC to add a detected object across the network
        [Rpc(RpcSources.All, RpcTargets.All)]
        private void AddDetectedObj_RPC(NetworkObject networkObject)
        {
            if (!detectedObjs.Contains(networkObject))
            {
                detectedObjs.Add(networkObject);
            }
        }

        // RPC to remove a detected object across the network
        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RemoveDetectedObj_RPC(NetworkObject networkObject)
        {
            if (detectedObjs.Contains(networkObject))
            {
                detectedObjs.Remove(networkObject);
            }
        }
        #endregion
    }
}