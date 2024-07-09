using UnityEngine;
using TMPro;
using Fusion;


namespace DEMO.UI
{
    public class TeamPlayerCell : NetworkBehaviour
    {
        [SerializeField] private TMP_Text teamPlayerCellTxt = null;
        
        public TMP_Text getTeamPlayerCellTxt()
        {
            return teamPlayerCellTxt;
        }

    }
}