using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        Health,
        Food,
        Coin,
        Wood,
        Bullet,
    }

    public ItemType itemType;
    public string name;
    public int amount;
    public bool isDefaultItem;

    public string GetName()
    {
        switch(itemType)
        {
            default:
            case ItemType.Health:       return "Health";
            case ItemType.Food:         return "Food";
            case ItemType.Coin:         return "Coin";
            case ItemType.Wood:         return "Wood";
            case ItemType.Bullet:       return "Bullet";
        }
    }

    public Sprite GetSprite()
    {
        switch(itemType)
        {
            default:
            case ItemType.Health:   return ItemAssets.Instance.healthSprite;
            case ItemType.Food:         return ItemAssets.Instance.foodSprite;
            case ItemType.Coin:         return ItemAssets.Instance.coinSprite;
            case ItemType.Wood:         return ItemAssets.Instance.woodSprite;
            case ItemType.Bullet:       return ItemAssets.Instance.bulletSprite;
        }
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}
