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

        [Networked] public int durability { get; set; }
        public int maxDurability = 100;

        [SerializeField] public int repair = 20;

        [Networked] private TickTimer durabilityTicker { get; set; }
        [SerializeField] UIManager uIManager;


        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            durability = maxDurability;
            durabilityTicker = TickTimer.CreateFromSeconds(Runner, 1);
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (durabilityTicker.Expired(Runner))
                {
                    if (durability > 0)
                    {
                        durability -= 1;
                        durabilityTicker = TickTimer.CreateFromSeconds(Runner, 5);
                    }
                }
            }
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetDurability_RPC(int durability)
        {
            this.durability = durability;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RepairDurability_RPC()
        {
            durability = Mathf.Min(durability + 10, maxDurability);
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
                        if (uIManager != null)
                        {
                            uIManager.UpdateDurabilitySlider(durability, maxDurability);
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
