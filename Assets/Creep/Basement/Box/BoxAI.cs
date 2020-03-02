using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAI : Creep
{
    public GameObject projectile;
    public float projectileSpeed;
    public float attackDelaySpeed;
    public Vector3 projOffset;
    public override void AI()
    {
        if (monica == null)
        {
            monicaInAutoRadius = false;
            monicaInRadius = false;
        }
        if (canMove)
        {
            if (monicaInRadius)
            {
                if (!monicaInAutoRadius)
                {
                    if (attackDelay)
                    {
                        StartCoroutine(AttackDelay());
                    }
                    else
                    {
                        anim.SetBool("isWalking", true);
                        NewPosition = monica.gameObject.transform.position + offset;
                        isMoving = true;
                        Vector3 lookPos = NewPosition - transform.position;
                        lookPos.y = 0;
                        transform.position = Vector3.MoveTowards(transform.position, NewPosition, Speed * Time.deltaTime);
                        Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                    }
                }
                else
                {
                    //auto attack
                    if (canAttack && !isAttacking)
                    {
                        anim.SetBool("isWalking", false);
                        anim.ResetTrigger("Attack");
                        anim.SetTrigger("Attack");
                        anim.SetBool("isAttacking", true);

                        isAttacking = true;
                        canAttack = false;
                        attackDelay = true;
                        attackingMonica = true;
                    }
                    NewPosition = monica.gameObject.transform.position;
                    Vector3 lookPos = NewPosition - transform.position;
                    lookPos.y = 0;
                    Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                }
            }
            
            if (!playerInRadius && !monicaInRadius)
            {
                isAttacking = false;
                canAttack = true;
                NewPosition = player.transform.position;
                if (Vector3.Distance(NewPosition, transform.position) > NexusAttackRange)
                {
                    anim.SetBool("isWalking", true);
                    anim.SetBool("isAttacking", false);

                    isMoving = true;
                    Vector3 lookPos = NewPosition - transform.position;
                    lookPos.y = 0;
                    transform.position = Vector3.MoveTowards(transform.position, NewPosition, Speed * Time.deltaTime);
                    Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);

                }
                else
                {
                    NewPosition = transform.position;
                }
            }
            else if (!playerInAutoRadius && !monicaInRadius)
            {
                //canAttack = true;
                //isAttacking = false;
                //anim.SetTrigger("StopAttack");

                    anim.SetBool("isWalking", true);
                    anim.SetBool("isAttacking", false);

                    NewPosition = player.gameObject.transform.position + offset;
                    isMoving = true;
                    Vector3 lookPos = NewPosition - transform.position;
                    lookPos.y = 0;
                    transform.position = Vector3.MoveTowards(transform.position, NewPosition, Speed * Time.deltaTime);
                    Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
            }
            else if (!monicaInRadius)
            {
                //auto attack
                if (canAttack && !attackDelay)
                {
                    anim.SetBool("isWalking", false);
                    anim.SetTrigger("Attack");
                    anim.SetBool("isAttacking", true);
                    anim.SetBool("isIdle", false);
                    isAttacking = true;
                    canAttack = false;
                    attackDelay = true;
                    attackingMonica = false;
                    StartCoroutine(AttackDelay());

                }
                
                NewPosition = player.gameObject.transform.position;
                Vector3 lookPos = NewPosition - transform.position;
                lookPos.y = 0;
                Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                
            }
        }
    }
    public void shootPlayerPos()
    {
        GameObject spawnedProj = Instantiate(projectile);
        spawnedProj.transform.position = this.transform.position + projOffset;
        EnemyProjectile enemyProjectile = spawnedProj.GetComponent<EnemyProjectile>();
        enemyProjectile.speed = projectileSpeed;
        enemyProjectile.damage = damage;
        enemyProjectile.targetPos = player.transform.position + new Vector3(0, 2, 0);
    }
    public void HitPlayer()
    {
        player.takeDamage(damage, false, false);
    }
    public override IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelaySpeed);
        attackDelay = false;
        anim.SetBool("isAttacking", false);
        anim.SetBool("isWalking", true);
        isAttacking = false;
        canAttack = true;
    }
    public void EndAttack()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("isIdle", true);
        //playerInAutoRadius = false;
    }
}

