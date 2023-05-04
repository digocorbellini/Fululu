using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Healthbar healthbar;
    public Image reticleRing;
    public Image grazeChargeBar;
    public Image captureImage;
    public PauseUI pauseUI;
    public GourdUI gourdUI;
    public Image screenTint;
    public Animator tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialNametag;
    public Image tutorialPortait;

    public ParticleSystem chargeBurst;
    public ParticleSystem chargeGlow;

    [HideInInspector]
    public static UIManager instance;

    private Coroutine screenTintRoutine;

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
        GameManager.instance.OnReset += Reset;
    }

    public void Reset()
    {
        // reset screen tint
        if (screenTintRoutine != null)
            StopCoroutine(screenTintRoutine);
        Color screenTintColor = screenTint.color;
        screenTintColor.a = 0.0f;
        screenTint.color = screenTintColor;
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

    public void ToggleTutorial(bool active)
    {
        if (active)
        {
            tutorialPanel.Play("Open");
        }
        else
        {
            tutorialPanel.Play("Close");
        }
    }

    public void SetTutorialText(string text)
    {
        tutorialText.text = text;
    }

    public void SetPotraitImage(Sprite img)
    {
        if(img != null)
        {
            tutorialPortait.sprite = img;
        }
    }

    public void SetTutorialNametag(string text)
    {
        if(!string.IsNullOrWhiteSpace(text))
        {
            tutorialNametag.text = text;
        }
    }

    public void TintScreen(Color color, float alpha, float duration)
    {
        //screenTint.color = new Color(color.r, color.g, color.b, 0f);
        StartCoroutine(ScreenTintRoutine(alpha, duration));
    }

    public void SetScreenTint(float alpha)
    {
        if (screenTint.color.a == alpha)
            return;

        if (screenTintRoutine != null)
        {
            StopCoroutine(screenTintRoutine);
        }

        screenTintRoutine = StartCoroutine(ScreenTintRoutine(alpha));
    }

    private IEnumerator ScreenTintRoutine(float alpha)
    {
        var tween = screenTint.DOFade(alpha, 0.1f);
        yield return tween.WaitForCompletion();
    }

    private IEnumerator ScreenTintRoutine(float alpha, float duration)
    {
        var tween = screenTint.DOFade(alpha, 0.1f);
        yield return tween.WaitForCompletion();
        tween = screenTint.DOFade(0f, duration);
        yield return new WaitForSeconds(duration);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnReset -= Reset;
    }
}
