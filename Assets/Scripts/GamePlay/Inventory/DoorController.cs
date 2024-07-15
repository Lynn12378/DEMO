using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace DEMO.GamePlay
{
    public class DoorController : NetworkBehaviour
    {
        // 定義網絡變量 IsOpen，初始值為false，表示門是關閉的
        [Networked] public bool IsOpen { get; set; } = false;
        
        // 定義序列化字段 door，用於引用門的遊戲對象
        [SerializeField] private GameObject door = null;

        // 用於檢測變化的 ChangeDetector
        private ChangeDetector changes;

        // 當物體生成時調用
        public override void Spawned()
        {
            // 初始化 ChangeDetector，來源為模擬狀態
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            // 檢查門對象是否存在，並設置初始狀態
            if (door != null)
            {
                UpdateDoorState(IsOpen);
            }
            else
            {
                Debug.LogError("找不到門的GameObject!");
            }
        }

        // 更新門的狀態，根據 isOpen 設置門的活動狀態
        private void UpdateDoorState(bool isOpen)
        {
            if (door != null)
            {
                door.SetActive(!isOpen); // 如果 isOpen 為真，設置門為非活動；如果 isOpen 為假，設置門為活動
                Debug.Log("門的狀態: " + (door.activeSelf ? "關閉" : "打開"));
            }
        }

        // 切換門的狀態，調用 RPC 方法
        public void ToggleDoor()
        {
            IsOpen = !IsOpen;
            SetIsOpen_RPC(IsOpen);
        }

        #region - RPCs -

        // 定義遠程過程調用 (RPC) 方法，用於設置門的開啟狀態
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetIsOpen_RPC(bool isOpen)
        {
            this.IsOpen = isOpen;
            UpdateDoorState(isOpen);
            Debug.LogError("SetIsOpen_RPC被調用，IsOpen狀態: " + isOpen);
        }

        #endregion

        #region - OnChanged Events -

        // 渲染方法，每幀調用，用於檢測變量變化並更新門的狀態
        public override void Render()
        {
            // 檢測 IsOpen 變量的變化
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                if (change == nameof(IsOpen)) // 如果 IsOpen 變量發生變化
                {
                    UpdateDoorState(IsOpen); // 更新門的狀態
                    Debug.LogError("Render方法檢測到IsOpen狀態變化: " + IsOpen); // 輸出變化調試信息
                }
            }
        }

        #endregion
    }
}
