using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; } // Singleton

    private void Awake()
    {
        Instance = this;
    }

    public Transform pfItemWorld;

    public Sprite healthSprite;
    public Sprite foodSprite;
    public Sprite coinSprite;
    public Sprite woodSprite;
    public Sprite bulletSprite;
}
