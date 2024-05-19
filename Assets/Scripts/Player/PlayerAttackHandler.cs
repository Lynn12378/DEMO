using System.Collections;
using UnityEngine;
using TMPro;

using Fusion;

namespace DEMO.Player
{
    public class PlayerAttackHandler : NetworkBehaviour
    {
        [SerializeField] private PlayerStatsUI playerStatsUI = null;
        [SerializeField] private Bullet bulletPrefab = null;
        [SerializeField] private Transform shootPoint = null;
        private int maxBullet = 30;
        private int currentBullet { get; set; }

        private void Start()
        {
            SetMaxBullet();
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
                //playerStatsUI.SetBulletUI(currentBullet);
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
            //playerStatsUI.SetBulletUI(currentBullet);
        }

        public void SetMaxBullet()
        {
            currentBullet = maxBullet;
            //playerStatsUI.SetBulletUI(currentBullet);
        }
    }
}