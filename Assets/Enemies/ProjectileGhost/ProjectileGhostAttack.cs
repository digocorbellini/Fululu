using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGhostAttack : MonoBehaviour
{
    // TODO: implement leading shots

    public GameObject shot;
    public int numShotsPerBurst = 3;
    public float timeBetweenBurts = 3;
    public float timeBetweenShots = .5f;
    public Transform bulletSpawnLocation;

    public AudioSource shootSource;
    public AudioClip shootSFX;

    // this is a workaround b/c player position is at model's feet
    public float playerPosHeightOffset = 1;

    private Transform player;
    ControllerBase controller;

    public bool isBoss = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if(!isBoss)
            controller = GetComponent<ControllerBase>();

        controller.gameObject.GetComponent<EntityHitbox>().OnDeath += StopShooting;

        StartCoroutine(startShooting());
    }

    public void StopShooting()
    {
        StopAllCoroutines();
    }
    
    public void StartShooting()
    {
        StartCoroutine(startShooting());
    }

    private IEnumerator startShooting()
    {
        while (true)
        {
            if (!controller.isStunned)
            {
                yield return new WaitForSeconds(timeBetweenBurts);
               
                if (controller.isStunned)
                    continue;

                for (int i = 0; i < numShotsPerBurst; i++)
                {
                    if (controller.isStunned)
                        continue;

                    Vector3 dirToPlayer = (player.position + new Vector3(0, playerPosHeightOffset, 0)) - bulletSpawnLocation.position;
                    dirToPlayer.Normalize();

                    Instantiate(shot, bulletSpawnLocation.position, Quaternion.LookRotation(dirToPlayer));

                    if (shootSFX != null && shootSource != null)
                    {
                        shootSource.PlayOneShot(shootSFX);
                    }

                    yield return new WaitForSeconds(timeBetweenShots);
                }
            }
            yield return null;
        }
    }
}
