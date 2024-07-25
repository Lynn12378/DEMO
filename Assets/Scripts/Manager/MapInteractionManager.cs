using System.Collections.Generic;
using UnityEngine;
using TMPro;

using DEMO.GamePlay.Interactable;
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

        public Spawner spawner;

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
                interaction.instructionIxt = " 按 [空白鍵] 關閉 ";
            }
            else if(interaction.interactionType == InteractionType.Pet)
            {
                interaction.instructionIxt = " 按 [P] 撫摸，按 [空白鍵] 關閉 ";
            }
        }

        public void SetCurrentInteraction(string name)
        {
            List<Interactions> matches = interactions.FindAll(i => i.name == name);

            if (matches.Count > 0)
            {
                // Randomly choose an interaction with match name
                Interactions interaction = matches[Random.Range(0, matches.Count)];
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

        public void Pet(GameObject go)
        {
            float randomValue = Random.value; // Random float of 0-1
            if (randomValue < 0.1f)
            {
                // 10% prob. to drop 1-2 item when pet animals
                spawner.SpawnItemAround(go.transform, Random.Range(1,3));
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
                Interactions afterPet = afterPetMatches[Random.Range(0, afterPetMatches.Count)];
                interactTxt.SetText(afterPet.interactionTxt);
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
