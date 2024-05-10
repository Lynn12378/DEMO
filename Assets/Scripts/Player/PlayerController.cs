using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Fusion;
using Fusion.Addons.Physics;

public class PlayerController : NetworkBehaviour
{
    //[SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
    [SerializeField] private PlayerMovementHandler movementHandler = null;
    [SerializeField] private PlayerAttackHandler attackHandler = null;
    //[SerializeField] private PlayerStats playerStats = null;


    [Networked] private NetworkButtons buttonsPrevious { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            ApplyInput(data);
        }
    }

    private void ApplyInput(NetworkInputData data)
    {
        NetworkButtons buttons = data.buttons;
        var pressed = buttons.GetPressed(buttonsPrevious);
        buttonsPrevious = buttons;

        movementHandler.Move(data);
        movementHandler.SetRotation(data.mousePosition);

        if (pressed.IsSet(InputButtons.FIRE))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                attackHandler.Shoot(data.mousePosition);
            }
        }
    }
}

