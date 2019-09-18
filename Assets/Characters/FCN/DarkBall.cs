using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBall : Hurtbox {
    public int timesHit = 0;
    public FCNPlayer fcn;
    public int maxTimesHit = 2;
    public override bool attackCreep(Creep creepHit)
    {
        if (fcn.ExtraDamageQ >= 500)
        {
            //don't do anything, yet.
        }else
        {
            fcn.ExtraDamageQ += 1;
        }
        //Debug.Log("Hit " + creepHit.name + " for " + damage + " damage");
        timesHit += 1;
        creepHit.takeDamage(damage);
        return false;
        if (timesHit >= maxTimesHit)
        {
            Destroy(gameObject);
        }
    }
}
