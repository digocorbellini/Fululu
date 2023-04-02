using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MummyFlamethrower : MummyState
{
    public GameObject flameRoot;
    public ParticleSystem[] particles;
    public Collider[] colliders;

    [Space(8)]
    public float startHP = .6f;
    public float stopHP = .4f;
    public float angularVelocity = 40f;
    public ParticleSystem afterimages;
    public EnemyFireControl swordRing;

    private float timer;
    private FlamethrowerPhase phase;
    private Transform arenaCenter;
    private RigidbodyConstraints rbc;

    private enum FlamethrowerPhase
    {
        Waiting,
        Readying,  
        Spinning
    }

    private void Awake()
    {
        arenaCenter = GameObject.FindGameObjectWithTag("Center").transform;
    }

    public override string getStateName()
    {
        return "BFlamethrower";
    }

    public override void enter()
    {
        afterimages.gameObject.SetActive(true);
        phase = FlamethrowerPhase.Waiting;
        controller.rb.velocity = Vector3.zero;
        timer = .3f;
        if (swordRing != null)
        {
            swordRing.autoFire = false;
        }
    }

    private void BeginFlamethrowers()
    {

        Debug.Log("Begin flamethrower");
        // Teleport to center
        controller.gameObject.transform.position = arenaCenter.position;
        timer = 2f;
        phase = FlamethrowerPhase.Readying;
        flameRoot.SetActive(true);
        colliders.ToList().ForEach(collider => collider.enabled = false);
        particles.ToList().ForEach(particleSystem => particleSystem.Play());
        rbc = controller.rb.constraints;
        controller.rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public override void run()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            if(phase == FlamethrowerPhase.Waiting)
            {
                BeginFlamethrowers();
            }
            else if(phase == FlamethrowerPhase.Readying)
            {
                colliders.ToList().ForEach(collider => collider.enabled = true);
                phase = FlamethrowerPhase.Spinning;
                afterimages.gameObject.SetActive(false);
            }
        }

        if(phase == FlamethrowerPhase.Spinning)
        {
            flameRoot.transform.eulerAngles += new Vector3(0, angularVelocity * Time.deltaTime, 0);

            if(controller.hitbox.HealthPercent() <= stopHP)
            {
                controller.switchState("BChase");
            }
        }
    }

    public override void exit()
    {
        colliders.ToList().ForEach(collider => collider.enabled = false);
        particles.ToList().ForEach(particleSystem => particleSystem.Stop());
        controller.rb.freezeRotation = false;
        if (swordRing != null)
        {
            swordRing.autoFire = true;
        }
        controller.rb.constraints = rbc;
    }
}
