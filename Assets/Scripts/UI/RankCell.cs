using UnityEngine;
using TMPro;

namespace DEMO.UI
{
    public class RankCell : MonoBehaviour
    {
        public TMP_Text rankText;
        public TMP_Text playerNameText;
        public TMP_Text valueText;

        private void SetText(TMP_Text textComponent, string text, float fontSize)
        {
            textComponent.text = text;
            textComponent.fontSize = fontSize;
        }

        public void SetRankTitle(string rank, string playerName, string value)
        {
            SetText(rankText, rank, 24);
            SetText(playerNameText, playerName, 32);
            SetText(valueText, value, value == "Survive Time" ? 28 : 32);
        }

        public void SetRankData(int rank, string playerName, int value)
        {
            rankText.text = rank.ToString();
            playerNameText.text = playerName;
            valueText.text = value.ToString();
        }

        public void SetRankData(int rank, string playerName, float value)
        {
            rankText.text = rank.ToString();
            playerNameText.text = playerName;
            valueText.text = value.ToString("F2");
        }
    }
}
