using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexRHitbox : Hurtbox {

    public override bool attackCreep(Creep creepHit)
    {
        if (creepHit.isSlowed)
        {
            creepHit.takeStun(2);
        }
        return base.attackCreep(creepHit);
    }
}
