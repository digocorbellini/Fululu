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

    public event Action OnReset;
    public void CallOnReset() => OnReset?.Invoke();

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
            UIManager.ShowPauseUI();
            UnlockCursor();
        } else
        {
            LockCursor();
            Time.timeScale = 1;
            isPaused = false;
            CallOnUnpause();
            UIManager.HidePauseUI();
        }
    }

    public void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Give the player some charge for hitting stuff
    public void OnHitGrazeCharge(float mult = 1.0f)
    {
        player.OnHitCharge(mult);
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void OnPlayerDeath()
    {
        StartCoroutine(HandlePlayerDeath());
    }

    private IEnumerator HandlePlayerDeath()
    {
        yield return new WaitUntil(() => player.isAnimationDone("Death"));
        yield return new WaitForSeconds(1.0f); // Suspenseful pause :o
        CallOnReset();
        SpawnPlayer();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
