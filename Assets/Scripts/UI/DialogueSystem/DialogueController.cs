using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class DialogueController : MonoBehaviour
{
    public List<Dialogue> DialogueList = new List<Dialogue>();
    public float timeBetweenAutoPlay = .01f;
    public TextMeshProUGUI subtitleText;
    public bool subtitlesOn = true;

    private AudioSource audioSource;
    [SerializeField] private Dialogue currentDialogue;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        subtitleText.text = "";
    }

    /// <summary>
    /// Set the current dialogue to be played.
    /// </summary>
    /// <param name="dialogueName">The name of the dialogue to be played</param>
    /// <param name="autoPlay">The dialogue will autoplay all of the audio/subtitles 
    /// if this is true. Otherwise, "PlayNextDialogue()" must be called to play
    /// each part of a dialogue system</param>
    /// <returns>True if dialougue with given name is found, false otherwise</returns>
    public bool SetCurrentDialogue(string dialogueName, bool autoPlay = false)
    {
        foreach (Dialogue d in DialogueList)
        {
            if (d.name == dialogueName)
            {
                currentDialogue = d;

                if(autoPlay)
                {
                    //StopAllCoroutines();
                    StartCoroutine(startAutoPlay(d));
                }

                return true;
            }
        }

        return false;
    }

    private IEnumerator startAutoPlay(Dialogue d)
    {
        string nextText;
        AudioClip nextAudio;
        while (currentDialogue.GetNextPair(out nextText, out nextAudio))
        {
            if (subtitlesOn)
                subtitleText.text = nextText;
            audioSource.clip = nextAudio;

            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + timeBetweenAutoPlay);
            audioSource.Stop();
            subtitleText.text = "";
        }

        yield return null;
    }

    /// <summary>
    /// Will play the next audio and subtitles for the given dialogue. This
    /// will skip previous audio/subtitles if called before they are done.
    /// </summary>
    public void PlayNextDialogue()
    {
        if (!currentDialogue)
            return;

        string nextText;
        AudioClip nextAudio;
        currentDialogue.GetNextPair(out nextText, out nextAudio);

        if (subtitlesOn)
            subtitleText.text = nextText;
        audioSource.clip = nextAudio;

        // stop previous audio
        StopAllCoroutines();
           
        // play new audio
        StartCoroutine(playAudio());
    }
    private IEnumerator playAudio()
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length + timeBetweenAutoPlay);

        audioSource.Stop();
        subtitleText.text = "";
    }

    /// <summary>
    /// Gets the current dialogue's name. 
    /// </summary>
    /// <returns>Returns the current dialogue's name. Will return "" if 
    /// current dialogue has not been set yet</returns>
    public string GetCurrentDialogueName()
    {
        if (currentDialogue)
            return currentDialogue.name;

        return "";
    }
}
