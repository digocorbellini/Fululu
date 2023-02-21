using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeEffects : MonoBehaviour
{
    [SerializeField] private Image grazeChargeBar;
    [SerializeField] private ParticleSystem chargeBurst;
    [SerializeField] private ParticleSystem chargeGlow;
    [SerializeField] private ParticleSystem chargePlayerParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip audio33;
    [SerializeField] private AudioClip audio66;
    [SerializeField] private AudioClip audio100;

    private int chargeStage = 0;

    public void UpdateGrazeUI(float chargePercent = 0f)
    {
        grazeChargeBar.fillAmount = chargePercent;

        if(chargeStage == 0 && chargePercent >= .33)
        {
            chargeStage = 1;
            source.PlayOneShot(audio33);
        }
        else if(chargeStage == 1 && chargePercent >= .66f)
        {
            chargeStage = 2;
            source.PlayOneShot(audio66);
        }
        else if(chargeStage == 2 && chargePercent >= 1.0f)
        {
            chargeStage = 3;
            source.PlayOneShot(audio100);
            chargeBurst.Play();
            chargePlayerParticles.Play();
            chargeGlow.enableEmission = true;
        }
        
    }

    public void OnChargeConsumed(float chargePercentage = 0.0f)
    {
        // Figure out what charge stage we should go back to

        chargeGlow.enableEmission = false;

        if (chargePercentage >= .66f)
        {
            chargeStage = 2;
        }
        else if(chargePercentage >= .33f)
        {
            chargeStage = 1;
        }
        else
        {
            chargeStage = 0;
        }
    }

    public void InitBar(Image img)
    {
        grazeChargeBar = img;
        UpdateGrazeUI();
    }

    public void InitUIParticles(ParticleSystem burst, ParticleSystem glow)
    {
        chargeBurst = burst;
        chargeGlow = glow;
        chargeGlow.enableEmission = false;
    }
}
