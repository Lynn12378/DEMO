using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;

using DEMO.GamePlay.Inventory;
using DEMO.Manager;
using DEMO.UI;

namespace DEMO.GamePlay.Interactable
{
    public class Building : NetworkBehaviour, IInteractable
    {
        public string buildingName;
        public Item.ItemType badgeType;             // Normal badgeType of this building
        public int achievementThreshold = 50;       // How many normal badge to achievement bagde
        public GameObject achievementBadge;
        public Transform achievementSpawnPoint;     // Achievement bagde spawn point

        [Networked] private int currentBadgeCount { get; set; }

        private MapInteractionManager mapInteractionManager;


        private void Start()
        {
            mapInteractionManager = FindObjectOfType<MapInteractionManager>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void AddBadge_RPC()
        {
            currentBadgeCount++;
            CheckForAchievement();
        }

        private void CheckForAchievement()
        {
            if (currentBadgeCount >= achievementThreshold)
            {
                ShowAchievementBadge();
            }
        }

        private void ShowAchievementBadge()
        {
            achievementBadge.SetActive(true);
        }

        public void Interact()
        {
            mapInteractionManager.SetCurrentInteraction(buildingName);
        }
    }
}
