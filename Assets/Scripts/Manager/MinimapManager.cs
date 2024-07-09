using System.Collections.Generic;
using UnityEngine;

using DEMO.DB;

namespace DEMO.Manager
{
    public class MinimapManager : MonoBehaviour
    {
        private Dictionary<PlayerNetworkData, GameObject> playerMinimapIcons = new Dictionary<PlayerNetworkData, GameObject>();

        public void RegisterPlayer(PlayerNetworkData playerData, GameObject minimapIcon)
        {
            if (!playerMinimapIcons.ContainsKey(playerData))
            {
                playerMinimapIcons.Add(playerData, minimapIcon);
            }
        }

        public void UnregisterPlayer(PlayerNetworkData playerData)
        {
            if (playerMinimapIcons.ContainsKey(playerData))
            {
                playerMinimapIcons.Remove(playerData);
            }
        }

        public void UpdateAllMinimapIconsVisibility(Color localColor)
        {
            foreach (var kvp1 in playerMinimapIcons)
            {
                var playerData1 = kvp1.Key;
                var playerIcon1 = kvp1.Value;

                foreach (var kvp2 in playerMinimapIcons)
                {
                    var playerData2 = kvp2.Key;
                    var minimapIcon2 = kvp2.Value;

                    if (playerData1 == playerData2)
                    {
                        minimapIcon2.SetActive(true);
                        minimapIcon2.GetComponent<SpriteRenderer>().color = localColor;
                        continue;
                    }

                    bool sameTeam = playerData1.teamID == playerData2.teamID && playerData1.teamID != -1;
                    minimapIcon2.SetActive(sameTeam);
                }
            }
        }
    }
}