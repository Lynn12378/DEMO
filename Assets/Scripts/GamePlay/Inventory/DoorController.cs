using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using DEMO.Manager;

namespace DEMO.GamePlay.Inventory
{
    public class DoorController : NetworkBehaviour
    {
        // 使用Networked属性让IsOpen在网络上同步
        [Networked]
        public bool IsOpen { get; private set; } = false;  // 门的初始状态是关闭的

        private Collider2D doorCollider;

        // 在Awake方法中获取门的Collider2D组件
        private void Awake()
        {
            doorCollider = GetComponent<Collider2D>();
            Debug.LogError(1);
        }

        // 当有物体进入触发器时调用
        private void OnTriggerEnter2D(Collider2D collider)
        {
            UpdateDoorCollision(collider);
            Debug.LogError(2);
        }

        // 当有物体离开触发器时调用
        private void OnTriggerExit2D(Collider2D collider)
        {
            UpdateDoorCollision(collider);
            Debug.LogError(3);
        }

        // 更新门与其他物体的碰撞状态
        private void UpdateDoorCollision(Collider2D collider)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
            {
                // 根据门的状态来忽略或启用碰撞
                Physics2D.IgnoreCollision(collider, doorCollider, IsOpen);
            }
        }

        // 切换门的状态
        private void ToggleDoorState()
        {
            IsOpen = !IsOpen;

            // 遍历所有与门重叠的碰撞体，并更新它们的碰撞状态
            foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position, doorCollider.bounds.size, 0f))
            {
                if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
                {
                    Physics2D.IgnoreCollision(collider, doorCollider, IsOpen);
                    Debug.LogError(4);
                }
            }
        }

        // 使用RPC方法在网络上同步门的开关状态
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void ToggleDoorRpc()
        {
            ToggleDoorState();
            Debug.LogError(5);
        }

        // 当玩家按下按键时调用该方法
        public void ToggleDoor()
        {
            if (Object.HasInputAuthority)
            {
                ToggleDoorRpc();
            }
            else
            {
                Debug.LogError("Local simulation is not allowed to send this RPC.");
            }
        }
    }
}
