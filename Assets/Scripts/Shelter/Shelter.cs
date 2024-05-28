using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

using DEMO.Manager;

namespace DEMO.DB
{
    public class shelter : NetworkBehaviour
    {
        private ChangeDetector changes;
        private UIManager uIManager = null;
        [Networked] public int Durability{ get; set; }
        public int MaxDurability = 100;

        public void SetUIManager(UIManager uIManager)
        {
            this.uIManager = uIManager;
        }
        // Start is called before the first frame update
        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(Runner.transform);
            
            if (Object.HasStateAuthority)
            {
                SetShelterDurability_RPC(MaxDurability);
            }
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetShelterDurability_RPC(int durability)
        {
            Durability = durability;
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
                            uIManager.UpdateDurabilitySlider(Durability, MaxDurability);
                            break;
                    }
                }
            }
        #endregion
    }
}
