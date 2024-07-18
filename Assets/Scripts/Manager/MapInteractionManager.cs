using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using DEMO.GamePlay.Interactable;
using DEMO.DB;
using DEMO.Manager;
using System.ComponentModel;
using DEMO.GamePlay.Inventory;

namespace DEMO.UI
{
    public class MapInteractionManager : MonoBehaviour
    {
        public static MapInteractionManager Instance { get; private set; }

        [SerializeField] private GameObject interactionPanel;
        [SerializeField] private TMP_Text interactTxt = null;
        [SerializeField] private TMP_Text instructionTxt = null;

        //public Interactions[] interactions;
        public List<Interactions> interactions = new List<Interactions>();
        public Interactions currentInteraction = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

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
            /*Interactions interaction = Array.Find(interactions, i => i.name == name);
            currentInteraction = interaction;

            StartInteraction();*/

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
            Item food = new Item
            {
                itemType = Item.ItemType.Food
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
                Debug.Log("There is no food in your inventory.");
            }
        }

        public void AfterFeed(string name)
        {
            /*Interactions afterFeed = Array.Find(interactions, i => i.name == "AfterFeed");

            interactTxt.SetText(name + afterFeed.interactionTxt);
            instructionTxt.SetText(afterFeed.instructionIxt);*/

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
