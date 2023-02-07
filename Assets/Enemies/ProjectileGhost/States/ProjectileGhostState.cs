using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileGhostState : State
{
    protected ProjectileGhostController controller;

    public override void init()
    {
        base.init();

        controller = (ProjectileGhostController) manager;
    }


}
