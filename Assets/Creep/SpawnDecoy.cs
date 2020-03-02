using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDecoy : Creep
{
    public override void Start()
    {
        base.Start();
        Invoke("kill", 4);
    }
    void kill()
    {
        gameManager.delayGold(0, transform, this);
    }
    public override void AI()
    {
        //no
    }
}
