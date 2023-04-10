using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerVoiceLines : MonoBehaviour
{
    public enum VoiceLineType
    {
        Jump,
        Dash,
        Hurt,
        HurtCrit,   // Hurt when reduced to 1 HP
        Capture,
        CapCrit,    // Capture when at 1 HP
        CapWeak     // Capture weak enemies (gremlins and single shot ghosts)

        // Sacrifice voice lines stored per capture weapon
        // Tutorials may have voice lines too
    }

    public AudioSource source;
    public TextMeshProUGUI subtitle;
    // Min type between each voiceline
    public float globalCooldown= 5.0f;
    private float timer;
    private bool subtitleVisible;
    private bool coroutineActive;

    [Range(0, 1f)]
    public float jumpChance;
    public Dialogue[] jump;
    [Space(5)]

    [Range(0, 1f)]
    public float dashChance;
    public Dialogue[] dash;
    [Space(5)]

    [Range(0, 1f)]
    public float hurtChance;
    public Dialogue[] hurt;
    [Space(5)]

    [Range(0, 1f)]
    public float critHurtChance;
    public Dialogue[] critHurt;
    [Space(5)]

    [Range(0, 1f)]
    public float capChance;
    public Dialogue[] capture;
    [Space(5)]

    [Range(0, 1f)]
    public float critCapChance;
    public Dialogue[] critCapture;

    [Range(0, 1f)]
    public float weakCapChance;
    public Dialogue[] weakCap;

    private void Update()
    {
        if(subtitleVisible && timer <= 3.0f)
        {
            subtitleVisible = false;
            if (subtitle) subtitle.text = "";
        }
        if(timer >= 0f)
        {
            timer -= Time.deltaTime;
        }
    }

    private void Start()
    {
        subtitle = GameObject.FindGameObjectWithTag("Subtitle").GetComponent<TextMeshProUGUI>();
    }



    public void PlayVoicelineRandom(VoiceLineType type)
    {
        switch (type)
        {
            case VoiceLineType.Jump:
                PlayLine(jumpChance, jump);
                break;
            case VoiceLineType.Dash:
                PlayLine(dashChance, dash);
                break;
            case VoiceLineType.Hurt:
                PlayLine(hurtChance, hurt);
                break;
            case VoiceLineType.HurtCrit:
                PlayLine(critHurtChance, critHurt);
                break;
            case VoiceLineType.Capture:
                PlayLine(capChance, capture, 1.5f);
                break;
            case VoiceLineType.CapCrit:
                PlayLine(critCapChance, critCapture, 1.5f);
                break;
            case VoiceLineType.CapWeak:
                PlayLine(weakCapChance, weakCap, 1.5f);
                break;
            default:
                break;
        }
    }

    public void PlayLine(float chance, Dialogue[] lines, float delay = 0f)
    {
        if (Random.Range(0.0f, 1.0f) > chance) return;
     
        Dialogue d = lines[Random.Range(0, lines.Length)];

        if (!coroutineActive)
        {
            StartCoroutine(PlayLineAfter(d, delay));
        }
        
    }

    private IEnumerator PlayLineAfter(Dialogue line, float delay)
    {
        coroutineActive = true;
        yield return new WaitForSecondsRealtime(delay);

        if (source) source.PlayOneShot(line.DialogueAudio[0]);
        if (subtitle) subtitle.text = line.DialogueText[0];
        subtitleVisible = true;
        timer = globalCooldown;
        coroutineActive = false;
    }
}
