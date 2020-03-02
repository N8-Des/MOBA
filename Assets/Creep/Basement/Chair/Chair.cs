using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Creep
{
    bool aggro;
    bool canDash = true;
    bool isDashing;
    public Renderer rend;
    public Material chair;
    public Material invis;
    public override void AI()
    {
        if (canMove && canAttack)
        {
            if (playerInAutoRadius || gameManager.creeps.Count == 1 || health != maxHealth)
            {
                aggro = true;
            }
            if (aggro && !isDashing && canDash)
            {
                NewPosition = player.gameObject.transform.position + offset;
                Vector3 lookPos = NewPosition - transform.position;
                lookPos.y = 0;
                Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                StartCoroutine(Dash());
                rend.material = chair;
                isDashing = true;
            }
        }      
    }
    IEnumerator Dash()
    {
        while (Vector3.Distance(transform.position, NewPosition) >= 0.4f)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.MoveTowards(transform.position, NewPosition, 25 * Time.deltaTime);
        }
        canDash = false;
        isDashing = false;
        Invoke("dashCooldown", 5);
        rend.material = invis;
    }
    void dashCooldown()
    {
        canDash = true;
    }
}
