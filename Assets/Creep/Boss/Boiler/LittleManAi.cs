using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleManAi : Creep
{
    public GameObject explosion;
    public override void AI()
    {
        if (!playerInAutoRadius)
        {
            isAttacking = false;
            canAttack = true;
            NewPosition = player.transform.position + offset;
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
        }else
        {
            anim.SetTrigger("Attack");
        }
    }
    public void spawnProj()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
    }
}
