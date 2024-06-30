using System;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

using DEMO.Manager;
using DEMO.DB;

namespace DEMO.UI
{
    public class TeamCell : NetworkBehaviour
    {
        private ChangeDetector changes;
        [SerializeField] private TMP_Text teamTxt = null;
        [SerializeField] private TMP_Text teamBtnTxt = null;
        [SerializeField] private Button teamBtn = null;
        [SerializeField] private Button expandBtn = null;        
        [Networked] public int teamID { get; set; } = 0;
        PlayerRef player= GamePlayManager.Instance.Runner.LocalPlayer;
        InGameManager inGameManager = null;
        public bool isExpanded = false; 

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            GamePlayManager.Instance.teamList.Add(this);
            GamePlayManager.Instance.UpdatedTeamList();
            inGameManager = FindObjectOfType<InGameManager>();
        }

        public void SetInfo(int id)
        {
            teamTxt.text = $"Team {id}";
            if (GamePlayManager.Instance.gamePlayerList.TryGetValue(player, out PlayerNetworkData playerNetworkData))
            {
                if (playerNetworkData.teamID != -1 && playerNetworkData.teamID != id)
                {
                    teamBtn.gameObject.SetActive(false);
                }
                else if (playerNetworkData.teamID != id)
                {
                    teamBtn.gameObject.SetActive(true);
                    teamBtnTxt.text = "join";
                }      
            }
        }
        
        public void OnTeamBtnClicked()
        {        
            if (GamePlayManager.Instance.gamePlayerList.TryGetValue(player, out PlayerNetworkData playerNetworkData))
            {
                if (teamBtnTxt.text == "join" && playerNetworkData.teamID == -1)
                {
                    playerNetworkData.SetPlayerTeamID_RPC(Int32.Parse(teamTxt.text.Substring(5,1)));
                    teamBtnTxt.text = "quit";
                } 
                else if (teamBtnTxt.text == "quit")
                {
                    bool emptyTeam = true;
                    isExpanded = true;
                    inGameManager.toggleExpand(this);
                    playerNetworkData.SetPlayerTeamID_RPC(-1);

                    foreach (var pnd in GamePlayManager.Instance.gamePlayerList.Values)
                    {
                        if (pnd.teamID == Int32.Parse(teamTxt.text.Substring(5,1)))
                        {
                            emptyTeam = false;
                            break;
                        }
                    }         
                    
                    if (emptyTeam)
                    {
                        GamePlayManager.Instance.teamList.Remove(this);
                        Destroy(gameObject);
                    }           
                }
            }
            GamePlayManager.Instance.UpdatedTeamList();
        }

        public void OnExpandBtnClicked()
        {
            inGameManager.toggleExpand(this);
        }

        public TMP_Text getTeamBtnTxt()
        {
            return teamBtnTxt;
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