using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {
    public int damage;
    public bool diesOnHit = true;
    public bool magicDamage = false;
    public bool hasHitEffect = false;
    public GameObject hitEffect;
    public PlayerBase player;
    public void OnTriggerEnter(Collider collider)
    {
        Creep creep = collider.GetComponent<Creep>();
        if (creep != null)
        {
            if (hasHitEffect)
            {
                Instantiate(hitEffect, creep.transform.position, hitEffect.transform.rotation);
            }
            attackCreep(creep);
        }
    }
    public virtual bool attackCreep(Creep creepHit)
    {
        if (player != null && player.itemsHad[1] && player.canBubble)
        {
            player.canBubble = false;
            GameObject Bubbles = GameObject.Instantiate((GameObject)Resources.Load("CeasarBubble"));
            Bubbles.transform.position = creepHit.transform.position;            
        }
        if (player != null && player.itemsHad[9])
        {
            player.takeDamage((int)(damage * -0.1f));
        }
        if (magicDamage)
        {
            bool creepDied = creepHit.takeMagicDamage(damage);
            if (creepDied)
            {
                if (diesOnHit)
                {
                    Destroy(this.gameObject);
                }
                return true;
            }
            else
            {
                if (diesOnHit)
                {
                    Destroy(this.gameObject);
                }
                return false;
            }
        }
        else
        {
            bool creepDied = creepHit.takeDamage(damage);
            if (creepDied)
            {
                if (diesOnHit)
                {
                    Destroy(this.gameObject);
                }
                return true;
            }
            else
            {
                if (diesOnHit)
                {
                    Destroy(this.gameObject);
                }
                return false;
            }
        }

    }
}
