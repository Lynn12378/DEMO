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
        [SerializeField] private TMP_Text teamTxt = null;
        //[SerializeField] private Button joinBtn = null;
        private int teamID;

        public void SetInfo(int teamID)
        {
            teamTxt.text = $"Team {teamID}";
            this.teamID = teamID;
        }

        public void OnJoinBtnClicked()
        {
        }
    }
}