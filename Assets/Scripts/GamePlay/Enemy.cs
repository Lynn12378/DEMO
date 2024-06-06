using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;
using DEMO.GamePlay.Player;
using Unity.Jobs.LowLevel.Unsafe;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D enemyNetworkRigidbody = null;
    private int causeDamage = 20;

    [SerializeField] public Slider hPSlider;

    [Networked] [OnChangedRender(nameof(HandleHpChanged))]
    public int Hp { get; set; }
    private int maxHp = 50;

    // Initialize
    public override void Spawned() 
    {
        Hp = maxHp;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var netObj = collision.collider.GetComponent<NetworkBehaviour>().Object;

        if (netObj == null) return;

        var player = collision.collider.GetComponent<PlayerController>();

        if (player != null && netObj.InputAuthority != Object.InputAuthority)
        {
            if(netObj.CompareTag("Player"))
            {
                player.TakeDamage(causeDamage);
            }
        }
    }

    #region - Hp -
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        SetEnemyHP_RPC(Hp);
        if (Hp <= 0)
        {
            DespawnEnemy_RPC(Object);
        }
    }

    public void HandleHpChanged()
    {
        hPSlider.value = Hp;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetEnemyHP_RPC(int hp)
    {
        Hp = hp;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DespawnEnemy_RPC(NetworkObject netObj)
    {
        Runner.Despawn(netObj);
    }
    #endregion
}
