using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummySwordSlam : MummyState
{
    public GameObject swordObject;
    public Transform mummyMesh;
    public EnemyFireControl swordRing;
    [Header("Pre-Attack stats")]
    public float shakeAmount = .01f;
    [Header("Chase stats")]
    public float moveSpeed = 5f;
    public float attackRange = 10f;
    public float rotationSpeed = 5f;
    public float maxTurnTime = 5f;
    public ParticleSystem afterimage;
    [Header("Attack stats")]
    public GameObject attackAreanObject;
    public float endLagTime = 1f;
    public float cameraShakeAmplitude = 2f;
    public float cameraShakeFrequency = 2f;
    public float cameraShakeDuration = 2f;
    public ParticleSystem swordAfterimage;
    public ParticleSystem swordFireParticles;


    private Animator swordAnim;
    private Vector3 originalPos;
    private Coroutine currentCoroutine;
    private Transform player;
    private PlayerController playerCtrl;

    public override string getStateName()
    {
        return "BSwordSlam";
    }

    public override void enter()
    {
        if (swordRing != null)
        {
            swordRing.autoFire = false;
        }

        swordObject.SetActive(false);
        afterimage.Stop();
        afterimage.gameObject.SetActive(false);

        controller.rb.velocity = Vector3.zero;
        swordAnim = swordObject.GetComponent<Animator>();
        playerCtrl = controller.player.GetComponent<PlayerController>();
        originalPos = mummyMesh.localPosition;
        player = controller.player.transform;
        attackAreanObject.SetActive(false); 

        currentCoroutine = StartCoroutine(preAttack());
    }

    // make mummy shake while sword moves out of the ground and above the mummy
    private IEnumerator preAttack()
    {
        print("pre attack");

        // enable sword
        swordObject.SetActive(true);

        //swordAfterimage.Play();

        // raise sword from ground
        swordAnim.Play("raise_sword_anim");
        yield return null;

        // wait for sword raising anim to finish playing
        while (swordAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            float randX = Random.Range(-shakeAmount, shakeAmount);
            float randY = Random.Range(-shakeAmount, shakeAmount);
            float randZ = Random.Range(-shakeAmount, shakeAmount);

            mummyMesh.localPosition += new Vector3(randX, randY, randZ);

            yield return null;
        }

        mummyMesh.localPosition = originalPos;

        // move on to moving towards player
        currentCoroutine = StartCoroutine(turnToFacePlayer());
    }

    private IEnumerator turnToFacePlayer()
    {
        print("turn to face player");

        float timer = 0;

        Vector3 dirToPlayer = player.position - controller.transform.position;
        dirToPlayer.y = 0;
        dirToPlayer.Normalize();
        while (Vector3.Angle(dirToPlayer, controller.transform.forward) > 2f && timer < maxTurnTime)
        {
            Quaternion lookAtPlayer = Quaternion.LookRotation(dirToPlayer);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, lookAtPlayer, Time.deltaTime * rotationSpeed);
            Vector3 rot = controller.transform.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            controller.transform.eulerAngles = rot;

            dirToPlayer = player.position - controller.transform.position;
            dirToPlayer.y = 0;
            dirToPlayer.Normalize();

            timer += Time.deltaTime;
            yield return null;
        }

        currentCoroutine = StartCoroutine(moveTowardsPlayer());
    }

    // move towards player until they are within attack range
    private IEnumerator moveTowardsPlayer()
    {
        print("move towards face player");
        afterimage.gameObject.SetActive(true);
        afterimage.Play();

        // chase player until they are within attack range
        while (Vector3.Distance(player.position, transform.position) > attackRange)
        {
            // move towards player
            Vector3 directionToPlayer = player.position - controller.transform.position;
            directionToPlayer.y = 0;
            directionToPlayer.Normalize();

            controller.rb.velocity = directionToPlayer * moveSpeed;

            // look at player
            controller.transform.LookAt(controller.player.transform.position);
            Vector3 rot = controller.transform.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            controller.transform.eulerAngles = rot;

            yield return null;
        }

        controller.rb.velocity = Vector3.zero;

        currentCoroutine = StartCoroutine(performAttack());
    }

    // swing sword down
    private IEnumerator performAttack()
    {
        print("perform attack");

        // set hurt area on
        attackAreanObject.SetActive(true);
        swordFireParticles.Play();

        yield return null;
        swordAnim.Play("swing_anim");
        yield return null;
        

        // wait for sword swing to end
        while (swordAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        // TODO: play particles on animation end and shake camera
        playerCtrl.ShakeCamera(cameraShakeAmplitude, cameraShakeFrequency, cameraShakeDuration);
        //swordDisapearParticles.Play();
        swordFireParticles.Stop();


        // wait a little for ~impact~
        yield return new WaitForSeconds(endLagTime);

        //swordAfterimage.Stop();
        swordObject.SetActive(false);

        // change state
        controller.switchState(controller.GetRandomState());
    }

    public override void exit()
    {
        // make sword pop out if we exit out of this state before it ends
        if (swordObject.activeSelf)
        {
            // TODO: create particles for this
            //Instantiate(swordDisapearParticles, swordObject.transform.position, Quaternion.identity).transform.parent = null;
        }

        if (swordRing != null)
        {
            swordRing.autoFire = true;
        }

        mummyMesh.localPosition = originalPos;
        attackAreanObject.SetActive(false);
        swordObject.SetActive(false);

        afterimage.Stop();
        afterimage.gameObject.SetActive(false);
        swordAfterimage.Stop();

        controller.rb.velocity = Vector3.zero;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
