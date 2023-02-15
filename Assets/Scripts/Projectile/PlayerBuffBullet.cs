using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffBullet : BulletBase
{
    [Space(10)]
    public PlayerBuffManager.PlayerBuffs type;
    public float multiplier;
    public float buffDuration;

    private void Start()
    {
        PlayerBuffManager player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBuffManager>();
        player.BuffPlayer(type, buffDuration, multiplier);

        Destroy(gameObject, 0.1f);
    }
}
