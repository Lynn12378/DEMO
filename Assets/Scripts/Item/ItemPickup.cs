using UnityEngine;
using Fusion;

namespace DEMO.Item
{
	public class ItemPickup : NetworkBehaviour 
	{
		// Pick up the item
		/*public void PickUp (PlayerRef player, ItemClass item)
		{
			Debug.Log(player);
			Inventory playerInventory = PlayerInventoryManager.instance.GetPlayerInventory(player);
			bool wasPickedUp = playerInventory.Add(item);	// Add to inventory

			// If successfully picked up
			if (wasPickedUp)
				Runner.Despawn(Object);	// Destroy item from scene
		}*/

	}
}