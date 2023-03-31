using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigil : MonoBehaviour
{
    public float threshold;
    public Color inactive;
    public Color active;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (GameManager.instance.player)
        {
            GameManager.instance.player.GetComponent<PlayerController>().OnBroadcastCharge += UpdateSigil;
        }

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = inactive;
        
    }

    private void UpdateSigil(float amount)
    {
        if(amount < threshold)
        {
            SetColor(inactive);
        }
        else
        {
            SetColor(active);
        }
    }

    private void SetColor(Color c)
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = c;
        }
    }
}
