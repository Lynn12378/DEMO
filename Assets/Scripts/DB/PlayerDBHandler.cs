using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;

using DEMO.Manager;
using WebSocketSharp;

namespace DEMO.DB
{
    public class PlayerDBHandler : DBMgr
    {
        [SerializeField] PanelManager panelManager = null;
        [SerializeField] private TMP_Text playerNameTxt = null;
        [SerializeField] private TMP_Text playerPasswordTxt = null;
        [SerializeField] private PlayerInfo playerInfo = null;
        [SerializeField] private GameObject WarningPanel = null; // Reference to the warning panel
        [SerializeField] private TMP_Text warningText = null; // Reference to the warning text component
        [SerializeField] private Button Closebutton = null; // Reference to the close button
        
        public void Login()
        {
            action = "login";
            StartCoroutine(SendData());
        }
        
        public void SignUp()
        {
            action = "signUp";
            StartCoroutine(SendData());
        }

        private new IEnumerator SendData()
        {
            SetPlayerName();
            SetPlayerPassword();

            if (playerInfo.Player_name.Length < 5 || playerInfo.Player_name.Length > 12)  //checking playername input
            {
                WarningPanel.SetActive(true);
                warningText.text = "Player's name must be between 5 to 12 letters.";
                yield break; // Exit the coroutine if validation fails
            }
            if (playerInfo.Player_password.IsNullOrEmpty())  //checking password input
            {
                WarningPanel.SetActive(true);
                warningText.text = "Please enter your password.";
                yield break; // Exit the coroutine if validation fails
            }


            formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("Player_name", playerInfo.Player_name),
                new MultipartFormDataSection("Player_password", playerInfo.Player_password)
            };

            SetForm(formData, "Player", action);

            yield return StartCoroutine(base.SendData());

            string responseText = base.GetResponseText();
            JObject jsonResponse = JObject.Parse(responseText);

            //登入註冊
            if (!string.IsNullOrEmpty(responseText))
            {
                var status = jsonResponse["status"].ToString();

                Debug.Log(responseText);
                if (status == "Success")
                {
                    int Player_id = Int32.Parse(jsonResponse["Player_id"].ToString());
                    SetPlayerID(Player_id);
                    
                    GameManager.playerInfo = playerInfo;
                    panelManager.OnActiveLobbyPanel();
                }
                var message = jsonResponse["message"].ToString();
                Debug.Log(message);
            }
            else
            {
                Debug.Log("Error: No response from server.");
            }
        }

        private void SetPlayerID(int Player_id)
        {
            playerInfo.Player_id = Player_id;
        }

        public void SetPlayerName()
        {
            playerInfo.Player_name = playerNameTxt.text.Trim('\u200b');
        }

        public void SetPlayerPassword()
        {
            playerInfo.Player_password = playerPasswordTxt.text.Trim('\u200b');
        }

        public void ClosingWarningPanel(){
            playerNameTxt.text = string.Empty;
            playerPasswordTxt.text = string.Empty;
            WarningPanel.SetActive(false);
        }
    }
}