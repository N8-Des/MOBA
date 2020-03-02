using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyHurtbox : Hurtbox {
    public Creep freindCreep;

    public override bool attackCreep(Creep creepHit)
    {
        if (creepHit != freindCreep)
        {
            return base.attackCreep(creepHit);
        } else
        {
            return false;
        }
    }
}
