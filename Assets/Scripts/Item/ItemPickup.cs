using UnityEngine;
using Fusion;

public class ItemPickup : NetworkBehaviour {

	// Pick up the item
	public void PickUp (Item item)
    {
		bool wasPickedUp = Inventory.instance.Add(item);	// Add to inventory

		// If successfully picked up
		if (wasPickedUp)
			Runner.Despawn(Object);	// Destroy item from scene
	}

}