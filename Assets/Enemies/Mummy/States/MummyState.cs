using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MummyState : State
{
    protected MummyController controller;

    public EnemyFireControl enemyFireControl;

    public override void init()
    {
        base.init();

        controller = (MummyController) manager;
    }
}
