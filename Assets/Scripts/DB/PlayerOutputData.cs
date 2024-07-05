using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.Manager;
using DEMO.UI;

namespace DEMO.DB
{
    public class PlayerOutputData : NetworkBehaviour
    {
        private GamePlayManager gamePlayManager = null;
        private ChangeDetector changes;

        [Networked] public PlayerRef playerRef { get; private set; }
        [Networked] public int killNo { get; set; }         // No. of enemy killed
        [Networked] public int deathNo { get; set; }        // No. of death
        [Networked] public float surviveTime { get; set; }  // Longest survival time
        public int collisionNo;                             // No. of player's collision with buildings
        public int bulletCollision;                         // No. of player bullet's collision with buildings
        public int remainHP;                                // HP amount remained when refill HP
        public int remainBullet;                            // Bullet amount remained when refill bullet


        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(Runner.transform);

            gamePlayManager = FindObjectOfType<GamePlayManager>();
            gamePlayManager.playerOutputList.Add(Object.InputAuthority, this);

            if (Object.HasStateAuthority)
            {
                SetPlayerRef_RPC();
            }
		}

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerRef_RPC()
        {
            playerRef = Runner.LocalPlayer;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddKillNo_RPC()
        {
            killNo++;
            Debug.Log(Object.InputAuthority.ToString() + " 's kill no. is " + killNo.ToString());
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddDeathNo_RPC()
        {
            deathNo++;
            Debug.Log(Object.InputAuthority.ToString() + " 's death no. is " + deathNo.ToString());
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetSurviveTime_RPC(float longestTime)
        {
            surviveTime = longestTime;
		}
        #endregion
    }
}
