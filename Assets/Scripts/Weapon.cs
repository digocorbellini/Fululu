using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeapon", menuName = "ScriptableObjects/PlayerWeapon")]
public class Weapon : ScriptableObject
{
    public BulletSpread bulletPrefab;
    public float fireCooldown = 0.1f;

    private float lastShot = 0.0f;

    private void OnEnable()
    {
        lastShot = 0.0f;
    }

    public bool Fire(Transform transform)
    {
        if (Time.time < lastShot + fireCooldown)
        {
            return false;
        }

        Instantiate(bulletPrefab, transform);

        lastShot = Time.time;

        return true;
    }
}
