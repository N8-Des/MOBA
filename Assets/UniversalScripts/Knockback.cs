using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : Hurtbox {
    public float speed = 3;
    public GameObject SpotToHit;
    public float length = 2;
    public Vector3 location;
    public bool InAnimation = false;
    public override bool attackCreep(Creep creepHit)
    {
        if (InAnimation)
        {
            location = SpotToHit.transform.position;
        }
        if (magicDamage)
        {
            creepHit.takeMagicDamage(damage);
        }
        else
        {
            creepHit.takeDamage(damage);
        }
        creepHit.takeKnockback(location, speed, length);
        return false;
    }
}
