using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KentEHitbox : Hurtbox
{
    public float speed = 3;
    public float length = 2;
    public Vector3 location;
    public bool InAnimation = false;
    public override bool attackCreep(Creep creepHit)
    {
        location = (creepHit.transform.position - player.gameObject.transform.position).normalized;
        creepHit.takeKnockback(creepHit.transform.position - location, speed, length);
        if (magicDamage)
        {
            creepHit.takeMagicDamage(damage);
        }
        else
        {
            creepHit.takeDamage(damage);
        }
        return false;
    }
}
