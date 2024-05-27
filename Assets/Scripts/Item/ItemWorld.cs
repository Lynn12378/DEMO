using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

using DEMO.Manager;

namespace DEMO.Item
{
    public class ItemWorld : NetworkBehaviour
    {
        // Set public for checking Item
        public ItemClass item;

        public void SetItem(ItemClass newItem)
        {
            item = newItem;
            
            // Set the sprite or other visual properties based on the item
            GetComponent<SpriteRenderer>().sprite = item.GetSprite();
        }

        public ItemClass GetItem()
        {
            return item;
        }
    }
}