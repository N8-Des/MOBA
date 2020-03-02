using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA_Projectile : MonoBehaviour {

    public GameObject creepTarget;
    public float speed;
    public int damage;
    public bool hasHitEffect;
    public string hitEffectName;
    public PlayerBase player;
    public bool KentSlow;
    Vector3 offset = new Vector3(0, 2, 0);
    public void Update()
    {
        if (creepTarget != null)
        {
            Vector3 NewPosition = creepTarget.transform.position + offset;
            if (Vector3.Distance(NewPosition, transform.position) > 0.7f)
            {
                Vector3 lookPos = NewPosition - transform.position;
                lookPos.y = 0;
                transform.position = Vector3.MoveTowards(transform.position, NewPosition, speed * Time.deltaTime);
                Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
            }
            else
            {
                if (player.itemsHad[2])
                {
                    Vector3 kb = (creepTarget.transform.position - player.transform.position).normalized / 2;
                    creepTarget.GetComponent<Creep>().takeKnockback(creepTarget.transform.position + kb, 5, 0.3f);
                }
                if (player.itemsHad[3])
                {
                    player.numFelix += 1;
                    if (player.numFelix >= 3 && creepTarget != null)
                    {
                        GameObject thunder = GameObject.Instantiate((GameObject)Resources.Load("Thunder"));
                        thunder.transform.position = creepTarget.transform.position + new Vector3(0, 10, 0);
                        player.numFelix = 0;
                    }
                }
                if (player.itemsHad[7])
                {
                    creepTarget.GetComponent<Creep>().shredResist(10, 3);
                }
                if (player.itemsHad[8])
                {
                    int rando = Random.Range(0, 99);
                    if (rando >= 80)
                    {
                        player.canAttackAfterAuto = true;
                        player.hitCreepWithAuto();
                    }
                }
                if (player.itemsHad[10])
                {
                    player.takeDamage((int)(player.AttackDamage * -0.2), false, false);
                }
                if (KentSlow)
                {
                    creepTarget.GetComponent<Creep>().slow(60, 4);
                }
                creepTarget.GetComponent<Creep>().takeDamage(damage);
                if (hasHitEffect)
                {
                    GameObject effect = GameObject.Instantiate((GameObject)Resources.Load(hitEffectName));
                    effect.transform.position = creepTarget.transform.position;
                }
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
