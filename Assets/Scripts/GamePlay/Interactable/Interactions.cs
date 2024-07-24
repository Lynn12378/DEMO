using UnityEngine;

namespace DEMO.GamePlay.Interactable
{
    [System.Serializable]
    public class Interactions
    {
        public string name;
        public InteractionType interactionType;
        public string interactionTxt;
        [HideInInspector] public string instructionIxt;
    }

    public enum InteractionType
    {
        TextOnly,   // Show text only
        Pet         // Pet livings
    }
}
