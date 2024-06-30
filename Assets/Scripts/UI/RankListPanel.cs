using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.Manager;
using DEMO.DB;

namespace DEMO.UI
{
    public class RankListPanel : MonoBehaviour
    {
        [SerializeField] private RankCell rankCellPrefab = null;
        [SerializeField] private Transform contentTrans = null;

        private PlayerOutputData playerOutputData;
        private string playerName;
        private int killNo = 0;
        private int deathNo = 0;
        private float surviveTime = 0f;

        private List<RankCell> rankCells = new List<RankCell>();

        public void UpdateKillNo()
        {

        }

        public void UpdateDeathNo()
        {

        }

        public void UpdateSurviveTime()
        {

        }

        public void CalculateRank()
        {

        }

        public void UpdateRankList()
        {
            foreach(Transform child in contentTrans)
            {
                Destroy(child.gameObject);
            }

            rankCells.Clear();

            foreach(var rankCell in GamePlayManager.Instance.rankList)
            {
                var cell = Instantiate(rankCellPrefab, contentTrans);

                cell.SetRankListInfo(playerName, killNo, deathNo, surviveTime);
            }
        }
    }

}