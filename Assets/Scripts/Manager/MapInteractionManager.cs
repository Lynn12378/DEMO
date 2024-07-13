using System;
using UnityEngine;
using TMPro;

using DEMO.GamePlay;

namespace DEMO.UI
{
    public class MapInteractionManager : MonoBehaviour
    {
        [SerializeField] private GameObject interactionPanel;
        [SerializeField] private TMP_Text interactTxt = null;

        public Interactions[] interactions;

        public static MapInteractionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void StartInteraction(string name)
        {
            interactionPanel.SetActive(true);

            Interactions interaction = Array.Find(interactions, i => i.name == name);
            interactTxt.SetText(interaction.txt);
        }

        public void EndInteraction()
        {
            interactionPanel.SetActive(false);
        }

        public bool GetActiveInteractionPanel()
        {
            return interactionPanel.activeSelf;
        }
    }
}
