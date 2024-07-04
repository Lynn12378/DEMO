using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private GameObject mapUI;

    private void Start()
    {
        // 確保地圖 UI 開始時是隱藏的
        mapUI.SetActive(false);
    }

    public void ShowMap()
    {
        mapUI.SetActive(true);
    }

    public void HideMap()
    {
        mapUI.SetActive(false);
    }

    public void ToggleMap()
    {
        mapUI.SetActive(!mapUI.activeSelf);
    }
}
