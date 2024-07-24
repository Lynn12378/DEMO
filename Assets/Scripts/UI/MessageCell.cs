using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

namespace DEMO.UI
{
    public class MessageCell : NetworkBehaviour
    {
        private ChangeDetector changes;
        [SerializeField] private TMP_Text messageTxt = null;
        [Networked] public string playerName { get; set; }
        [Networked] public string message { get; set; }
        [Networked] TickTimer timer { get; set; }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            timer = TickTimer.CreateFromSeconds(Runner, 10f);

            GameManager.Instance.messages.Add(this);

            messageTxt.text = $"{playerName}:{message}";
            GameManager.Instance.UpdatedMessages();
        }

        public override void FixedUpdateNetwork()
        {
            if (timer.Expired(Runner))
            {
                Runner.Despawn(Object);
                GameManager.Instance.messages.Remove(this);
            }
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetMessage_RPC(string playerName, string message)
        {
            this.playerName = playerName;
            this.message = message;
		}

        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(message):
                            messageTxt.text = $"{playerName}:{message}";
                            GameManager.Instance.UpdatedMessages();
                            break;
                    }
                }
            }
        
        #endregion
    }
}