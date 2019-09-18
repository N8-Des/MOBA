using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonicaController : MonoBehaviour {

    public NicPlayer nic;
    public Animator Anim;
    public bool isAttacking;
    bool isWalkingToTarget;
    Transform nicTarget;
    List<Creep> creepsInRadius = new List<Creep>();
    List<Creep> creepsToAttack = new List<Creep>();
    bool nicWalking;
    Creep creepTarget;
    public bool creepInAutoRadius;
    Vector3 walkingPos;
    public GameObject autoPos;
    public int autoDamage;
    void Update()
    {
        AI();
    }
    public void addCreep(Creep newCreep)
    {
        if (!creepsInRadius.Contains(newCreep))
        {
            creepsInRadius.Add(newCreep);
        }
    }
    public void removeCreep(Creep newCreep)
    {
        if (creepsInRadius.Contains(newCreep))
        {
            creepsInRadius.Remove(newCreep);
        }
    }
    public void addCreepAtk(Creep newCreep)
    {
        if (!creepsToAttack.Contains(newCreep))
        {
            creepsToAttack.Add(newCreep);
        }
    }
    public void removeCreepAtk(Creep newCreep)
    {
        if (creepsToAttack.Contains(newCreep))
        {
            creepsToAttack.Remove(newCreep);
        }
    }
    void AI()
    {
        if ( creepsInRadius.Count > 0 && creepsInRadius[0] == null)
        {
            creepsInRadius.Remove(creepsInRadius[0]);
        }
        if (creepsToAttack.Count > 0 && creepsToAttack[0] == null)
        {
            creepsToAttack.Remove(creepsToAttack[0]);
        }
        if (nicWalking)
        {
            isAttacking = false;
            walkingPos = nicTarget.position;
            walkingPos.y = 0.09f;
            if (Mathf.Abs(Vector3.Distance(walkingPos, transform.position)) >= 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, walkingPos, Time.deltaTime * 3);
                Anim.SetBool("IsIdle", false);
                Anim.SetBool("IsAttacking", false);
                Anim.SetBool("IsWalking", true);
            }else
            {
                nicWalking = false;
                Anim.SetBool("IsWalking", false);
                Anim.SetBool("IsIdle", true);
            }
        }
        else if (creepsInRadius.Count > 0)
        {
            if (!isAttacking)
            {
                if (creepsToAttack.Count > 0)
                {
                    isWalkingToTarget = false;
                    attackCreep();
                } else
                {
                    isWalkingToTarget = true;
                    walkToTarget();
                }
            }
        } else
        {
            Anim.SetBool("IsIdle", true);
            Anim.SetBool("IsAttacking", false);
            Anim.SetBool("IsWalking", false);
        }
    }
    void walkToTarget()
    {
        transform.LookAt(creepsInRadius[0].transform.position);
        Anim.SetBool("IsWalking", true);
        Anim.SetBool("IsIdle", false);
        Anim.SetBool("IsAttacking", false);
        walkingPos = creepsInRadius[0].transform.position;
        walkingPos.y = 0.09f;
        transform.position = Vector3.MoveTowards(transform.position, walkingPos, Time.deltaTime * 3);
    }
    public void endAttack()
    {
        isAttacking = false;
        Anim.SetBool("IsIdle", true);
        Anim.SetBool("IsAttacking", false);
        Anim.SetBool("IsWalking", false);
    }
    public void hitCreepWithAuto()
    {
        //spawn auto;
        GameObject autoProj = GameObject.Instantiate((GameObject)Resources.Load("MonicaAuto"));
        autoProj.transform.position = autoPos.transform.position;
        MonicaAA auto = autoProj.GetComponent<MonicaAA>();
        auto.damage = autoDamage;
        auto.creepTarget = creepsToAttack[0];
    }
    void attackCreep()
    {
        transform.LookAt(creepsToAttack[0].transform);
        Anim.SetTrigger("Attack");
        Anim.SetBool("IsIdle", false);
        Anim.SetBool("IsAttacking", true);
        Anim.SetBool("IsWalking", false);
        isAttacking = true;
    }
}
