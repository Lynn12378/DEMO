using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DEMO;

namespace DEMO.Lobby
{
    public class CreateRoomPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private LobbyManager lobbyManager = null;
        [SerializeField] private CanvasGroup canvasGroup = null;

        [SerializeField] private TMP_InputField roomNameInputField = null;

        public void DisplayPanel(bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }

        public void OnBackBtnClicked()
        {
            lobbyManager.SetPairState(PairState.Lobby);
        }

        public async void OnCreateBtnClicked()
        {
            string roomName = roomNameInputField.text;
            
            await lobbyManager.CreateRoom(roomName);
        }
    }
}