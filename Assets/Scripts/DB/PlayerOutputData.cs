using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.Manager;
using DEMO.UI;
using Unity.VisualScripting;

namespace DEMO.DB
{
    public class PlayerOutputData : NetworkBehaviour
    {
        private GamePlayManager gamePlayManager = null;
        private ChangeDetector changes;

        [Networked] public int _playerId { get; private set; }
        [Networked] public PlayerRef playerRef { get; private set; }
        [Networked] public int _killNo { get; set; }
        [Networked] public int _deathNo { get; set; }
        [Networked] public float _surviveTime { get; set; }

        public int playerId = 0;
        public int killNo = 0;                                  // No. of enemy killed
        public int deathNo = 0;                                 // No. of death
        public float surviveTime = 0;                           // Longest survival time
        public int collisionNo = 0;                             // No. of player's collision with buildings
        public int bulletCollision = 0;                         // No. of player bullet's collision with buildings
        public int bulletCollisionOnLiving = 0;                 // No. of player shoot another player or animals              
        public List<int> remainHP = new List<int>();            // HP amount remained when refill HP
        public List<int> remainBullet = new List<int>();        // Bullet amount remained when refill bullet
        public float totalVoiceDetectionDuration = 0;           // Duration of voice detected on player's mic
        public int organizeNo = 0;                              // No. of player organize inventory
        public int fullNo = 0;                                  // No. of player's inventory full
        public int placeholderNo = 0;                           // No. of placeholder items that player pick up
        public int rankNo = 0;                                  // No. of player open rank panel
        public int giftNo = 0;                                  // No. of player gift items to others
        public int createTeamNo = 0;                            // No. of player create new team
        public int joinTeamNo = 0;                              // No. of player join team
        public int quitTeamNo = 0;                              // No. of player quit team
        public int repairQuantity = 0;                          // Quantity of player given to repair shelter
        public int restartNo = 0;                               // No. of times shelter durability = 0
        public int usePlaceholderNo = 0;                         // No. of player use badge at right building
        public int feedNo = 0;                                  // No. of player feed livings


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

        #region - JSON -
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        #endregion

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerId_RPC(int id)
        {
            _playerId = id;
            playerId = id;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerRef_RPC()
        {
            playerRef = Runner.LocalPlayer;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddKillNo_RPC()
        {
            _killNo++;
            killNo++;
            Debug.Log(Object.InputAuthority.ToString() + " 's kill no. is " + killNo.ToString());
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddDeathNo_RPC()
        {
            _deathNo++;
            deathNo++;
            Debug.Log(Object.InputAuthority.ToString() + " 's death no. is " + deathNo.ToString());
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetSurviveTime_RPC(float longestTime)
        {
            _surviveTime = longestTime;
            surviveTime = longestTime;
		}
        #endregion
    }
}
