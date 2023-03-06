using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("order of audio and text lists must match and they must be the same length.")]
    public List<AudioClip> DialogueAudio = new List<AudioClip>();
    public List<string> DialogueText = new List<string>();

    [Header("This name will be used to access what dialogue to play")]
    public string DialogueName;

    private int index = 0;

    /// <summary>
    /// Get the next audio and text needed for this dialogue.
    /// </summary>
    /// <param name="text">the next string to be displayed in the subtitles. "" if no next available</param>
    /// <param name="audio">the next audio to be played. Null if no next available</param>
    /// <returns>True if we have any pairs left, otherwise false</returns>
    public bool GetNextPair(out string text, out AudioClip audio)
    {
        if (index >= DialogueAudio.Count || index >= DialogueText.Count)
        {
            Debug.Log("GET NEXT PAIR IS FALSE. index = " + index);
            text = "";
            audio = null;
            return false;
        }

        text = DialogueText[index];
        audio = DialogueAudio[index];

        index++;
        return true;
    }

    /// <summary>
    /// Reset this dialogue to start playing from the start
    /// </summary>
    public void Reset()
    {
        index = 0;
    }
}
