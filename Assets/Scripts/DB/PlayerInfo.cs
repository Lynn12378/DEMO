using System.Collections;
using System.Collections.Generic;
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
        [Networked] public bool isReady { get; private set; }
        public int Player_id;
        public string Player_name;
        public string Player_password;
        public List<Color> colorList = new List<Color>();
        public List<string> outfits = new List<string>();
        public Dictionary<string, string> outfitList = new Dictionary<string, string>();

        public override void Spawned()
        {
			gameManager = GameManager.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasStateAuthority)
            {
                SetPlayerInfo_RPC();
            }

            transform.SetParent(GameManager.Instance.transform);
            gameManager.playerList.Add(Object.InputAuthority, this);
            gameManager.UpdatePlayerList();

            Debug.Log("setParent");
		}

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void FromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC()
        {
            playerId = Player_id;
			playerName = Player_name;
            isReady = false;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetIsReady_RPC()
        {
            isReady = !isReady;
		}
        
        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(isReady):
                            GameManager.Instance.UpdatePlayerList();
                            break;
                    }
                }
            }
        #endregion
    }
}