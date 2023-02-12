using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component allows a projectile spread to be determined. However,
/// this is indepenedent of the bullet/projectile itself.
/// 
/// This only shoots "IBullet" projectiles.
/// </summary>
public class BulletSpread : MonoBehaviour
{
    [Tooltip("This is the projectile that is meant to be fired in the given spread.")]
    public GameObject Bullet;

    [Header("Shot Stats")]
    public int NumOfBullets = 2;
    public float SpawnRadius = 1;
    public float DistanceBetweenBullets;
    public bool isOffCentered = true;

    [Space(10)]

    [Header("Burst Options")]
    public int numBursts = 1;
    public float timeBetweenBursts = 0.0f;

    public void Fire()
    {
        StartCoroutine(FireCoroutine());
    }

    public void SpawnBullets()
    {
        // angle between consecutive bullets
        float rotationAngle = DistanceBetweenBullets / SpawnRadius;
        // find the center of the circle relative to the starting 
        // object position
        Vector3 centerOfCircle = (isOffCentered) ? transform.position - (transform.forward * SpawnRadius) : transform.position;
        float x = centerOfCircle.x;
        float z = centerOfCircle.z;

        // get position of first bulelt
        float totalDistance = DistanceBetweenBullets * (NumOfBullets - 1);
        float startingRot = -((totalDistance / 2) / SpawnRadius);
        float r00 = Mathf.Cos(startingRot);
        float r01 = -Mathf.Sin(startingRot);
        float r10 = Mathf.Sin(startingRot);
        float r11 = r00;
        Vector3 startingPos = (isOffCentered) ? transform.position : (transform.position) + transform.forward * SpawnRadius;
        float xin1 = startingPos.x;
        float zin1 = startingPos.z;
        // perform rotation so that collection of bullets is centered
        float xin = r00 * xin1 + r01 * zin1 + x - r00 * x - r01 * z;
        float zin = r10 * xin1 + r11 * zin1 + z - r10 * x - r11 * z;

        // spawn the bullets
        float currentRotation = 0;
        for (int i = 0; i < NumOfBullets; i++)
        {
            // find spawn position of bullet in the radius of the circle
            r00 = Mathf.Cos(currentRotation);
            r01 = -Mathf.Sin(currentRotation);
            r10 = Mathf.Sin(currentRotation);
            r11 = r00;
            float newX = r00 * xin + r01 * zin + x - r00 * x - r01 * z;
            float newZ = r10 * xin + r11 * zin + z - r10 * x - r11 * z;



            Vector3 spawnPos = new Vector3(newX, transform.position.y, newZ);
            Quaternion xRot = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
            spawnPos = xRot * spawnPos;
            Vector3 spawnDirection = (spawnPos - centerOfCircle).normalized;
            spawnPos = centerOfCircle + (spawnDirection * SpawnRadius);
            spawnDirection = (spawnPos - centerOfCircle).normalized;
            Transform bullet = Instantiate(Bullet, spawnPos, Quaternion.LookRotation(spawnDirection)).transform;
            // rotate bullet vertically to match parent vertical rotation
            bullet.RotateAround(centerOfCircle, transform.right, transform.rotation.eulerAngles.x);

            currentRotation += rotationAngle;
        }
    }

    public IEnumerator FireCoroutine()
    {
        for(int i = 0; i < numBursts; i++)
        {
            SpawnBullets();
            yield return new WaitForSeconds(timeBetweenBursts);
        }

        Selfdestruct();
    }

    public void Selfdestruct()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Vector3 centerOfCircle = (isOffCentered) ? transform.position - (transform.forward * SpawnRadius) : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerOfCircle, SpawnRadius);

        // angle between consecutive bullets
        float rotationAngle = DistanceBetweenBullets / SpawnRadius;
        // find the center of the circle relative to the starting 
        // object position
        float x = centerOfCircle.x;
        float z = centerOfCircle.z;

        // get position of first bulelt
        float totalDistance = DistanceBetweenBullets * (NumOfBullets - 1);
        float startingRot = -((totalDistance / 2) / SpawnRadius);
        float r00 = Mathf.Cos(startingRot);
        float r01 = -Mathf.Sin(startingRot);
        float r10 = Mathf.Sin(startingRot);
        float r11 = r00;
        Vector3 startingPos = (isOffCentered) ? transform.position : (transform.position) + transform.forward * SpawnRadius;
        float xin1 = startingPos.x;
        float zin1 = startingPos.z;
        // perform rotation so that collection of bullets is centered
        float xin = r00 * xin1 + r01 * zin1 + x - r00 * x - r01 * z;
        float zin = r10 * xin1 + r11 * zin1 + z - r10 * x - r11 * z;

        // spawn the bullets
        float currentRotation = 0;
        for (int i = 0; i < NumOfBullets; i++)
        {
            // find spawn position of bullet in the radius of the circle
            r00 = Mathf.Cos(currentRotation);
            r01 = -Mathf.Sin(currentRotation);
            r10 = Mathf.Sin(currentRotation);
            r11 = r00;
            float newX = r00 * xin + r01 * zin + x - r00 * x - r01 * z;
            float newZ = r10 * xin + r11 * zin + z - r10 * x - r11 * z;

            Vector3 spawnPos = new Vector3(newX, transform.position.y, newZ);
            Quaternion xRot = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
            spawnPos = xRot * spawnPos;
            Vector3 spawnDirection = (spawnPos - centerOfCircle).normalized;
            spawnPos = centerOfCircle + (spawnDirection * SpawnRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(spawnPos, 1);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(centerOfCircle, (spawnPos - centerOfCircle).normalized * (SpawnRadius + 5));

            currentRotation += rotationAngle;
        }
    }
}
