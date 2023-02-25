using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatState : State
{
    protected GoatController controller;
    public override string getStateName()
    {
        return "GoatState";
    }

    public override void init()
    {
        base.init();
        controller = (GoatController) manager;
    }
}
