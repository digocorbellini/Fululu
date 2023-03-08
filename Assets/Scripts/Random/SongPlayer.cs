using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class SongPlayer : MonoBehaviour
{
    private AudioSource musicSource;
    private bool isPlaying = false;
    private Transform player;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if(col.bounds.Intersects(new Bounds(player.position, Vector3.one)))
        {
            if (!isPlaying)
            {
                musicSource.Play();
                isPlaying = true;
            }
        }
        else
        {
            if(isPlaying)
            {
                musicSource.Stop();
                isPlaying = false;
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == 6 && !isPlaying)
    //    {
    //        musicSource.Play();
    //        isPlaying = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.layer == 6 && isPlaying)
    //    {
    //        if (col.bounds.Intersects(new Bounds(player.position, Vector3.one)))
    //        {
    //            return;
    //        }

    //        musicSource.Stop();
    //        isPlaying = false;
    //    }
    //}
}
