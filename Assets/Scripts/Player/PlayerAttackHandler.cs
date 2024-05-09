using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

using Fusion;

public class PlayerAttackHandler : NetworkBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootPoint = null;


    public void Shoot(Vector3 mousePosition)
    {
        Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles - Vector3.forward * 90);
        bulletPrefab.mousePosition = mousePosition;
        Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority);
    }
}