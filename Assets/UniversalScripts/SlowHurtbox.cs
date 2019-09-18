using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowHurtbox : Hurtbox {
    public float slowPotency;
    public float slowTime;
    public override bool attackCreep(Creep creepHit)
    {
        creepHit.slow(slowPotency, slowTime);
        return false;
    }
}
