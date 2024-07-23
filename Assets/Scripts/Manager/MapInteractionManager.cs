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
using DEMO.GamePlay;

namespace DEMO.UI
{
    public class MapInteractionManager : MonoBehaviour
    {
        [SerializeField] private GameObject interactionPanel;
        [SerializeField] private TMP_Text interactTxt = null;
        [SerializeField] private TMP_Text instructionTxt = null;

        public List<Interactions> interactions = new List<Interactions>();
        public Interactions currentInteraction = null;

        private Spawner spawner;

        private void Start()
        {
            spawner = FindObjectOfType<Spawner>();

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
            else if(interaction.interactionType == InteractionType.Pet)
            {
                interaction.instructionIxt = " Tap [P] to pet, [space] to close ";
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

        /*public void Feed(PlayerNetworkData PND, PlayerOutputData POD)
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
        }*/

        public void Pet(GameObject go)
        {
            float randomValue = UnityEngine.Random.value; // Random float of 0-1
            if (randomValue < 0.1f)
            {
                // 10% prob. to drop 0-1 item when enemy died
                spawner.SpawnItemAround(go.transform, UnityEngine.Random.Range(0, 2));
                AfterPet(true);
            }
            else
            {
                AfterPet(false);
            }
        }

        public void AfterPet(bool dropItem)
        {
            string interactionName = dropItem ? "AfterPetDrop" : "AfterPet";
            List<Interactions> afterPetMatches = interactions.FindAll(i => i.name == interactionName);

            if (afterPetMatches.Count > 0)
            {
                // Randomly choose an interaction with match name
                Interactions afterPet = afterPetMatches[UnityEngine.Random.Range(0, afterPetMatches.Count)];
                interactTxt.SetText(name + afterPet.interactionTxt);
                instructionTxt.SetText(afterPet.instructionIxt);
            }
        }

        public void EndInteraction()
        {
            interactionPanel.SetActive(false);
            currentInteraction = null;
        }
    }
}
