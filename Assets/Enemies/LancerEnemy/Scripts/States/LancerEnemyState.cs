using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LancerEnemyState : State
{
    protected LancerEnemyController controller;

    public override void init()
    {
        base.init();

        controller = (LancerEnemyController) manager;
    }
}
