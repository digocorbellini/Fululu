using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;
    public Transform currentPlayerSpawn;
    public PlayerController playerPrefab;
    public PlayerController player;
    public UIManager UIManager;

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
            if (player)
            {
                Destroy(player.gameObject);
            }

            player = Instantiate<PlayerController>(playerPrefab, currentPlayerSpawn.transform.position, currentPlayerSpawn.transform.rotation);
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

    private void Start()
    {
        SpawnPlayer();
    }
}
