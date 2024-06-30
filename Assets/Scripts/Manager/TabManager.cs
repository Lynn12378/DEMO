using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabManager : MonoBehaviour
{
    public GameObject[] tabs;
    public Image[] tabButtons;
    public TMP_Text[] tabButtonTxt;
    public Vector2 inactiveButtonSize, activeButtonSize;
    public Color inactiveButtonColor, activeButtonColor;
    public Color inactiveTextColor, activeTextColor;

    public void SwitchToTab(int tabID)
    {
        foreach(GameObject tab in tabs)
        {
            tab.SetActive(false);
        }
        tabs[tabID].SetActive(true);

        foreach(Image img in tabButtons)
        {
            img.color = inactiveButtonColor;
            img.rectTransform.sizeDelta = inactiveButtonSize;
        }
        tabButtons[tabID].color = activeButtonColor;
        tabButtons[tabID].rectTransform.sizeDelta = activeButtonSize;

        foreach(TMP_Text txt in tabButtonTxt)
        {
            txt.color = inactiveTextColor;
        }
        tabButtonTxt[tabID].color = activeTextColor;
    }
}
