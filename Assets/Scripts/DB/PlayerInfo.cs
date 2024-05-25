using System.Collections;
using UnityEngine;
using Fusion;

using DEMO.Manager;

namespace DEMO.DB
{
    public class PlayerInfo : NetworkBehaviour
    {
        private GameManager gameManager = null;
        private ChangeDetector changes;

        [Networked] public int playerId { get; private set; }
        [Networked] public string playerName { get; private set; }
        public bool isInRoom = false;
        public int Player_id;
        public string Player_name;
        public string Player_password;

        public override void Spawned()
        {
			gameManager = GameManager.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            if(!isInRoom)
            {
                gameManager.playerList.Add(Object.InputAuthority, this);
                transform.SetParent(GameManager.Instance.transform);
                if (Object.HasStateAuthority)
                {
                    SetPlayerInfo_RPC();
                }
            }
            
            gameManager.UpdatePlayerList();
		}

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC()
        {
            playerId = Player_id;
			playerName = Player_name;
		}
        
        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(playerName):
                            GameManager.Instance.UpdatePlayerList();
                            break;
                        // case nameof(IsReady):
                        //     GameManager.Instance.UpdatePlayerList();
                        //     break;
                    }
                }
            }
        #endregion
    }
}