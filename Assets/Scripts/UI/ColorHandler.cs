using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DEMO.DB;

namespace DEMO.UI
{
    public class ColorHandler : MonoBehaviour
    {
        [SerializeField] private PlayerOutfitsHandler playerOutfitsHandler;
        [SerializeField] private HSVColorPicker colorPicker;
        [SerializeField] private int index;
        [SerializeField] private Image img;
        private PlayerInfo playerInfo;

        public void OnColorBtnClicked()
        {
            colorPicker.OnSelectedColor += UpdateSelectedColor;
            colorPicker.Init(img);
        }

        public void UpdateSelectedColor()
        {
            if(index == 0)
            {
                playerOutfitsHandler.SetSkinColor(img.color);
            }else{
                playerOutfitsHandler.SetHairColor(img.color);
            }

            playerInfo.colorList[index] = img.color;
        }

        void Start()
        {
            playerInfo = GameManager.playerInfo;
            playerOutfitsHandler = FindObjectOfType<PlayerOutfitsHandler>();
        }
    }
}