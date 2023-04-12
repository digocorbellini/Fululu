using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrippleBulletRing : BulletBase
{
    [Header("Tripple Bullet Ring Stats")]
    public BulletSpread trippleRadialSpread;
    public BulletBase bullet;

    private List<BulletBase> spawnedBullets = new List<BulletBase>();
    // Start is called before the first frame update
    void Start()
    {
        // make bullet be parallel to the ground
        Vector3 newForward = transform.forward;
        newForward.y = 0;
        newForward.Normalize();
        transform.forward = newForward;

        BulletSpread instance = Instantiate(trippleRadialSpread, transform.position, Quaternion.LookRotation(transform.forward));
        instance.Bullet = bullet;
        instance.Fire(ref spawnedBullets);

        Destroy(this.gameObject);
    }
}
