using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

using DEMO.Manager;

namespace DEMO.DB
{
    public class Shelter : NetworkBehaviour
    {
        private ChangeDetector changes;

        [Networked] public int Durability { get; set; }
        public int MaxDurability = 100;

        [SerializeField] public int repair = 20;

        [Networked] private TickTimer durabilityTicker { get; set; }
        [SerializeField] UIManager uIManager;

        // Start is called before the first frame update
        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            Durability = MaxDurability;
            durabilityTicker = TickTimer.CreateFromSeconds(Runner, 1);
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (durabilityTicker.Expired(Runner))
                {
                    if (Durability > 0)
                    {
                        Durability -= 1;
                        durabilityTicker = TickTimer.CreateFromSeconds(Runner, 1);
                    }
                }
            }
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetDurability_RPC(int durability)
        {
            Durability = durability;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RepairDurability_RPC()
        {
            Durability = Mathf.Min(Durability + 10, MaxDurability);
        }
        #endregion

        #region - OnChanged Events -

        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(Durability):
                        if (uIManager != null)
                        {
                            uIManager.UpdateDurabilitySlider(Durability, MaxDurability);
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
