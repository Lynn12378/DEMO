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

        private ChangeDetector changes;

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (door != null)
            {
                UpdateDoorState(IsOpen);
                Debug.Log("門的初始狀態: " + (door.activeSelf ? "關閉" : "打開"));
            }
            else
            {
                Debug.LogError("找不到門的GameObject!");
            }
        }

        private void UpdateDoorState(bool isOpen)
        {
            if (door != null)
            {
                door.SetActive(!isOpen);
                Debug.Log("門的狀態: " + (door.activeSelf ? "關閉" : "打開"));
            }
        }

        public void ToggleDoor()
        {
            IsOpen = !IsOpen;
            SetIsOpen_RPC(IsOpen);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetIsOpen_RPC(bool isOpen)
        {
            this.IsOpen = isOpen;
            UpdateDoorState(isOpen);
            Debug.Log("SetIsOpen_RPC 被調用，IsOpen 狀態: " + isOpen);
        }

        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                if (change == nameof(IsOpen))
                {
                    UpdateDoorState(IsOpen);
                    Debug.Log("Render 方法檢測到 IsOpen 狀態變化: " + IsOpen);
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ToggleDoor();
                Debug.Log("P 鍵被按下，嘗試切換門的狀態");
            }
        }
    }
}