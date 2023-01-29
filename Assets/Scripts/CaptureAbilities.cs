using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CaptureAbilties", menuName = "ScriptableObjects/CaptureAbilties")]
public class CaptureAbilities : ScriptableObject
{
    // For UI 
    public string enemyName;
    public Sprite enemySprite;

    // abilities
    public PassiveAbility passive;
    public SacrificeAbility sacrifice;

    public GameObject sacrificeAbility;
}
