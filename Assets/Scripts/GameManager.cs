using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;

using DEMO.DB;
using DEMO.UI;
using DEMO.GamePlay;
using DEMO.GamePlay.Player;
using DEMO.Gameplay;

namespace DEMO
{
    public class GameManager : MonoBehaviour
    {
        private string baseUrl = "http://localhost/DEMO/BFI-15.php";

        public static GameManager Instance { get; private set; }
        [SerializeField] private NetworkRunner runner = null;
        [SerializeField] private PlayerController[] players;
        [SerializeField] private Shelter shelter;
        [SerializeField] private Spawner spawner;

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

        #region - EndGame -
        public void EndGame()
        {
            SceneManager.LoadScene("EndGame");

            GoToQuestion();
        }

        public void GoToQuestion()
        {
            int playerId = 0;

            foreach (var player in GamePlayManager.Instance.gamePlayerList)
            {
                if(player.Key == Runner.LocalPlayer) playerId = player.Value.playerId;
            }

            // Construct the full URL with the player ID as a parameter
            string fullUrl = baseUrl + "?player_id=" + playerId.ToString();

            if(playerId != 0)
            {
                Application.OpenURL(fullUrl);
            }
        }
        #endregion

        #region - Restart -
        public void RestartGame()
        {
            players = FindObjectsOfType<PlayerController>();
            shelter = FindObjectOfType<Shelter>();
            spawner = FindObjectOfType<Spawner>();

            if(runner.IsSceneAuthority)
            {
                foreach (var player in GamePlayManager.Instance.playerOutputList)
                {
                    player.Value.restartNo++;
                }

                // Restart after 2 seconds
                Invoke("Restart", 2f);
            }
        }

        private void Restart()
        {
            if(runner.IsSceneAuthority)
            {
                shelter.Restart();

                foreach (PlayerController player in players)
                {
                    player.Restart();
                }

                //spawner.Restart();
            }
        }

        #endregion

        #region - playerInfo -
            public static PlayerInfo playerInfo = null;
            public Dictionary<PlayerRef, PlayerInfo> playerList = new Dictionary<PlayerRef, PlayerInfo>();
            
            public event Action OnPlayerListUpdated = null;
            public void UpdatePlayerList()
            {
                OnPlayerListUpdated?.Invoke();
            }
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