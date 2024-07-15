using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;

using DEMO.UI;
using DEMO.Manager;

namespace DEMO.DB
{
    public class PlayerDBHandler : DBMgr
    {
        [SerializeField] private TMP_Text LPlayerNameTxt;
        [SerializeField] private TMP_Text LPlayerPasswordTxt;

        [SerializeField] private TMP_Text SPlayerNameTxt;
        [SerializeField] private TMP_Text SPlayerPasswordTxt;

        [SerializeField] private TMP_Text messageTxt;
        [SerializeField] private List<GameObject> dialog = new List<GameObject>();

        [SerializeField] private List<GameObject> lobby = new List<GameObject>();
        [SerializeField] private List<GameObject> CreateRolePanel = new List<GameObject>();
        
        [SerializeField] private PlayerInfo playerInfo;
        private PanelManager panelManager;

        private void Start()
        {
            panelManager = FindObjectOfType<PanelManager>();
        }

        public void Login()
        {
            action = "login";
            SetPlayerName(LPlayerNameTxt.text);
            SetPlayerPassword(LPlayerPasswordTxt.text);
            StartCoroutine(SendData());
        }
        
        public void SignUp()
        {
            action = "signUp";
            SetPlayerName(SPlayerNameTxt.text);
            SetPlayerPassword(SPlayerPasswordTxt.text);
            StartCoroutine(SendData());
        }

        public void Create()
        {
            action = "create";
            SetOufits();
            StartCoroutine(SendData());
        }

        public new IEnumerator SendData()
        {
            formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("PlayerInfo", playerInfo.ToJson()),
            };

            SetForm(formData, "Player", action);
            yield return StartCoroutine(base.SendData());

            string responseText = base.GetResponseText();
            Debug.Log(responseText);
            JObject jsonResponse = JObject.Parse(responseText);

            if (!string.IsNullOrEmpty(responseText))
            {
                var status = jsonResponse["status"].ToString();
                if (status == "Success")
                {
                    if(action == "signUp")
                    {
                        int Player_id = Int32.Parse(jsonResponse["Player_id"].ToString());
                        SetPlayerID(Player_id);
                        panelManager.OnActivePanel(CreateRolePanel);
                    }else if(action == "login"){
                        var playerInfoJS = jsonResponse["PlayerInfo"].ToString();
                        playerInfo.FromJson(playerInfoJS);
                        panelManager.OnActivePanel(lobby);
                    }else{
                        panelManager.OnActivePanel(lobby);
                    }
                }
                else
                {
                    panelManager.OnActivePanel(dialog);
                }
                
                GameManager.playerInfo = playerInfo;

                messageTxt.text = jsonResponse["message"].ToString();
            }
            else
            {
                messageTxt.text = "Error: No response from server.";
            }
        }

        private void SetPlayerID(int Player_id)
        {
            playerInfo.Player_id = Player_id;
        }

        public void SetPlayerName(string text)
        {
            playerInfo.Player_name = text.Trim('\u200b');
        }

        public void SetPlayerPassword(string text)
        {
            playerInfo.Player_password = text.Trim('\u200b');
        }

        public void SetOufits()
        {
            var playerOutfitsHandler = FindObjectOfType<PlayerOutfitsHandler>();
            playerInfo.outfits = new List<string>();
            foreach(var resolver in playerOutfitsHandler.resolverList)
            {
                playerInfo.outfits.Add(resolver.GetLabel().ToString());
            }
        }
    }
}