using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;

using DEMO.Manager;

namespace DEMO.DB
{
    public class shelter : NetworkBehaviour
    {
        private ChangeDetector changes;
        private UIManager uIManager = null;

        [Networked] public int Durability{ get; set; }
        public int MaxDurability = 100;

        [Networked] private TickTimer durabilityTicker { get; set; }

        public void SetUIManager(UIManager uIManager)
        {
            this.uIManager = uIManager;
        }
        // Start is called before the first frame update
        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            durabilityTicker = TickTimer.CreateFromSeconds(Runner,0);
            
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

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DropDurability_RPC()
        {
            if(durabilityTicker.Expired(Runner))
            {
                if(Durability > 0){
                    Durability = Durability - 1;
                }    
            }
            durabilityTicker = TickTimer.CreateFromSeconds(Runner,1);

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

                        case nameof(durabilityTicker):
                            uIManager.UpdateDurabilitySlider(Durability, MaxDurability);
                            break;
                        
                    }
                }
            }
        #endregion
    }
}
