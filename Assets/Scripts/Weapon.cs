using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeapon", menuName = "ScriptableObjects/PlayerWeapon")]
public class Weapon : ScriptableObject
{
    public Sprite captureImage;

    [Header("Firing Options")]
    public float chargeTime = 1.0f;
    public float rapidFireCooldown = 0.0f;
    public float chargeFireCooldown = 0.0f;

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

    private void SpawnBullets(Transform transform, Vector3? target, BulletSpread spread, BulletBase bullet)
    {
        BulletSpread instance = Instantiate(spread, transform);

        if (target != null)
        {
            Vector3 targetNonNull = target.Value;
            instance.transform.LookAt(targetNonNull);
        }

        instance.Bullet = bullet.gameObject;
        instance.Fire();
    }

    public bool ChargedFire(Transform shootPos, Vector3? target)
    {
        SpawnBullets(shootPos, target, spreadPrefab, chargedBullet);

        return true;
    }

    public bool Fire(Transform shootPos, Vector3? target)
    {
        SpawnBullets(shootPos, target, spreadPrefab, bullet);

        return true;
    }

    public bool Sacrifice(Transform transform)
    {
        if (sacrificeSpreadPrefab && sacrificeBullet)
        {
            SpawnBullets(transform, null, sacrificeSpreadPrefab, sacrificeBullet);
            return true;
        }

        return false;
    }
}
