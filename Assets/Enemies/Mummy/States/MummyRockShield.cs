using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyRockShield : MummyState
{
    public GameObject rockWallObject;
    public Transform rockWallSpawnPos;
    public float startHealth = .25f;
    public float shakeAmount = 5f;
    public float startLagTime = 2f;
    public float maxDamage = 12f;
    public Transform mummyMesh;
    public EnemyFireControl swordRing;

    // TODO: maybe add lancer attack
    public AOERainAttack rainAttack;

    private Vector3 originalPos;
    private Coroutine activeCoroutine;
    private float attackTimer;
    private Animator rockWallsAnim;
    private float originalHealth;
    private bool shakeComplete;
    public override string getStateName()
    {
        return "BRockShield";
    }

    public override void enter()
    {
        if (swordRing != null)
        {
            swordRing.autoFire = false;
        }

        originalPos = mummyMesh.localPosition;

        controller.rb.velocity = Vector3.zero;

        originalHealth = controller.hitbox.health;
        controller.hitbox.OnHurt += HurtListener;

        attackTimer = rainAttack.getTotalAttackTime();

        shakeComplete = false;
        activeCoroutine = StartCoroutine(shakeAbout());
    }

    public override void run()
    {
        attackTimer -= Time.deltaTime;

        if (!shakeComplete)
            return;

        if (attackTimer <= 0)
        {
            attackTimer = rainAttack.getTotalAttackTime();
            rainAttack.attack();
        }
    }

    private void HurtListener(float damage, bool isExplosive, Collider other)
    {
        // check to see if max damage has been taken
        if (controller.hitbox.health <= originalHealth - maxDamage)
        {
            // go to next state
            controller.switchState(controller.GetRandomState());
        }
    }

    private IEnumerator shakeAbout()
    {
        float timer = 0;

        while (timer < startLagTime)
        {
            float randX = Random.Range(-shakeAmount, shakeAmount);
            float randY = Random.Range(-shakeAmount, shakeAmount);
            float randZ = Random.Range(-shakeAmount, shakeAmount);

            mummyMesh.localPosition += new Vector3(randX, randY, randZ);

            timer += Time.deltaTime;

            yield return null;
        }
        mummyMesh.localPosition = originalPos;

        GameObject rocks = Instantiate(rockWallObject, rockWallSpawnPos.position, Quaternion.LookRotation(controller.transform.forward));
        rockWallsAnim = rocks.GetComponent<Animator>();
        rockWallsAnim.Play("raise_anim");
        print("rocks initialized and anim played. State of anim: " + (rockWallsAnim == null));
        yield return null;

        shakeComplete = true;

        activeCoroutine = StartCoroutine(waitForExplosion());
    }

    private IEnumerator waitForExplosion()
    {
        // wait for rock to be destroyed
        while (rockWallsAnim != null && !isDestroyed(rockWallsAnim.gameObject))
        {
            print("rock not yet destroyed");
            yield return null;
        }

        print("ROCK DESTROYED");

        // turn to face player
        controller.transform.LookAt(controller.player.transform.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;

        // go to next state
        controller.switchState(controller.GetRandomState());
    }

    // <summary>
    /// Checks if a GameObject has been destroyed.
    /// </summary>
    /// <param name="gameObject">GameObject reference to check for destructedness</param>
    /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
    private bool isDestroyed(GameObject gameObject)
    {
        // UnityEngine overloads the == opeator for the GameObject type
        // and returns null when the object has been destroyed, but 
        // actually the object is still there but has not been cleaned up yet
        // if we test both we can determine if the object has been destroyed.
        return gameObject == null && !ReferenceEquals(gameObject, null);
    }

    public override void exit()
    {
        if (swordRing != null)
        {
            swordRing.autoFire = true;
        }

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        if (rockWallsAnim != null && !isDestroyed(rockWallsAnim.gameObject))
        {
            Destroy(rockWallsAnim.gameObject);
        }

        controller.hitbox.OnHurt -= HurtListener;
        shakeComplete = false;
    }

    private void OnDestroy()
    {
        controller.hitbox.OnHurt -= HurtListener;
    }
}
