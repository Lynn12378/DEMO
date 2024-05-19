using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Fusion;

namespace DEMO.Player
{
    public class PlayerNetworkData : NetworkBehaviour
    {
		private GameManager gameManager = null;
        private ChangeDetector changes;

		[Networked] public string PlayerName { get; set; }
		[Networked] public NetworkBool IsReady { get; set; }
        [Networked] public int CurrentHealth { get; set; }
        [Networked] public int CurrentBullet { get; set; }
	
        public override void Spawned()
        {
			gameManager = GameManager.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

			transform.SetParent(GameManager.Instance.transform);

			gameManager.playerList.Add(Object.InputAuthority, this);
			gameManager.UpdatePlayerList();

			if (Object.HasInputAuthority)
			{
				SetPlayerName_RPC(gameManager.PlayerName);
			}
		}

        #region - RPCs -

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
		public void SetPlayerName_RPC(string name)
        {
			PlayerName = name;
		}

		[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
		public void SetReady_RPC(bool isReady)
		{
			IsReady = isReady;
		}

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
		public void SetHealth_RPC(int health)
		{
			CurrentHealth = health;
		}

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
		public void SetBullet_RPC(int bullet)
		{
			CurrentBullet = bullet;
		}

        #endregion

        #region - OnChanged Events -

        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(PlayerName):
                        GameManager.Instance.UpdatePlayerList();
                        break;
                    case nameof(IsReady):
                        GameManager.Instance.UpdatePlayerList();
                        break;
                    case nameof(CurrentHealth):
                        GameManager.Instance.UpdatePlayerUI();
                        break;
                    case nameof(CurrentBullet):
                        GameManager.Instance.UpdatePlayerUI();
                        break;
                }
            }
        }

        #endregion
    }
}