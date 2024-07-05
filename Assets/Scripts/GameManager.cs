using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.DB;
using DEMO.UI;

namespace DEMO
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
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

        #region - playerInfo -

            public static PlayerInfo playerInfo = null;
            public Dictionary<PlayerRef, PlayerInfo> playerList = new Dictionary<PlayerRef, PlayerInfo>();
            
            public event Action OnPlayerListUpdated = null;
            public void UpdatePlayerList()
            {
                OnPlayerListUpdated?.Invoke();
            }
  
        #endregion

        #region - playerNetworkData -

        public Dictionary<PlayerRef, PlayerNetworkData> gamePlayerList = new Dictionary<PlayerRef, PlayerNetworkData>();

        #endregion

        #region - Messages -

        public List<MessageCell> messages = new List<MessageCell>();

        public event Action OnMessagesUpdated = null;
        public void UpdatedMessages()
        {
            OnMessagesUpdated?.Invoke();
        }
        
        #endregion

    }
}