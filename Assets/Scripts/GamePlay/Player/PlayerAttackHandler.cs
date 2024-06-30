using System.Collections;
using UnityEngine;
using TMPro;

using Fusion;

namespace DEMO.GamePlay.Player
{
    public class PlayerAttackHandler : NetworkBehaviour
    {
        [SerializeField] private Bullet bulletPrefab = null;
        [SerializeField] private Transform shootPoint = null;

        public void Shoot(Vector2 mousePosition)
        {
<<<<<<< HEAD
            // mousePosition = mousePosition - new Vector2(transform.position.x, transform.position.y);  
            Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles);
            Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority,
                (Runner, NO) => NO.GetComponent<Bullet>().Init(mousePosition));
=======
            Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles);
            Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority,
                (Runner, NO) => NO.GetComponent<Bullet>().Init(mousePosition, Object.InputAuthority));
>>>>>>> 1e73d3857742deca280a555b5041ca54311b10f9
        }
    }
}