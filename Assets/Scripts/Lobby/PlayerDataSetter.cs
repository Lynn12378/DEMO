using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using DEMO.Lobby;

namespace DEMO.Lobby
{
    public class PlayerDataSetter : MonoBehaviour
    {
        private GameManager gameManager = null;
        [SerializeField] private TMP_InputField inputField = null;
        [SerializeField] private LobbyManager lobbyManager = null;

        private void Start()
        {
            gameManager = lobbyManager.getGameManager();
        }
        public void OnPlayerNameInputFieldChange()
        {
            gameManager.PlayerName = inputField.text;
            gameManager.SetPlayerNetworkData();
        }
    }

}
