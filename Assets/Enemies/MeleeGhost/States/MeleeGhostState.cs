using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGhostState : State { 


    protected MeleeGhostController controller;
    public override string getStateName()
    {
        return "MeleeGhostState";
    }

    public override void init()
    {
        base.init();
        controller = (MeleeGhostController) manager;
    }

}
