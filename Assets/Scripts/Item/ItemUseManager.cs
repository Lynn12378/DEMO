using UnityEngine;
using System.Collections.Generic;

public class ItemUseManager : MonoBehaviour
{
    // Singleton
	public static ItemUseManager instance;

    // Dictionary to map item names to their respective use methods
    private Dictionary<string, System.Action> useActions = new Dictionary<string, System.Action>();

    //private PlayerStats playerStats;
    //private PlayerAimWeapon playerAimWeapon;

	void Awake ()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of ItemUseManager found!");
			return;
		}

		instance = this;

        //playerStats = FindObjectOfType<PlayerStats>();
        //playerAimWeapon = FindObjectOfType<PlayerAimWeapon>();

        // Initialize use actions for each item
        InitializeUseActions();
	}

    // Define use actions for each item
    private void InitializeUseActions()
    {
        // Add use actions for each item
        //useActions.Add("HealthPill", UseHealthPill);
        useActions.Add("Food", UseFood);
        useActions.Add("Wood", UseWood);
        //useActions.Add("Bullet", UseBullet);
    }

    // Method to use an item based on its name
    public void UseItemByManager(string itemName)
    {
        // Check if the item name exists in the dictionary
        if (useActions.ContainsKey(itemName))
        {
            // Call the corresponding use action for the item
            useActions[itemName].Invoke();
        }
        else
        {
            Debug.LogWarning("Item \"" + itemName + "\" does not have a use action defined.");
        }
    }

/*
    private void UseHealthPill()
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(-25); // Add 25 health
            Debug.Log("Health pill used. Current health: " + playerStats.currentHealth);
        }
        else
        {
            Debug.LogWarning("PlayerStats not found!");
        }
    }*/

    private void UseFood()
    {
        // Implement use behavior for mana potion
        Debug.Log("Using food...");
    }

    private void UseWood()
    {
        // Implement use behavior for wood
        Debug.Log("Using wood...");
    }

    /*private void UseBullet()
    {
        if (playerAimWeapon != null)
        {
            playerAimWeapon.AddBullet(1); // Add 1 bullet
        }
        else
        {
            Debug.LogWarning("PlayerAimWeapon not found!");
        }
    }*/
}
