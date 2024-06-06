using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

using DEMO.Manager;
using DEMO.GamePlay.Inventory;
using System;

namespace DEMO.DB
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        [SerializeField] public Slider hpSlider;
        private GamePlayManager gamePlayManager = null;
        private ChangeDetector changes;
        private UIManager uIManager = null;

        [Networked] public int playerId { get; private set; }
        [Networked] public PlayerRef playerRef { get; private set; }
        [Networked] public string playerRefString { get; private set; }
        [Networked] public string playerName { get; private set; }
        [Networked] public int HP { get; set; }
        [Networked] public int bulletAmount { get; set; }
        [Networked] public int teamID { get; set; }

        public int MaxHP = 100;
        public int MaxBullet = 50;
        public List<Item> itemList = new List<Item>();

        public void SetUIManager(UIManager uIManager)
        {
            this.uIManager = uIManager;
        }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            transform.SetParent(Runner.transform);

            gamePlayManager = FindObjectOfType<GamePlayManager>();
            gamePlayManager.gamePlayerList.Add(Object.InputAuthority, this);      
  
            if (Object.HasStateAuthority)
            {
                SetPlayerInfo_RPC(0,"TEST");
                SetPlayerHP_RPC(MaxHP);
                SetPlayerBullet_RPC(MaxBullet);
                SetPlayerTeamID_RPC(-1);
            }
            

            gamePlayManager.UpdatedGamePlayer();
		}

        private void UpdateHPSlider(int health)
        {
            hpSlider.value = health;
        }

        #region - ItemList - 
        public void UpdateItemList()
        {
            uIManager.SetItemList(itemList);
            uIManager.UpdateInventoryUI(itemList);
        }
        #endregion

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerInfo_RPC(int id, string name)
        {
            playerId = id;
			playerName = name;
            playerRefString = playerRef.ToString();
            this.playerRef = playerRef;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerHP_RPC(int hp)
        {
            HP = hp;

            // Change color of slider for LocalPlayer
            if (playerRef == Runner.LocalPlayer)
            {
                // Change color of color code, if failed then color = white
                Color fillColor = ColorUtility.TryParseHtmlString("#A0FF71", out Color color) ? color : Color.white;
                hpSlider.fillRect.GetComponent<Image>().color = fillColor;
            }
		}
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerBullet_RPC(int amount)
        {
            bulletAmount = amount;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetPlayerTeamID_RPC(int id)
        {
            teamID = id;
		}

        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                if(!Object.HasStateAuthority){return;}
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(HP):
                            uIManager.UpdateHPSlider(HP, MaxHP);
                            UpdateHPSlider(HP);
                            break;

                        case nameof(bulletAmount):
                            uIManager.UpdateBulletAmountTxt(bulletAmount, MaxBullet);
                            break;

                        case nameof(teamID):
                            //call UIManager change Team
                            break;
                    }
                }
            }
        #endregion
    }
}