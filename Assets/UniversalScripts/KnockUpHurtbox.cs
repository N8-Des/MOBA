using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockUpHurtbox : Hurtbox {
    public float KnockTime;
    public override bool attackCreep(Creep creepHit)
    {
        if (magicDamage)
        {
            creepHit.takeMagicDamage(damage);
        }else
        {
            creepHit.takeDamage(damage);
        }
        creepHit.knockUp(KnockTime);
        return false;
    }
}
