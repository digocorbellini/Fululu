using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public Healthbar healthbar;
    public Image reticleRing;
    public Image grazeChargeBar;
    public Image captureImage;
    public PauseUI pauseUI;
    public GourdUI gourdUI;
    public Image screenTint;

    public ParticleSystem chargeBurst;
    public ParticleSystem chargeGlow;

    [HideInInspector]
    public static UIManager instance;

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

    public void Initialize(PlayerController player)
    {
        healthbar.Initialize(player);
        player.SetReticleRing(reticleRing);
        player.SetGrazeChargeBar(grazeChargeBar);
        player.SetChargeParticles(chargeBurst, chargeGlow);
    }

    public void ShowPauseUI()
    {
        pauseUI?.Show();
    }

    public void HidePauseUI()
    {
        pauseUI?.Hide();
    }

    public void TintScreen(Color color, float alpha, float duration)
    {
        screenTint.color = new Color(color.r, color.g, color.b, 0f);
        StartCoroutine(ScreenTintRoutine(alpha, duration));
    }

    private IEnumerator ScreenTintRoutine(float alpha, float duration)
    {
        var tween = screenTint.DOFade(alpha, 0.1f);
        yield return tween.WaitForCompletion();
        yield return new WaitForSeconds(duration);
        tween = screenTint.DOFade(0f, 0.1f);
    }
}
