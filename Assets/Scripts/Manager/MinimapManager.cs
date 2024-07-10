using System.Collections.Generic;
using UnityEngine;

using DEMO.DB;
using UnityEngine.Analytics;

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
                Debug.Log(playerData.playerRefString + "registered.");
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
            Debug.Log(playerMinimapIcons.ToString());

            foreach (var kvp in playerMinimapIcons)
            {
                var otherPlayerData = kvp.Key;
                var minimapIcon = kvp.Value;

                if (playerData == otherPlayerData) continue;

                if(playerData.teamID != -1)
                {
                    bool sameTeam = playerData.teamID == otherPlayerData.teamID;
                    minimapIcon.SetActive(sameTeam);
                }
            }
        }

        /*public void UpdateAllMinimapIconsVisibility(Color localColor)
        {
            Debug.Log("Into update all.");
            Debug.Log(playerMinimapIcons.ToString());

            foreach (var kvp1 in playerMinimapIcons)
            {
                var playerData1 = kvp1.Key;
                var playerIcon1 = kvp1.Value;

                playerIcon1.GetComponent<SpriteRenderer>().color = localColor;

                UpdatePlayerMinimapIconVisibility(playerData1);
            }
        }*/
    }
}