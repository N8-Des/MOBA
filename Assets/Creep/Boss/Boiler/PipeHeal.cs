using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeHeal : Creep
{
    public BoilerBoss bb;
    public bool numPipe;
    public override void AI()
    {
    }
    public override void Update()
    {
    }
    public override bool takeDamage(int damage)
    {
        int newDamage = (int)(damage * (100.0f / (100.0f + armor)));
        health -= newDamage;
        GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageText"));
        dmgNum.transform.SetParent(canvas1.transform);
        dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
        dmgNum.GetComponent<DamageNum>().damageText = newDamage.ToString();
        if (health <= 0)
        {
            bb.pipeAnim(numPipe);
            return true;
        }
        return false;
    }
    public override bool takeMagicDamage(int damage)
    {
        int newDamage = (int)(damage * (100.0f / (100.0f +  magicResist)));
        health -= newDamage;
        GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextMagic"));
        dmgNum.transform.SetParent(canvas1.transform);
        dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
        dmgNum.GetComponent<DamageNum>().damageText = newDamage.ToString();
        if (health <= 0)
        {
            bb.pipeAnim(numPipe);
            return true;
        }
        return false;
    }
}
