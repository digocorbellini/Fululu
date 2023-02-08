using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AoeGhostState : State
{
    protected AoeGhostController controller;

    public override void init()
    {
        base.init();

        controller = (AoeGhostController) manager;
    }
}
