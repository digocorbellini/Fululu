using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class DisposableAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public void Awake()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Release()
    {
        CallOnRelease();
        OnRelease = null; // Clear the event
        transform.parent = AudioManager.instance.transform;
        gameObject.SetActive(false);
        AudioManager.instance.ReturnAudioSource(this);
    }

    public event Action<DisposableAudio> OnRelease;
    public void CallOnRelease() => OnRelease?.Invoke(this);

    public void ReleaseAfter(float seconds)
    {
        StartCoroutine(ReleaseAfterCoroutine(seconds));
    }

    public IEnumerator ReleaseAfterCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Release();
    }
}
