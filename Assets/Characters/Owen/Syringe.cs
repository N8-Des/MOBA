using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : Hurtbox {
    public OwenPlayer Owen;

    public override bool attackCreep(Creep creepHit)
    {
        bool heal = base.attackCreep(creepHit);
        Owen.Heal(heal);
        return false;
        Destroy(this.gameObject);
    }
}
