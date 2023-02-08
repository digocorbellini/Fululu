using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericGhostState : State
{
    protected GenericGhostController controller;

    public override void init()
    {
        base.init();

        controller = (GenericGhostController) manager;
    }


}
