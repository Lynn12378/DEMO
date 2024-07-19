using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

using DEMO.GamePlay.Interactable;
using DEMO.DB;
using DEMO.Manager;
using System.ComponentModel;
using DEMO.GamePlay.Inventory;
using TMPro.Examples;

namespace DEMO.UI
{
    public class MapInteractionManager : MonoBehaviour
    {
        [SerializeField] private GameObject interactionPanel;
        [SerializeField] private TMP_Text interactTxt = null;
        [SerializeField] private TMP_Text instructionTxt = null;

        public List<Interactions> interactions = new List<Interactions>();
        public Interactions currentInteraction = null;

        private void Start()
        {
            foreach(Interactions i in interactions)
            {
                SetInstructionIxt(i);
            }
        }

        public void SetInstructionIxt(Interactions interaction)
        {
            if(interaction.interactionType == InteractionType.TextOnly)
            {
                interaction.instructionIxt = " Tap [space] to close ";
            }
            else
            {
                interaction.instructionIxt = " Tap [F] to feed, [space] to close ";
            }
        }

        public void SetCurrentInteraction(string name)
        {
            List<Interactions> matches = interactions.FindAll(i => i.name == name);

            if (matches.Count > 0)
            {
                // Randomly choose an interaction with match name
                Interactions interaction = matches[UnityEngine.Random.Range(0, matches.Count)];
                currentInteraction = interaction;

                StartInteraction();
            }
            else
            {
                Debug.LogError($"No interactions found with name: {name}");
            }
        }

        public void StartInteraction()
        {
            interactionPanel.SetActive(true);
            interactTxt.SetText(currentInteraction.interactionTxt);
            instructionTxt.SetText(currentInteraction.instructionIxt);
        }

        public void Feed(PlayerNetworkData PND, PlayerOutputData POD)
        {
            Debug.Log(PND.ToString());
            PND.ShowList();

            Item food = new Item
            {
                itemType = Item.ItemType.Food,
            };
            
            if(PND.itemList.Contains(food))
            {
                PND.itemList.Remove(food);
                PND.UpdateItemList();
                AudioManager.Instance.Play("Eat");
                POD.feedNo++;

                AfterFeed(currentInteraction.name);
            }
            else
            {
                //Debug.Log("There is no food in your inventory.");
                GamePlayManager.Instance.ShowWarningBox("There is no food in your inventory.");
            }
        }

        public void AfterFeed(string name)
        {
            List<Interactions> afterFeedMatches = interactions.FindAll(i => i.name == "AfterFeed");

            if (afterFeedMatches.Count > 0)
            {
                // Randomly choose an interaction with match name
                Interactions afterFeed = afterFeedMatches[UnityEngine.Random.Range(0, afterFeedMatches.Count)];
                interactTxt.SetText(name + afterFeed.interactionTxt);
                instructionTxt.SetText(afterFeed.instructionIxt);
            }
        }

        public void EndInteraction()
        {
            interactionPanel.SetActive(false);
            currentInteraction = null;
        }

        public bool GetActiveInteractionPanel()
        {
            return interactionPanel.activeSelf;
        }
    }
}
