using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial", menuName = "ScriptableObjects/Tutorial")]
public class Tutorial : ScriptableObject
{
    public string[] dialogue;

    [Header("Leave blank if no changes")]
    public string[] names;
    public Sprite[] potraits;
    public Dialogue[] voiceLines;

    public Dialogue afterVoiceLine;


    private void OnValidate()
    {
        int length = dialogue.Length;

        if(names.Length < length)
        {
            string[] temp = new string[length];
            Array.Copy(names, temp, names.Length);
            names = temp;
        }

        if (potraits.Length < length)
        {
            Sprite[] temp = new Sprite[length];
            Array.Copy(potraits, temp, potraits.Length);
            potraits = temp;
        }

        if (voiceLines.Length < length)
        {
            Dialogue[] temp = new Dialogue[length];
            Array.Copy(voiceLines, temp, voiceLines.Length);
            voiceLines = temp;
        }
    }
}
