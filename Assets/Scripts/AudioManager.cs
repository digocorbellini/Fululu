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
