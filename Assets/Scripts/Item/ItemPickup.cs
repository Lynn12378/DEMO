using UnityEngine;
using Fusion;

public class ItemPickup : MonoBehaviour {

	// Pick up the item
	public void PickUp (Item item)
    {
		bool wasPickedUp = Inventory.instance.Add(item);	// Add to inventory

		// If successfully picked up
		if (wasPickedUp)
			Destroy(gameObject);	// Destroy item from scene
	}

}
