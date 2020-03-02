using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilAI : Creep
{
    Vector3 playerOffset = new Vector3(0, 1, 0);
    Vector3 dashPos;
    public bool isDashing;
    bool canDash;
    public GameObject dashHB;
    public override void AI()
    {
        if (playerInRadius && canAttack && !isDashing && canMove)
        {
            isAttacking = true;
            dashPos = player.transform.position + playerOffset;
            isDashing = true;
            canDash = true;
            canAttack = false;
            dashHB.SetActive(true);
        }
        else if(!isDashing && canMove)
        {
            Vector3 NewPosition = player.transform.position;
            NewPosition += playerOffset;
            Vector3 lookPos = NewPosition - transform.position;
            lookPos.y = 0;
            transform.position = Vector3.MoveTowards(transform.position, NewPosition, Speed * Time.deltaTime);
            Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
            this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
        }
        else
        {
            if (isDashing && canDash)
            {
                if (Vector3.Distance(dashPos, transform.position) > 0.01f)
                {
                    Vector3 NewPosition = dashPos;
                    Vector3 lookPos = NewPosition - transform.position;
                    lookPos.y = 0;
                    transform.position = Vector3.MoveTowards(transform.position, NewPosition, (Speed * 6) * Time.deltaTime);
                    Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                } else
                {
                    dashHB.SetActive(false);

                    isDashing = false;
                    canDash = false;
                    canMove = false;
                    canAttack = true;
                    isAttacking = false;
                    Invoke("startDash", 3f);
                }
            }
        }
    }

    void startDash()
    {
        canDash = true;
        canMove = true;
    }
}
