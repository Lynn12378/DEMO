using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DEMO.UI;
using DEMO.DB;

namespace DEMO.Manager
{
    public class TabManager : MonoBehaviour
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
                img.rectTransform.sizeDelta = inactiveButtonSize;
            }
            tabButtons[tabID].color = activeButtonColor;
            tabButtons[tabID].rectTransform.sizeDelta = activeButtonSize;

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
            titleCell.SetRankTitle("Rank", "Player Name", GetRankTitleByTabID(tabID));

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
                    Debug.LogWarning("Invalid tabID: " + tabID);
                    break;
            }
        }

        private string GetRankTitleByTabID(int tabID)
        {
            switch (tabID)
            {
                case 0:
                    return "Kill No.";
                case 1:
                    return "Death No.";
                case 2:
                    return "Survive Time";
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
                //string playerName = GamePlayManager.Instance.GetPlayerNameByRef(data.playerRef);

                RankCell cell = Instantiate(rankCellPrefab, tabs[tabID].transform);
                //cell.SetRankData(i + 1, playerName, GetRankValueByTabID(data, tabID));

                // Use playerRef to test
                cell.SetRankData(i + 1, data.playerRef.ToString(), GetRankValueByTabID(data, tabID));
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