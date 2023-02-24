using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public string settingName;
    public AudioMixerGroup mixerGroup;

    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.HasKey(settingName))
        {
            volumeSlider.value = PlayerPrefs.GetFloat(settingName);
            mixerGroup.audioMixer.SetFloat(settingName, volumeSlider.value);
        }

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void UpdateVolume(float value)
    {
        mixerGroup.audioMixer.SetFloat(settingName, value);
        PlayerPrefs.SetFloat(settingName, value);
    }
}