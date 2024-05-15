using UnityEngine;
using TMPro;

namespace DEMO.Player
{
    public class PlayerStatsUI : MonoBehaviour
    {
        public TextMeshProUGUI bulletAmountText;

        public void UpdateBulletAmount(int amount)
        {
            if (bulletAmountText != null)
            {
                bulletAmountText.text = amount.ToString();
            }
        }
    }
}
