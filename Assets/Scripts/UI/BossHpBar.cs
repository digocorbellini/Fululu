using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    public Image mainBar;
    public Image subBar;

    private float currHP;
    private float maxHP;
    private float mainBarTarget;
    private float subBarDelay;
    private float  subBarTarget;

    public void RegisterBoss(EntityHitbox boss)
    {
        boss.OnHurt += OnHurt;
        boss.OnDeath += OnDeath;

        maxHP = boss.maxHealth;
        currHP = boss.maxHealth;

        mainBarTarget = 1;
        subBarTarget = 1;

        mainBar.fillAmount = 1;
        subBar.fillAmount = 1;
    }

    private void Update()
    {
        if(subBarDelay > 0)
        {
            subBarDelay -= Time.deltaTime;
        }

        if(subBarDelay <= 0)
        {
            // Boss has taken damage for a while
            // Sub bar should catch up with main bar
            subBarTarget = mainBarTarget;
        }

        float lerpVal = .9f;
        if (mainBar.fillAmount - mainBarTarget <= .001f)
        {
            lerpVal = 0;
        }

        float subLerpVal = .98f;
        if (subBar.fillAmount - subBarTarget <= .001f)
        {
            subLerpVal = 0;
        }

        mainBar.fillAmount = Mathf.Lerp(mainBarTarget, mainBar.fillAmount, lerpVal);
        subBar.fillAmount = Mathf.Lerp(subBarTarget, subBar.fillAmount, subLerpVal);
    }

    private void OnHurt(float damage, bool isExplosive, Collider other)
    {
        currHP -= damage;
        mainBarTarget = currHP / maxHP;
        subBarDelay = 1.0f;
    }

    private void OnDeath()
    {
        mainBarTarget = 0.0f;
    }
}
