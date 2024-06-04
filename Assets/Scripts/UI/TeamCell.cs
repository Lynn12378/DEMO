using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

using DEMO.Manager;

namespace DEMO.UI
{
    public class TeamCell : NetworkBehaviour
    {
        private ChangeDetector changes;
        [SerializeField] private TMP_Text teamTxt = null;
        //[SerializeField] private Button joinBtn = null;
        [Networked] public int teamID { get; set; } = 0;

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            GamePlayManager.Instance.teamList.Add(this);
            GamePlayManager.Instance.newTeamID += 1;

            GamePlayManager.Instance.UpdatedTeamList();
        }

        public void SetInfo(int id)
        {
            teamTxt.text = $"Team {id}";
        }

        public void OnJoinBtnClicked()
        {
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerTeamID_RPC(int id)
        {
            teamID = id;
		}

        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(teamID):
                            GamePlayManager.Instance.UpdatedTeamList();
                            break;
                    }
                }
            }
        
        #endregion
    }
}

