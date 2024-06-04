using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEMO.GamePlay.Inventory
{
	public class Inventory
	{
		private List<Item> items;

        public Inventory()
        {
            items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public List<Item> GetItems()
        {
            return items;
        }
    }
}