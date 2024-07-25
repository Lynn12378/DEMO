using UnityEngine;
using Fusion;

using DEMO.UI;

namespace DEMO.Gameplay
{
    public class Shelter : NetworkBehaviour
    {
        private ChangeDetector changes;
        private int maxDurability = 100;
        [SerializeField] private BoxCollider2D doorCollider;
        [SerializeField] private UIManager uIManager;
        [SerializeField] public int repair = 20;
        [Networked] public bool IsOpen { get; set; } = false;
        [Networked] public int durability { get; private set; }
        [Networked] private TickTimer durabilityTicker { get; set; }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            SetDurability_RPC(maxDurability);
            durabilityTicker = TickTimer.CreateFromSeconds(Runner, 1);
        }

        public override void FixedUpdateNetwork()
        {
            if (durabilityTicker.Expired(Runner) && durability > 0)
            {
                SetDurability_RPC(durability - 1);
                durabilityTicker = TickTimer.CreateFromSeconds(Runner, 5);
            }
            
            if(durability <= 0)
            {
                GameManager.Instance.RestartGame();
            }
        }

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetDurability_RPC(int durability)
        {
            this.durability = durability;
        }

        [Rpc]
        public void SetIsOpen_RPC()
        {
            this.IsOpen = !IsOpen;
        }
        #endregion

        #region - OnChanged Events -
        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(durability):
                        uIManager.UpdateDurabilitySlider(durability, maxDurability);
                        break;
                    case nameof(IsOpen):
                        doorCollider.isTrigger = IsOpen;
                        break;
                }
            }
        }
        #endregion
    }
}