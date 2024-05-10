using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

public class PlayerMovementHandler : MonoBehaviour
{
    [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
    [SerializeField] private Transform Weapon = null;

    [SerializeField] private float moveSpeed = 5f;

    public void Move(NetworkInputData data)
    {
        Vector2 moveVector = data.movementInput.normalized;
        playerNetworkRigidbody.Rigidbody.velocity = moveVector * moveSpeed;
    }

    public void SetRotation(Vector2 mousePosition)
    {
        float rotation = Vector2.SignedAngle(Vector2.up, mousePosition - new Vector2(transform.position.x, transform.position.y));
        Weapon.rotation = Quaternion.Euler(Vector3.forward * (rotation + 90));
    }
}