using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false; // 門的開關狀態

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家進入，開門
            OpenDoor();
        }
        else if (other.CompareTag("Zombie") && isOpen)
        {
            // 殭屍進入且門是開著的
            AllowZombieEnterShelter();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家離開，關門
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        // 播放開門動畫或其他效果
        Debug.Log("Door is opened.");
    }

    private void CloseDoor()
    {
        isOpen = false;
        // 播放關門動畫或其他效果
        Debug.Log("Door is closed.");
    }

    private void AllowZombieEnterShelter()
    {
        // 殭屍進入SHELTER的處理邏輯
        Debug.Log("Zombie entered the shelter!");
    }
}
