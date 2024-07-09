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
        public int bulletCollisionOnLiving;                 // No. of player shoot another player or animals
        public int remainHP;                                // HP amount remained when refill HP
        public int remainBullet;                            // Bullet amount remained when refill bullet
        public float totalVoiceDetectionDuration;           // Duration of voice detected on player's mic
        public int organizeNo;                              // No. of player organize inventory
        public int fullNo;                                  // No. of player's inventory full
        public int placeholderNo;                           // No. of placeholder items that player pick up
        public int rankNo;                                  // No. of player open rank panel
        public int giftNo;                                  // No. of player gift items to others
        public int createTeamNo;                            // No. of player create new team
        public int joinTeamNo;                              // No. of player join team
        public int quitTeamNo;                              // No. of player quit team


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
