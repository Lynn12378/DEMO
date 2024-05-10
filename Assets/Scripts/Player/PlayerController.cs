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

    public LayerMask itemsLayerMask;

    void Start()
    {
        itemsLayerMask = LayerMask.GetMask("ItemsLayer");
    }

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
        movementHandler.SetRotation(data.rotation);

        if (pressed.IsSet(InputButtons.FIRE))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                attackHandler.Shoot(Input.mousePosition);
            }
        }
        if (pressed.IsSet(InputButtons.PICKUP))
        {
            HandlePickUp();
        }
    }

    private void HandlePickUp()
    {
        Debug.Log("Into HandlePickUp");
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Raycast to find the item on top
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, itemsLayerMask);
        Debug.Log(hit.collider);

        if (hit.collider != null && hit.collider.CompareTag("ItemsInteractable"))
        {
            Debug.Log("In if hit.collider");
            ItemPickup itemPickup = hit.collider.GetComponent<ItemPickup>();
            ItemWorld itemWorld = hit.collider.GetComponent<ItemWorld>();
            Item item = itemWorld.GetItem();

            if (itemPickup != null)
            {
                itemPickup.PickUp(item);
            }
        }
    }
}

