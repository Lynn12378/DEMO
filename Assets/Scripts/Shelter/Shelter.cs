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

        [Networked] private TickTimer durabilityTicker { get; set; }
        [SerializeField] UIManager uIManager;

        // Start is called before the first frame update
        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasStateAuthority)
            {
                SetShelterDurability_RPC(MaxDurability);
                durabilityTicker = TickTimer.CreateFromSeconds(Runner, 1);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (durabilityTicker.Expired(Runner))
                {
                    DropDurability_RPC();
                }
            }
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetShelterDurability_RPC(int durability)
        {
            Durability = durability;
            if (uIManager != null)
            {
                uIManager.UpdateDurabilitySlider(Durability, MaxDurability);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DropDurability_RPC()
        {
            Debug.Log($"Current Durability: {Durability}");
            if (Durability > 0)
            {
                Durability -= 1;
                durabilityTicker = TickTimer.CreateFromSeconds(Runner, 1);
            }
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

                    case nameof(durabilityTicker):
                        break;
                }
            }
        }

        #endregion
    }
}
