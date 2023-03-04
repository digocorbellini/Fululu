using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBugState : State
{
    protected ShieldBugController controller;
    public override string getStateName()
    {
        return "ShieldBugState";
    }

    public override void init()
    {
        base.init();
        controller = (ShieldBugController) manager;
    }
}
