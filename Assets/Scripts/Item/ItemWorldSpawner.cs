using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemWorldSpawner : NetworkBehaviour
{
    public Item item;

    /*public override void Spawned()
    {
        ItemWorld.SpawnItemWorld(transform.position, item);

        Runner.Despawn(Object);
    }*/
}

