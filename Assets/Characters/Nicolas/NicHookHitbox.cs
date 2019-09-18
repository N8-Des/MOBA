using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicHookHitbox : Hurtbox {
    public NicPlayer playerNic;
    public override bool attackCreep(Creep creepHit)
    {
        if (playerNic != null && playerNic.itemsHad[1] && player.canBubble)
        {
            player.canBubble = false;
            GameObject Bubbles = GameObject.Instantiate((GameObject)Resources.Load("CeasarBubble"));
            Bubbles.transform.position = creepHit.transform.position;
        }
        creepHit.takeMagicDamage(damage);
        creepHit.takeQ(playerNic.hookObject);
        creepHit.takeStun(6);
        playerNic.EndAttack();
        playerNic.hitQ(creepHit);
        return false;

    }
}
