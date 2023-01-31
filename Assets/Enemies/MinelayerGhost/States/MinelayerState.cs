using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinelayerState : State
{
    protected MinelayerController controller;

    public override string getStateName()
    {
        return "MLState";
    }

    public override void init()
    {
        base.init();
        controller = (MinelayerController) manager;
    }

}
