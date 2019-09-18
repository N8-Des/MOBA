using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicSpecialHitbox : Hurtbox
{
    public float KnockTime;
    public Creep creepTarget;
    public override bool attackCreep(Creep creepHit)
    {
        if(creepHit != creepTarget)
        {
            if (magicDamage)
            {
                creepHit.takeMagicDamage(damage);
            }
            else
            {
                creepHit.takeDamage(damage);
            }
            creepHit.knockUp(KnockTime);
        }
        return false;
    }
}
