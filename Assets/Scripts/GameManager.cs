using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;

using DEMO.DB;
using DEMO.UI;

namespace DEMO
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private NetworkRunner runner = null;

        [SerializeField] private FusionVoiceClient voiceClient;
        [SerializeField] private Recorder rec;
        [SerializeField] private GameObject speakerPrefab;

        public NetworkRunner Runner
        {
            get
            {
                if (runner == null)
                {
                    runner = gameObject.AddComponent<NetworkRunner>();
                    runner.ProvideInput = true;

                    voiceClient = gameObject.AddComponent<FusionVoiceClient>();
                    voiceClient.PrimaryRecorder = rec;
                    voiceClient.SpeakerPrefab = speakerPrefab;
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

        #region - End game & Reload -
        public void EndGame()
        {
            foreach (var player in GamePlayManager.Instance.playerOutputList)
            {
                player.Value.restartNo++;
            }

            Destroy(voiceClient);
            Destroy(Runner);

            // Restart game after 2 seconds
            Invoke("Restart", 2f);
        }

        private void Restart()                                  ////////////////////// Restart still need refine
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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