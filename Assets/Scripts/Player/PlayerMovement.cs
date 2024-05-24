using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D _networkRigidbody = null;

    [SerializeField] private float PlayerSpeed = 5f;

    public override void FixedUpdateNetwork()
    {
        // Only move own player
        if (HasStateAuthority == false)
        {
            return;
        }

        // Get input
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Calculate move vector
        Vector2 move = new Vector2(moveX, moveY) * PlayerSpeed;

        _networkRigidbody.Rigidbody.velocity = move;
    }
}