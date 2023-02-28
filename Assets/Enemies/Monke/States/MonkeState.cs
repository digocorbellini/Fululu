using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonkeState : State
{
    protected MonkeController controller;

    public override void init()
    {
        base.init();

        controller = (MonkeController)manager;
    }
}
