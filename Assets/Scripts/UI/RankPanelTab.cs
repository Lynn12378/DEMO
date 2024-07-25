using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DEMO.DB;

namespace DEMO.UI
{
    public class RankPanelTab : MonoBehaviour
    {
        public GameObject[] tabs;
        public Image[] tabButtons;
        public TMP_Text[] tabButtonTxt;
        public Vector2 inactiveButtonSize, activeButtonSize;
        public Color inactiveButtonColor, activeButtonColor;
        public Color inactiveTextColor, activeTextColor;

        [SerializeField] private RankCell rankCellPrefab = null;
        List<PlayerOutputData> playerOutputDataList;


        private void Start()
        {
            playerOutputDataList = new List<PlayerOutputData>(GamePlayManager.Instance.playerOutputList.Values);
            SwitchToTab(0);
        }

        #region - Switch Tab -
        public void SwitchToTab(int tabID)
        {
            foreach(GameObject tab in tabs)
            {
                tab.SetActive(false);
            }
            tabs[tabID].SetActive(true);

            foreach(Image img in tabButtons)
            {
                img.color = inactiveButtonColor;
                img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x, inactiveButtonSize.y);
            }
            tabButtons[tabID].color = activeButtonColor;
            tabButtons[tabID].rectTransform.sizeDelta = new Vector2(tabButtons[tabID].rectTransform.sizeDelta.x, activeButtonSize.y);

            foreach(TMP_Text txt in tabButtonTxt)
            {
                txt.color = inactiveTextColor;
            }
            tabButtonTxt[tabID].color = activeTextColor;

            CalculateRank(tabID); // Calculate rank for that tab when switch
        }
        #endregion

        #region - Rank Calculate -
        public void CalculateRank(int tabID)
        {
            // Clear old data
            foreach (GameObject tab in tabs)
            {
                foreach (Transform child in tab.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            // Add rank title
            RankCell titleCell = Instantiate(rankCellPrefab, tabs[tabID].transform);
            titleCell.SetRankTitle("排名", "玩家暱稱", GetRankTitleByTabID(tabID));

            // Calculate rank by tabID
            switch (tabID)
            {
                case 0:
                    CalculateAndDisplayRank(GamePlayManager.Instance.playerOutputList.Values, (a, b) => b.killNo.CompareTo(a.killNo), tabID);
                    break;
                case 1:
                    CalculateAndDisplayRank(GamePlayManager.Instance.playerOutputList.Values, (a, b) => a.deathNo.CompareTo(b.deathNo), tabID);
                    break;
                case 2:
                    CalculateAndDisplayRank(GamePlayManager.Instance.playerOutputList.Values, (a, b) => b.surviveTime.CompareTo(a.surviveTime), tabID);
                    break;
                default:
                    break;
            }
        }

        private string GetRankTitleByTabID(int tabID)
        {
            switch (tabID)
            {
                case 0:
                    return "擊殺數量";
                case 1:
                    return "死亡次數";
                case 2:
                    return "生存時長";
                default:
                    return "";
            }
        }

        private void CalculateAndDisplayRank(IEnumerable<PlayerOutputData> playerOutputList, Comparison<PlayerOutputData> comparison, int tabID)
        {
            // Sort
            List<PlayerOutputData> sortedList = new List<PlayerOutputData>(playerOutputList);
            sortedList.Sort(comparison);

            // Update rank UI
            for (int i = 0; i < sortedList.Count; i++)
            {
                PlayerOutputData data = sortedList[i];
                string playerName = GamePlayManager.Instance.GetPlayerNameByRef(data.playerRef);

                RankCell cell = Instantiate(rankCellPrefab, tabs[tabID].transform);
                cell.SetRankData(i + 1, playerName, GetRankValueByTabID(data, tabID));
            }
        }

        private int GetRankValueByTabID(PlayerOutputData data, int tabID)
        {
            switch (tabID)
            {
                case 0:
                    return data.killNo;
                case 1:
                    return data.deathNo;
                case 2:
                    return Mathf.RoundToInt(data.surviveTime);
                default:
                    return 0;
            }
        }
        #endregion
    }
}