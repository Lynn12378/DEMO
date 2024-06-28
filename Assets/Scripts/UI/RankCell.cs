using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DEMO.UI
{
    public class RankCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName = null;
        [SerializeField] private TMP_Text killNo = null;
        [SerializeField] private TMP_Text deathNo = null;
        [SerializeField] private TMP_Text surviveTime = null;

        public void SetRankListInfo(string name, int kill, int death, float time)
        {
            playerName.text = name;
            killNo.text = kill.ToString();
            deathNo.text = death.ToString();
            surviveTime.text = time.ToString();
        }
    }
}
