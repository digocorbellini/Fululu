using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MummyFlamethrower : MummyState
{
    public GameObject flamePrefab;
    public Transform spawnPosition;

    [Space(8)]
    public float startHP = .6f;
    public float stopHP = .4f;
    public float angularVelocity = 40f;
    public ParticleSystem afterimages;
    public EnemyFireControl swordRing;

    public Dialogue[] startVoicelines;
    public Dialogue[] stopVoicelines;

    private float timer;
    private FlamethrowerPhase phase;
    private Transform arenaCenter;
    private RigidbodyConstraints rbc;

    private FlamethrowerSpin spawned;
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

        GameManager.instance.PlayVoiceLine(startVoicelines[0], 1.0f, 0.0f);
    }

    private void BeginFlamethrowers()
    {

        Debug.Log("Begin flamethrower");
        // Teleport to center
        controller.gameObject.transform.position = arenaCenter.position;
        timer = 2f;
        phase = FlamethrowerPhase.Readying;
        spawned = Instantiate(flamePrefab, spawnPosition.position, Quaternion.identity).GetComponent<FlamethrowerSpin>();
        spawned.angularVelocity = angularVelocity;
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
                
                phase = FlamethrowerPhase.Spinning;
                spawned.StartSpinning();
                afterimages.gameObject.SetActive(false);
            }
        }

        if(phase == FlamethrowerPhase.Spinning)
        {
            if(controller.hitbox.HealthPercent() <= stopHP)
            {
                controller.switchState("BChase");
            }
        }
    }

    public override void exit()
    {
        spawned.StopSpinning();
        controller.rb.freezeRotation = false;
        if (swordRing != null)
        {
            swordRing.autoFire = true;
        }
        GameManager.instance.PlayVoiceLine(stopVoicelines[0], 1.0f, 0.0f);
        controller.rb.constraints = rbc;
    }
}
