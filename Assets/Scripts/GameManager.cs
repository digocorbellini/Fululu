using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;
    public static bool isPaused = false;
    public Transform currentPlayerSpawn;
    public PlayerController playerPrefab;
    public PlayerController player;
    public UIManager UIManager;

    public event Action OnUnpause;
    public void CallOnUnpause() => OnUnpause?.Invoke();

    public void Awake()
    {
        if(!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnPlayer()
    {
        if (playerPrefab && currentPlayerSpawn)
        {
            if (!player)
            {
                player = Instantiate<PlayerController>(playerPrefab, currentPlayerSpawn.transform.position, currentPlayerSpawn.transform.rotation);
                player.hitbox.OnDeath += OnPlayerDeath;
            } else
            {
                Debug.Log("Reviving and respawning player");
                player.Revive();
                player.controller.enabled = false;
                player.transform.SetPositionAndRotation(currentPlayerSpawn.transform.position, currentPlayerSpawn.transform.rotation);
                player.controller.enabled = true;
            }

            UIManager.Initialize(player);
            
        } else
        {
            if (!playerPrefab)
            {
                Debug.LogError("Failed to spawn player due to missing player prefab");
            }

            if (!currentPlayerSpawn)
            {
                Debug.LogError("Failed to spawn player due to missing player spawn");
            }
        }
    }

    public void TogglePause()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            isPaused = true;
        } else
        {
            Time.timeScale = 1;
            isPaused = false;
            CallOnUnpause();
        }
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void OnPlayerDeath()
    {
        SpawnPlayer();
    }
}
