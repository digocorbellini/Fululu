using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundEffect
{
    public AudioClip clip;
    [Range(0f, 3f)]
    public float volume = 1.0f;
    [Range(-3f, 3f)]
    public float pitch = 1.0f;
    [Range(0f, 3f)]
    public float pitchFluctuation;
    public AudioMixerGroup outputAudioMixerGroup;
}

public class AudioComponent : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioMixerGroup animEventAudioMixerGroup;
    
    private List<DisposableAudio> disposableSources = new List<DisposableAudio>();

    private void Awake()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
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
            } else
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

    public void PlaySound(SoundEffect sound)
    {
        if(sound == null)
        {
            return;
        }
        if(sound.clip != null)
        {
            if (sound.pitchFluctuation != 0 || sound.pitch != audioSource.pitch)
            {
                PlayDisposable(sound, false);
            } else
            {
                audioSource.PlayOneShot(sound.clip, sound.volume);
            }
        }
    }

    public void PlaySoundAnim(AnimationEvent sound)
    {
        
        if(sound.objectReferenceParameter is AudioClip clip)
        {
            float volume = sound.floatParameter;
            if (volume == 0f)
            {
                volume = 1f;
            }
            float pitch = sound.intParameter * .1f;
            if (pitch == 0f)
            {
                pitch = 1f;
            }
            float pitchFluctuation = 0.0f;
            float.TryParse(sound.stringParameter, out pitchFluctuation);

            SoundEffect effect = new SoundEffect();
            effect.clip = clip;
            effect.volume = volume;
            effect.pitch = pitch;
            effect.pitchFluctuation = pitchFluctuation;
            effect.outputAudioMixerGroup = animEventAudioMixerGroup;

            PlaySound(effect);
        }
    }

    public void PlayDisposable(SoundEffect sound, bool independent = true)
    {
        DisposableAudio source = AudioManager.instance.PlayEffectAt(sound, transform.position);
        if (!independent)
        {
            source.transform.parent = transform;
            disposableSources.Add(source);
            source.OnRelease += RemoveSource;
        }
    }

    private void RemoveSource(DisposableAudio source)
    {
        disposableSources.Remove(source);
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying || disposableSources.Count > 0;
    }

    /// <summary>
    /// Destroys the gameobject this script is attached to.
    /// Useful as an animation event
    /// </summary>
    public void DestroyGameObject()
    {
        foreach (DisposableAudio source in disposableSources)
        {
            source.Release();
        }
        Destroy(gameObject);
    }
}
