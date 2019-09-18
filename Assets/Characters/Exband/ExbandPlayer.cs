using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExbandPlayer : PlayerBase
{
    public Hurtbox QHitbox;
    public Hurtbox EHitbox;
    public Hurtbox RHitbox;
    public KnockUpHurtbox EHiboxEnd;
    public GameObject ELocation;
    Vector3 dashLoc = Vector3.zero;
    Vector3 PunchLoc = Vector3.zero;
    public Knockback kb;
    bool isDashing;
    bool isReducing;
    float currentSpeed;
    public KnockUpHurtbox kbh; 
    public GameObject WFX;
    public override void E()
    {
        EHitbox.damage = EDamage + (int)(AttackDamage * 0.6f);
        kbh.damage = (int)(EDamage * 1.5f) + (int)(AttackDamage * 0.9f);
        EHitbox.player = this;
        isDashing = true;
        StartCoroutine(Dash());
    }
    public IEnumerator Dash()
    {
        while (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashLoc, Time.deltaTime * 3);
            yield return new WaitForEndOfFrame();
        }
    }
    public void endDash()
    {
        isDashing = false;
        NewPosition = transform.position;
    }
    public override void Q()
    {
        QHitbox.damage = QDamage + (int)(AttackDamage * 0.8f);
        QHitbox.player = this;
    }
    public override void R()
    {
        RHitbox.damage = RDamage + (int)(AttackDamage * 1.2f);
        RHitbox.player = this;
    }
    public override void activateQ()
    {
        if (canAttack)
        {
            CooldownQ = false;
            canMove = false;
            IndQ.StartCooldown(CDQ, this, 1);
            forceStopMoving();
            PunchLoc = kb.SpotToHit.transform.position;
            kb.location = PunchLoc;
            graphics.transform.rotation = QIndicator.transform.rotation;
            Anim.SetTrigger("Q");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
        }
    }
    public override void activateW()
    {
        CooldownW = false;
        IndW.StartCooldown(CDW, this, 2);
        WindRot = WIndicator.transform.rotation;
        W();
    }
    public override void activateE()
    {
        if (canAttack)
        {
            CooldownE = false;
            canMove = false;
            IndE.StartCooldown(CDE, this, 3);
            dashLoc = ELocation.transform.position;
            forceStopMoving();
            graphics.transform.rotation = EIndicator.transform.rotation;
            Anim.SetTrigger("E");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            NewPosition = dashLoc;
        }
    }
    public override void activateR()
    {
        if (canAttack)
        {
            CooldownR = false;
            canMove = false;
            RindRot = RIndicator.transform.rotation;
            IndR.StartCooldown(CDR, this, 4);
            forceStopMoving();
            Anim.SetTrigger("R");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
        }
    }
    public override void abilityDescription()
    {
        Qdesc = "Exband punches forward, expanding their arm. Enemies hit are pushed back and are dealt " + QDamage + "<color=orange> (+" + (int)(AttackDamage * 0.8) + ")</color> physical damage.";
        Wdesc = "Exband greatly increases the density in their body, reducing all damage taken by " + WDamage + "<color=red> (+" + (int)(maxHealth * 0.001) + ")</color>% for 5 seconds. "
            + "While this effect is active, Exband's movespeed is reduced by 150.";
        Edesc = "After a small delay, Exband dashes forwards, dealing " + EDamage + " <color=orange>(+" + (int)(AttackDamage * 0.6) + ")</color> physical damage, and stunning for 1.5 seconds. "
            + "Afterwards, Exband slams the ground, knocking all enemies up for 1 second and dealing " + (int)(EDamage * 1.5) + " <color=orange>(+" + (int)(AttackDamage * 0.9f) +
            ")</color> magic damage.";
        Rdesc = "Exband stretches their arm out and spins, hitting all enemies in a large circle, and knocking them back. This attack deals " + RDamage + " <color=orange>(+"
            + (int)(AttackDamage * 1.2) + ")</color> physical damage.";

        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Stretchy Punch");
        IndW.updateAbilityName("(W) Densify");
        IndE.updateAbilityName("(E) Heroic Dash");
        IndR.updateAbilityName("(R) Arm of Protection");
    }
    public override void W()
    {
        isReducing = true;
        currentSpeed = speed;
        WFX.SetActive(true);
        speed -= 1.5f;
        StartCoroutine(EndDamageReduction());
    }
    IEnumerator EndDamageReduction()
    {
        yield return new WaitForSeconds(5);
        isReducing = false;
        speed = currentSpeed;
        WFX.SetActive(false);
        Anim.SetBool("isWalkingBlock", false);
    }
    public override void ClickUpdate()
    {
        bool RMB = Input.GetMouseButton(1);
        if (RMB && canMove)
        {
            stopAbilityIndication();
            EndAttack();
            autoNum = 1;
            RaycastHit hit;
            Ray raymond = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(raymond, out hit, Mathf.Infinity, groundOnly))
            {
                NewPosition = hit.point;
                NewPosition.y = 0.5f;
            }
            if (Physics.Raycast(raymond, out hit) && hit.transform.tag == "Creep")
            {
                Creep touchedCreep = hit.transform.gameObject.GetComponent<Creep>();
                creepSelected = touchedCreep;
                if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack)
                {
                    NewPosition = transform.position;
                    stopMoving();
                    transform.LookAt(touchedCreep.transform.position);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    Anim.SetTrigger("Attack");
                    Anim.SetBool("isAttacking", true);
                    Anim.SetBool("isIdle", false);

                }
                else
                {
                    walkTowardsTarget(touchedCreep);
                }
            }
        }
        if (Vector3.Distance(NewPosition, transform.position) > walkRange)
        {
            if (isReducing)
            {
                isMoving = true;
                Anim.SetBool("isIdle", false);
                Anim.SetBool("isWalking", false);
                Anim.SetBool("IsWalkingBlock", true);
                Vector3 lookPos = NewPosition - transform.position;
                lookPos.y = 0;
                transform.position = Vector3.MoveTowards(transform.position, NewPosition, speed * Time.deltaTime);
                Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            }
            else
            {
                isMoving = true;
                Anim.SetBool("isIdle", false);
                Anim.SetBool("IsWalkingBlock", false);
                Anim.SetBool("isWalking", true);
                Vector3 lookPos = NewPosition - transform.position;
                lookPos.y = 0;
                transform.position = Vector3.MoveTowards(transform.position, NewPosition, speed * Time.deltaTime);
                Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            }

        }
        else
        {
            stopMoving();
        }
    }
    public override void stopMoving()
    {
        Anim.SetBool("IsWalkingBlock", false);
        base.stopMoving();
    }

}
