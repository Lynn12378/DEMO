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
        [Networked] public int currentHealth { get; set; }
        [Networked] public int currentBullet { get; set; }
	
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
                //SetHealth_RPC(gameManager.currentHealth);
                //SetBullet_RPC(gameManager.currentBullet);
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
			currentHealth = health;
		}

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
		public void SetBullet_RPC(int bullet)
		{
			currentBullet = bullet;
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
                    case nameof(currentHealth):
                        GameManager.Instance.UpdatePlayerUI();
                        break;
                    case nameof(currentBullet):
                        GameManager.Instance.UpdatePlayerUI();
                        break;
                }
            }
        }

        #endregion
    }
}