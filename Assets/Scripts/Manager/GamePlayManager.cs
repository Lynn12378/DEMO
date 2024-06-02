using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.DB;
using DEMO.UI;
//using DEMO.Item;

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

        #endregion

        #region - TeamList -

        public List<TeamCell> teamList = new List<TeamCell>();

        public event Action OnTeamListUpdated = null;
        public void UpdatedTeamList()
        {
            OnTeamListUpdated?.Invoke();
        }
        
        #endregion
    }
}