using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeDam : MonoBehaviour
{
    public List<Hurtbox> knives = new List<Hurtbox>();
    public void giveDamage(int damage)
    {
        foreach(Hurtbox hurt in knives)
        {
            hurt.damage = damage;
        }
    }
}
