using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

using DEMO.Manager;

namespace DEMO.Gameplay
{
    public class Shelter : NetworkBehaviour
    {
        private ChangeDetector changes;
        private int maxDurability = 100;
        [SerializeField] private BoxCollider2D collider;
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
                        collider.isTrigger = IsOpen;
                        break;
                }
            }
        }
        #endregion
    }
}