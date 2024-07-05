using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Photon.Voice.Unity;

using DEMO.DB;
using DEMO.UI;
using DEMO.GamePlay.Inventory;
using DEMO.GamePlay.EnemyScript;
using DEMO.GamePlay.Player;

namespace DEMO.Manager
{
    public class GamePlayManager : MonoBehaviour
    {
        /// 代替GameManager
        public static GamePlayManager Instance { get; private set; }
        [SerializeField] private NetworkRunner runner = null;

        public NetworkRunner Runner
        {
            get
            {
                if (runner == null)
                {
                    runner = gameObject.AddComponent<NetworkRunner>();
                    runner.ProvideInput = true;
                }
                return runner;
            }
        }

        private void Awake()
        {
            Runner.ProvideInput = true;

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        #region - playerNetworkData -
            public Dictionary<PlayerRef, PlayerNetworkData> gamePlayerList = new Dictionary<PlayerRef, PlayerNetworkData>();
            
            public event Action OnInGamePlayerUpdated = null;
            public void UpdatedGamePlayer()
            {
                OnInGamePlayerUpdated?.Invoke();
            }

            public string GetPlayerNameByRef(PlayerRef playerRef)
            {
                if (gamePlayerList.TryGetValue(playerRef, out PlayerNetworkData playerNetworkData))
                {
                    return playerNetworkData.playerName;
                }
                return "Unknown";
            }
        #endregion

        #region - OutputData & VoiceDetection -

        public Dictionary<PlayerRef, PlayerOutputData> playerOutputList = new Dictionary<PlayerRef, PlayerOutputData>();
        public Dictionary<PlayerRef, PlayerVoiceDetection> playerVoiceList = new Dictionary<PlayerRef, PlayerVoiceDetection>();

        public void AddOrganizeNo(PlayerRef organizePlayerRef)
        {
            foreach (var kvp in playerOutputList)
            {
                PlayerRef playerRefKey = kvp.Key;
                PlayerOutputData playerOutputData = kvp.Value;

                if (playerRefKey == organizePlayerRef)
                {
                    playerOutputData.organizeNo++;
                    Debug.Log(playerRefKey.ToString() + "'s organize no is: " + playerOutputData.organizeNo);
                }
            }
        }

        public void AddRankNo(PlayerRef rankPlayerRef)
        {
            foreach (var kvp in playerOutputList)
            {
                PlayerRef playerRefKey = kvp.Key;
                PlayerOutputData playerOutputData = kvp.Value;

                if (playerRefKey == rankPlayerRef)
                {
                    playerOutputData.rankNo++;
                    Debug.Log(playerRefKey.ToString() + "'s rank no is: " + playerOutputData.rankNo);
                }
            }
        }
        
        #endregion

        #region - TeamList -
            public int newTeamID = 0;
            public List<TeamCell> teamList = new List<TeamCell>();

            public event Action OnTeamListUpdated = null;
            public void UpdatedTeamList()
            {
                OnTeamListUpdated?.Invoke();
            }
        #endregion

        #region - Spawn List -
            public List<Item> itemList = new List<Item>();
            public List<Enemy> enemyList = new List<Enemy>();
        #endregion

        #region - RankList -
            public List<RankCell> rankList = new List<RankCell>();
            public event Action OnRankListUpdated = null;
            public void UpdateRankList()
            {
                OnRankListUpdated?.Invoke();
            }
        #endregion
    }
}