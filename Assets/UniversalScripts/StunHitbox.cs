using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHitbox : Hurtbox {
    public float duration;
    public override bool attackCreep(Creep creepHit)
    {
        creepHit.takeStun(duration);
        return false;
    }
}
