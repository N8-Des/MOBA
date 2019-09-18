using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwenPlayer : PlayerBase {
    public GameObject SyringeSpawn;
    public float syrSpeed;
    bool isTony = false;
    public GameObject ELocation;
    float originalSpeed;
    public GameObject speedTrail;
    public GameObject tonyHood;
    public Hurtbox UltHurtbox;
    public override void W()
    {
        isTony = false;
        tonyHood.SetActive(false);
        GameObject syringe = GameObject.Instantiate((GameObject)Resources.Load("Syringe"));
        syringe.transform.rotation = WindRot;
        syringe.transform.position = SyringeSpawn.transform.position;
        Syringe syr = syringe.GetComponent<Syringe>();
        Rigidbody rb = syringe.GetComponent<Rigidbody>();
        syr.Owen = this;
        syr.damage += (int)(AttackDamage * 0.7f);
        rb.velocity = syringe.transform.forward * syrSpeed;
    }
    public void EndAutoAttackTony()
    {
        endTony();
        if (creepSelected != null && achecker.EnemiesInRadius.Contains(creepSelected))
        {
            isAttacking = false;
            AttackCreep(creepSelected.transform);
        }
        else
        {
            autoNum = 1;
            canMove = true;
            canAttack = true;
            isAttacking = false;
            Anim.SetBool("isIdle", true);
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
        }
    }
    public override void Q()
    {
        isTony = true;
        isInvisible = true;
    }
    public void Heal(bool minionDied)
    {
        if (minionDied)
        {
            health += (int)(AbilityPower * 0.4f) + 130;
        }
        else
        {
            health += (int)(AbilityPower * 0.15f) + 60;
        }
    }
    public override void R()
    {
        isTony = false;
        tonyHood.SetActive(false);
        UltHurtbox.damage = (int)(RDamage + (AbilityPower * 0.9f));
        UltHurtbox.damage += (int)(AttackDamage * 0.6f);
        int healthTaken = (int)(health * 0.2);
        health -= healthTaken;
    }
    public override void abilityDescription()
    {
        Qdesc = "Owen puts on his hood, becoming Tony. Tony is invisible, but invisibility ends upon using an ability or attacking. While Owen is invisible, " +
            "his next autoattack deals " + QDamage + "<color=orange> (+" + (int)(AttackDamage * 0.7f) + ")</color> physical damage.";
        Wdesc = "Owen throws a syringe of Insulin, hitting up to one target in its path, dealing " + WDamage + "<color=orange> (+" + (int)(AttackDamage * 0.7) + ")</color> physical damage." +
            " upon hitting a target, Owen heals for 60 <color=#32fb93>(+" + (int)(AbilityPower * 0.15f) + ")</color> health. If the syringe kills the target, it instead heals for" +
            " 130 <color=#32fb93>(+" + (int)(AbilityPower * 0.4f) + ")</color> health.";
        Edesc = "Owen places a flat escalator, pushing all enemies in a direction. Additionally, this gives Owen a " + EDamage + "% increase in speed for 3 seconds";
        Rdesc = "Owen opens the hole in his stomach, releasing all of his inner juices, including an excess amount of Mountain Dew, in a cone. This acidic mixture" +
            " deals " + RDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.9f) + ")</color> <color=orange>(+" + (int)(AttackDamage * 0.6f) + ")</color>" +
            " magic damage. This ability costs 20% of Owen's current health, and leaves him vulnerable.";

        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
    }

    public override void AttackCreep(Transform target)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            NewPosition = transform.position;
            stopMoving();
            Quaternion transRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transRot *= new Quaternion(0, 0, 0, 0);
            graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            if (isTony)
            {
                Anim.SetTrigger("TonyAuto");
                autoNum = 1;
            }
            else if (autoNum == 1)
            {
                Anim.SetTrigger("Attack");
                autoNum = 2;
            }
            else if (autoNum == 2)
            {
                Anim.SetTrigger("Attack2");
                autoNum = 1;
            }
        }
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
            if (Physics.Raycast(raymond, out hit, layerMask))
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
                    if (isTony)
                    {
                        NewPosition = transform.position;
                        stopMoving();
                        transform.LookAt(touchedCreep.transform.position);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                        Anim.SetTrigger("TonyAuto");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                    }
                    else
                    {
                        NewPosition = transform.position;
                        stopMoving();
                        transform.LookAt(touchedCreep.transform.position);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                        Anim.SetTrigger("Attack");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                    }

                }
                else
                {
                    walkTowardsTarget(touchedCreep);
                }
            }
        }
        if (Vector3.Distance(NewPosition, transform.position) > walkRange)
        {
            if (isTony)
            {
                isMoving = true;
                Anim.SetBool("isIdle", false);
                Anim.SetBool("isWalkingTony", true);
                Vector3 lookPos = NewPosition - transform.position;
                lookPos.y = 0;
                transform.position = Vector3.MoveTowards(transform.position, NewPosition, speed * Time.deltaTime);
                Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            }else
            {
                isMoving = true;
                Anim.SetBool("isIdle", false);
                Anim.SetBool("isWalkingTony", false);
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

    public override void activateE()
    {
        if (canAttack)
        {
            CooldownE = false;
            canMove = false;
            IndE.StartCooldown(CDE, this, 3);
            forceStopMoving();
            Anim.SetTrigger("E");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
        }
    }
    public override void E()
    {
        isTony = false;
        tonyHood.SetActive(false);
        GameObject Escelator = GameObject.Instantiate((GameObject)Resources.Load("Escalator"));
        Escelator.transform.rotation = ELocation.transform.rotation;
        Escelator.transform.position = ELocation.transform.position + new Vector3(0, -0.7f, 0);
        Escelator.transform.rotation *= Quaternion.Euler(0, 180, 0);
        StartCoroutine(speedDec());
    }
    public IEnumerator speedDec()
    {
        originalSpeed = speed;
        speedTrail.SetActive(true);
        speed *= (EDamage * 0.01f) + 1;
        yield return new WaitForSeconds(3);
        speed = originalSpeed;
        speedTrail.SetActive(false);

    }
    public override void activateW()
    {
        if (canAttack)
        {
            CooldownW = false;
            canMove = false;
            IndW.StartCooldown(CDW, this, 2);
            WindRot = WIndicator.transform.rotation;
            graphics.transform.rotation = WIndicator.transform.rotation;
            forceStopMoving();
            Anim.SetTrigger("W");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;

        }
    }
    public override void activateQ()
    {
        if (canAttack && !isTony)
        {
            canMove = false;
            forceStopMoving();
            QuindRot = QIndicator.transform.rotation;
            graphics.transform.rotation = QIndicator.transform.rotation;
            Anim.SetTrigger("Q");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
        }
    }
    public override void stopMoving()
    {
        Anim.SetBool("isWalkingTony", false);
        base.stopMoving();
    }
    public void StabTony()
    {
        creepSelected.takeDamage((int)(AttackDamage * 0.7f) + QDamage);
    }
    void endTony()
    {
        CooldownQ = false;
        IndQ.StartCooldown(CDQ, this, 1);
        tonyHood.SetActive(false);
        isTony = false;
        isInvisible = false;
    }
    void startTony()
    {
        isInvisible = true;
        isTony = true;
        tonyHood.SetActive(true);
    }
}
