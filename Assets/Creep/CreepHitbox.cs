using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepHitbox : MonoBehaviour
{
    public int damage;
    public bool isMagic;
    public bool dieOnHit;
    void OnTriggerEnter(Collider collider)
    {
        PlayerBase player = collider.GetComponent<PlayerBase>();
        if (player != null)
        {
            player.takeDamage(damage, isMagic, false);
            if (dieOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
