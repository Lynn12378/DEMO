using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Fusion;
using Fusion.Addons.Physics;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
    [SerializeField] private PlayerMovementHandler movementHandler = null;
    [SerializeField] private PlayerAttackHandler attackHandler = null;


    [Networked] private NetworkButtons buttonsPrevious { get; set; }

    private int maxHp = 100;
    private HealthPoint healthPoint = null;

    public override void Spawned() // Initialize
    {
        if (Object.HasStateAuthority)
        {
            healthPoint =  /*FindObjectOfType*/GetComponentInChildren<HealthPoint>();
            healthPoint.Hp = maxHp;
        }
    }

    private void Respawn() // When restart
    {
        playerNetworkRigidbody.transform.position = Vector3.zero;
        healthPoint.Hp = maxHp;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            ApplyInput(data);
        }

        if (healthPoint.Hp <= 0)
        {
            Respawn();
        }
    }

    private void ApplyInput(NetworkInputData data)
    {
        NetworkButtons buttons = data.buttons;
        var pressed = buttons.GetPressed(buttonsPrevious);
        buttonsPrevious = buttons;

        movementHandler.Move(data);
        movementHandler.SetRotation(data.rotation);

        if (pressed.IsSet(InputButtons.FIRE))
        {
            attackHandler.Shoot();
        }

        if (pressed.IsSet(InputButtons.SPACE))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            healthPoint.Hp -= damage;
        }
    }
}

