using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    public EntityHitbox player;

    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    private float health;

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<EntityHitbox>();
            Initialize(player);
        }
    }

    private void OnDeath()
    {
        health = 0;
        UpdateDisplay(0, false);
    }

    public void Initialize(PlayerController player)
    {
        Initialize(player.hitbox);
    }
    
    public void Initialize(EntityHitbox player)
    {
        if (player)
        {
            this.player = player;
            health = player.health;
            player.OnHurt += UpdateDisplay;
            player.OnDeath += OnDeath;
            UpdateDisplay(0, false);
        }
    }

    private void UpdateDisplay(float damage, bool isExplosive)
    {
        health -= damage;

        int i = 0;
        for(; i < health; i++)
        {
            hearts[i].sprite = fullHeart;
        }

        for(; i<hearts.Length; i++)
        {
            hearts[i].sprite = emptyHeart;
        }
        
    }
}
