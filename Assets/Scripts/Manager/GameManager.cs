using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.DB;

namespace DEMO.Manager
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
<<<<<<< HEAD

=======
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
            public static PlayerInfo playerInfo = null;
            public Dictionary<PlayerRef, PlayerInfo> playerList = new Dictionary<PlayerRef, PlayerInfo>();
            
            public event Action OnPlayerListUpdated = null;
            public void UpdatePlayerList()
            {
                OnPlayerListUpdated?.Invoke();
            }
<<<<<<< HEAD
  
        #endregion

        #region - playerNetworkData -
=======
        #endregion

        /*#region - playerNetworkData -
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
            public Dictionary<PlayerRef, PlayerNetworkData> gamePlayerList = new Dictionary<PlayerRef, PlayerNetworkData>();
            
            public event Action OnInGamePlayerUpdated = null;
            public void UpdatedGamePlayer()
            {
                OnInGamePlayerUpdated?.Invoke();
            }
<<<<<<< HEAD

        #endregion
=======
        #endregion*/
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
        
    }
}