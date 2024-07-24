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
        [Networked] public int teamID { get; set; } = 0;
        private PlayerRef player;
        private InGameManager inGameManager = null;
        private GamePlayManager gamePlayManager;
        public bool isExpanded = false;

        public override void Spawned()
        {
            gamePlayManager = GamePlayManager.Instance;

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            player = Runner.LocalPlayer;
            gamePlayManager.teamList.Add(this);
            gamePlayManager.UpdatedTeamList();
            inGameManager = FindObjectOfType<InGameManager>();
        }

        public void SetInfo(int id)
        {
            teamTxt.text = $"Team {id}";
            if (gamePlayManager.gamePlayerList.TryGetValue(player, out PlayerNetworkData playerNetworkData))
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
            if (gamePlayManager.gamePlayerList.TryGetValue(player, out PlayerNetworkData playerNetworkData))
            {
                if (teamBtnTxt.text == "join" && playerNetworkData.teamID == -1)
                {
                    playerNetworkData.SetPlayerTeamID_RPC(Int32.Parse(teamTxt.text.Substring(5,1)));
                    teamBtnTxt.text = "quit";

                    if (gamePlayManager.playerOutputList.TryGetValue(player, out PlayerOutputData playerOutputData))
                    {
                        playerOutputData.joinTeamNo++;
                    }
                } 
                else if (teamBtnTxt.text == "quit")
                {
                    bool emptyTeam = true;
                    isExpanded = true;
                    inGameManager.toggleExpand(this);
                    playerNetworkData.SetPlayerTeamID_RPC(-1);

                    if (gamePlayManager.playerOutputList.TryGetValue(player, out PlayerOutputData playerOutputData))
                    {
                        playerOutputData.quitTeamNo++;
                    }

                    foreach (var pnd in gamePlayManager.gamePlayerList.Values)
                    {
                        if (pnd.teamID == Int32.Parse(teamTxt.text.Substring(5,1)))
                        {
                            emptyTeam = false;
                            break;
                        }
                    }         
                    
                    if (emptyTeam)
                    {
                        gamePlayManager.teamList.Remove(this);
                        gamePlayManager.UpdatedTeamList();

                        Destroy(gameObject);
                    }           
                }
            }
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
                            gamePlayManager.UpdatedTeamList();
                            break;
                    }
                }
            }
        
        #endregion
    }
}
