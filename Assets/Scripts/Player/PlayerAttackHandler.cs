using System.Collections;
using UnityEngine;
using TMPro;

using Fusion;

namespace DEMO.Player
{
    public class PlayerAttackHandler : NetworkBehaviour
    {
        // private PlayerStatsUI playerStatsUI = null;
        [SerializeField] private Bullet bulletPrefab = null;
        [SerializeField] private Transform shootPoint = null;
        [SerializeField] private int maxBullet = 30;
        private int currentBullet;

        private void Start()
        {
            // playerStatsUI = FindObjectOfType<PlayerStatsUI>();
            // if (playerStatsUI != null)
            // {
                currentBullet = maxBullet;
            //     playerStatsUI.UpdateBulletAmount(currentBullet);
            // }
            // else
            // {
            //     Debug.LogError("PlayerStatsUI not found!");
            // }
        }

        public void Shoot(Vector2 mousePosition)
        {
            if(currentBullet > 0)
            {
                mousePosition = mousePosition - new Vector2(transform.position.x, transform.position.y);  
                Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles - Vector3.forward * 90);
                Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority,
                    (Runner, NO) => NO.GetComponent<Bullet>().Init(mousePosition));

                currentBullet -= 1;
                // playerStatsUI.UpdateBulletAmount(currentBullet);
            }
            else
            {
                Debug.Log("Not enough bullet.");
                // Show message: Not enough bullet
            }
        }

        // About Bullet Amount and UI Update
        // public void AddBullet(int amount)
        // {
        //     currentBullet += amount;
        //     playerStatsUI.UpdateBulletAmount(currentBullet);
        // }
    }
}