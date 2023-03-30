using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SongPlayer : MonoBehaviour
{
    public AudioClip music;
    public float volume = 1.0f;
    public float fadeTime = 0.0f;
    [Tooltip("If true, the song will restart when hitting this trigger")]
    public bool restartIfPlaying;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (restartIfPlaying || AudioManager.GetCurrentMusicClip() != music)
            {
                AudioManager.PlayMusic(music, volume, fadeTime);
            }
        }
    }
}
