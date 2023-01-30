using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeapon", menuName = "ScriptableObjects/PlayerWeapon")]
public class Weapon : ScriptableObject
{
    [Header("Firing Options")]
    public float chargeTime = 1.0f;

    [Tooltip("Determines number and positioning of bullets each shot")]
    public BulletSpread spreadPrefab;
    
    [Tooltip("Fired when weapon is not fully charged")]
    public BulletBase bullet;

    [Tooltip("Fired when the weapon is fully charged")]
    public BulletBase chargedBullet;

    [Space(10)]

    [Header("Sacrifice Options")]

    [Tooltip("Determines number and positioning of bullets when sacrificing the weapon")]
    public BulletSpread sacrificeSpreadPrefab;

    [Tooltip("Fired when sacrificing the weapon")]
    public BulletBase sacrificeBullet;

    private void SpawnBullets(Transform transform, BulletSpread spread, BulletBase bullet)
    {
        BulletSpread instance = Instantiate(spread, transform);

        instance.Bullet = bullet.gameObject;
    }

    public bool ChargedFire(Transform transform)
    {
        SpawnBullets(transform, spreadPrefab, bullet);

        return true;
    }

    public bool Fire(Transform transform)
    {
        SpawnBullets(transform, spreadPrefab, chargedBullet);

        return true;
    }

    public bool Sacrifice(Transform transform)
    {
        if (sacrificeSpreadPrefab && sacrificeBullet)
        {
            SpawnBullets(transform, sacrificeSpreadPrefab, sacrificeBullet);
            return true;
        }

        return false;
    }
}
