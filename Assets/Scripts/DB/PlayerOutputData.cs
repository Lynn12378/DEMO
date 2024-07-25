using System.Collections.Generic;
using UnityEngine;
using Fusion;

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
        public int usePlaceholderNo = 0;                        // No. of player use badge at right building
        public int petNo = 0;                                  // No. of player pet livings
        public int sendMessageNo = 0;                           // No. of player send text message
        public float durationOfRound = 0;                       // Duration of that round when game ends or restarts


        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(Runner.transform);

            gamePlayManager = GamePlayManager.Instance;
            gamePlayManager.playerOutputList.Add(Object.InputAuthority, this);

            if (Object.HasStateAuthority)
            {
                SetPlayerRef_RPC();
            }

            durationOfRound = 0f;
		}

        public override void FixedUpdateNetwork()
        {
            durationOfRound += Runner.DeltaTime;
        }

        public void Restart()
        {
            durationOfRound = 0f;
            killNo = 0;
            deathNo = 0;
            surviveTime = 0;
            collisionNo = 0;
            bulletCollision = 0;
            bulletCollisionOnLiving = 0;
            remainHP.Clear();
            remainBullet.Clear();
            totalVoiceDetectionDuration = 0;
            organizeNo = 0;
            fullNo = 0;
            placeholderNo = 0;
            rankNo = 0;
            giftNo = 0;
            createTeamNo = 0;
            joinTeamNo = 0;
            quitTeamNo = 0;
            repairQuantity = 0;
            restartNo = 0;
            usePlaceholderNo = 0;
            petNo = 0;
            sendMessageNo = 0;
            _killNo = 0;
            _deathNo = 0;
            _surviveTime = 0f;
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
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void AddDeathNo_RPC()
        {
            _deathNo++;
            deathNo++;
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
