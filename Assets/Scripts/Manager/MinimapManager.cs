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

        public void UpdatePlayerMinimapIconVisibility(PlayerNetworkData playerData)
        {
            foreach (var kvp in playerMinimapIcons)
            {
                var otherPlayerData = kvp.Key;
                var minimapIcon = kvp.Value;

                if (playerData == otherPlayerData) continue;

                bool sameTeam = playerData.teamID == otherPlayerData.teamID;
                minimapIcon.SetActive(sameTeam);
            }
        }

        public void UpdateAllMinimapIconsVisibility(Color localColor)
        {
            foreach (var kvp1 in playerMinimapIcons)
            {
                var playerData1 = kvp1.Key;
                var minimapIcon1 = kvp1.Value;

                foreach (var kvp2 in playerMinimapIcons)
                {
                    minimapIcon1.SetActive(true);

                    var playerData2 = kvp2.Key;
                    var minimapIcon2 = kvp2.Value;

                    if (playerData1 == playerData2)
                    {
                        minimapIcon2.SetActive(true);
                        minimapIcon2.GetComponent<SpriteRenderer>().color = localColor;
                        continue;
                    }
                    
                    if (playerData1.teamID == -1)
                    {
                        // Player with no team only sees their own icon
                        minimapIcon2.SetActive(false);
                    }
                    else
                    {
                        // Players in a team see each other
                        bool sameTeam = playerData1.teamID == playerData2.teamID;
                        minimapIcon2.SetActive(sameTeam);
                    }
                }
            }
        }
    }
}