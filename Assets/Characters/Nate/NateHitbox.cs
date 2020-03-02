using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NateHitbox : SlowHurtbox
{
    public NatePlayer p;
    public override bool attackCreep(Creep creepHit)
    {
        if(p.IndR.levelNum >= 1)
        {
            if (creepHit.isMarkedByNate)
            {
                Destroy(creepHit.gameObject.GetComponentInChildren<NateMark>().gameObject);
                GameObject ultMark2 = GameObject.Instantiate((GameObject)Resources.Load("NateRMark"));
                ultMark2.transform.position = creepHit.transform.position;
                ultMark2.transform.parent = creepHit.transform;
                ultMark2.GetComponent<NateMark>().player = player.GetComponent<NatePlayer>();
                ultMark2.GetComponent<NateMark>().creepOn = creepHit;
                p.marks.Add(ultMark2.GetComponent<NateMark>());
            }
            GameObject ultMark = GameObject.Instantiate((GameObject)Resources.Load("NateRMark"));
            ultMark.transform.position = creepHit.transform.position;
            ultMark.transform.parent = creepHit.transform;
            ultMark.GetComponent<NateMark>().player = player.GetComponent<NatePlayer>();
            ultMark.GetComponent<NateMark>().creepOn = creepHit;
            p.marks.Add(ultMark.GetComponent<NateMark>());
            creepHit.isMarkedByNate = true;

        }
        base.attackCreep(creepHit);
        return false;
    }

}
