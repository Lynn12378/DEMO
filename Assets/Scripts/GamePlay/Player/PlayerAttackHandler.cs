using UnityEngine;
using Fusion;

using DEMO.Manager;

namespace DEMO.GamePlay.Player
{
    public class PlayerAttackHandler : NetworkBehaviour
    {
        [SerializeField] private Bullet bulletPrefab = null;
        [SerializeField] private Transform shootPoint = null;
        private PlayerController playerController;

        public void Init(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public void Shoot(Vector2 mousePosition)
        {
            Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles);
            Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority,
                (Runner, NO) => NO.GetComponent<Bullet>().Init(mousePosition, playerController));
            AudioManager.Instance.Play("Shoot");
        }
    }
}