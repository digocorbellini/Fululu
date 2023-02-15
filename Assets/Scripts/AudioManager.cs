using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public DisposableAudio audioSourcePrefab;
    private List<DisposableAudio> audioSourcePool = new List<DisposableAudio>();
    public int initialAudioSourcePoolSize = 5;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < initialAudioSourcePoolSize; i++)
        {
            audioSourcePool.Add(CreateAudioSource());
        }
    }

    private DisposableAudio CreateAudioSource()
    {
        DisposableAudio audioSource = Instantiate<DisposableAudio>(audioSourcePrefab, transform);
        audioSource.gameObject.SetActive(false);
        return audioSource;
    }

    public void ReturnAudioSource(DisposableAudio source)
    {
        audioSourcePool.Add(source);
    }

    public float GetPitch(float pitch, float pitchFluctuation)
    {
        float result = pitch + Random.Range(-pitchFluctuation, pitchFluctuation);
        if (result == 0)
        {
            // Zero pitch means a paused clip :(
            if (pitch > 0)
            {
                result = 0.1f;
            }
            else
            {
                result = -0.1f;
            }
        }
        return result;
    }

    public float GetPitch(SoundEffect sound)
    {
        return GetPitch(sound.pitch, sound.pitchFluctuation);
    }

    public DisposableAudio PlayEffectAt(SoundEffect sound, Vector3 pos)
    {
        if (sound != null && sound.clip != null)
        {
            DisposableAudio source = GetAudioSource();
            source.transform.position = pos;

            AudioSource tempSource = source.audioSource;
            tempSource.clip = sound.clip;
            tempSource.volume = sound.volume;
            tempSource.pitch = GetPitch(sound);
            tempSource.Play();
            source.ReleaseAfter(sound.clip.length / Mathf.Abs(tempSource.pitch));
            return source;
        }
        return null;
    }

    public DisposableAudio GetAudioSource()
    {
        if (audioSourcePool.Count != 0)
        {
            DisposableAudio result = audioSourcePool[0];
            audioSourcePool.RemoveAt(0);
            result.gameObject.SetActive(true);
            return result;
        } else
        {
            DisposableAudio result = CreateAudioSource();
            result.gameObject.SetActive(true);
            return result;
        }
    }
}
