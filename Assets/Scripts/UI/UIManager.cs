using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Healthbar healthbar;
    public Image reticleRing;
    public Image grazeChargeBar;
    public Image captureImage;
    public PauseUI pauseUI;
    public GourdUI gourdUI;

    public ParticleSystem chargeBurst;
    public ParticleSystem chargeGlow;

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
}
