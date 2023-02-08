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

    public float health;

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<EntityHitbox>();
            Initialize(player);
        }
    }

    public void Initialize(PlayerController player)
    {
        Initialize(player.hitbox);
    }
    
    public void Initialize(EntityHitbox player)
    {
        if (this.player)
        {
            this.player.OnHurt -= UpdateDisplay;
        }
        if (player)
        {
            this.player = player;
            Debug.Log("Initializing healthbar. Health: " + player.health);
            health = player.health;
            Debug.Log("player: " + player.health + " health: " + health);
            player.OnHurt += UpdateDisplay;
            UpdateDisplay(0, false);
        }
    }

    private void UpdateDisplay(float damage, bool isExplosive)
    {
        Debug.Log("Update display called. damage: " + damage);
        health -= damage;
        Debug.Log("new health: " + health);

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
