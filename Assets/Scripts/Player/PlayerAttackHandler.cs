using System.Collections;
using UnityEngine;
using TMPro;

using Fusion;

public class PlayerAttackHandler : NetworkBehaviour
{
    private PlayerStatsUI playerStatsUI = null;
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootPoint = null;
    [SerializeField] private int maxBullet = 30;
    private int currentBullet;

    private void Start()
    {
        playerStatsUI = FindObjectOfType<PlayerStatsUI>();
        if (playerStatsUI != null)
        {
            currentBullet = maxBullet;
            playerStatsUI.UpdateBulletAmount(currentBullet);
        }
        else
        {
            Debug.LogError("PlayerStatsUI not found!");
        }
    }

    public void Shoot(Vector3 mousePosition)
    {
        if(currentBullet > 0)
        {
            Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles - Vector3.forward * 90);
            bulletPrefab.mousePosition = mousePosition;
            Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority);

            currentBullet -= 1;
            playerStatsUI.UpdateBulletAmount(currentBullet);
        }
        else
        {
            Debug.Log("Not enough bullet.");
            // Show message: Not enough bullet
        }
    }

    // About Bullet Amount and UI Update
    public void AddBullet(int amount)
    {
        currentBullet += amount;
        playerStatsUI.UpdateBulletAmount(currentBullet);
    }

    
}