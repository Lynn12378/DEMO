using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace DEMO.GamePlay
{
    public class DoorController : NetworkBehaviour
    {
        [Networked] public bool IsOpen { get; set; } = false;
        
        [SerializeField] private GameObject door = null;
        private Renderer doorRenderer; // 用于引用门的 Renderer 组件
        private ChangeDetector changes;

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (door != null)
            {
                doorRenderer = door.GetComponent<Renderer>(); // 获取 Renderer 组件
                UpdateDoorState(IsOpen);
                Debug.Log("找到門的GameObject!");
            }
            else
            {
                Debug.LogError("找不到門的GameObject!");
            }
        }

        private void UpdateDoorState(bool isOpen)
        {
            if (doorRenderer != null)
            {
                doorRenderer.enabled = !isOpen; // 控制 Renderer 的可见性
                Debug.Log("門的狀態: " + (doorRenderer.enabled ? "關閉" : "打開"));
                Debug.Log("這裡OK");
            }
            else
            {
                Debug.LogError("找不到Renderer组件!");
            }
        }

        public void ToggleDoor()
        {
            IsOpen = !IsOpen;
            SetIsOpen_RPC(IsOpen);
            Debug.Log("ToggleDoor被调用");
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetIsOpen_RPC(bool isOpen)
        {
            this.IsOpen = isOpen;
            UpdateDoorState(isOpen);
            Debug.LogError("SetIsOpen_RPC被调用，IsOpen状态: " + isOpen);
        }

        #endregion

        #region - OnChanged Events -

        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                if (change == nameof(IsOpen))
                {
                    UpdateDoorState(IsOpen);
                    Debug.LogError("Render方法检测到IsOpen状态变化: " + IsOpen);
                }
            }
        }

        #endregion
    }
}
