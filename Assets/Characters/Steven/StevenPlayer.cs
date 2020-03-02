using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StevenPlayer : PlayerBase
{
    public GameObject WLoc;
    public float WOffset;
    public GameObject EPos;
    public Hurtbox QHB;
    public UltDoT RHB;
    int numattacks = 0;
    float damageReduction;
    float div;
    int autoOn = 1;
    public Hurtbox WHB;
    public Hurtbox EHB;
    bool QBuff;
    bool isJumping;
    int extraQDamage;
    Vector3 jumpos;
    bool ascended;
    int extraAD;
    public List<GameObject> ultObjects = new List<GameObject>();
    public override void W()
    {
        isJumping = true;
        StartCoroutine(WJump(jumpos, 3, 0.25f));
        Anim.SetBool("WIdle", true);
        NewPosition = jumpos;
        WHB.damage = WDamage + (int)(AbilityPower * 0.7f);
    }
    public override void R()
    {
        foreach(GameObject obj in ultObjects)
        {
            obj.SetActive(true);
        }
        ascended = true;
        RHB.Damage = RDamage + (int)(AbilityPower * 0.2);
        Invoke("ultTimer", 15);
    }
    public override void E()
    {
        EHB.damage = EDamage + (int)(AttackDamage * 0.8);
    }
    public override void activateQ()
    {
        CooldownQ = false;
        QBuff = true;
        IndQ.StartCooldown(CDQ, this, 1);
    }
    public override void activateW()
    {
        if (canAttack)
        {
            jumpos = WLoc.transform.position;
            jumpos.y -= 1;
            CooldownW = false;
            canMove = false;
            IndW.StartCooldown(CDW, this, 2);
            transform.LookAt(jumpos);
            forceStopMoving();
            Anim.SetTrigger("W");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
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
            RaycastHit yeet;
            bool cant = false;
            Ray raymond = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(raymond, out hit, Mathf.Infinity, groundOnly))
            {
                if (Physics.Linecast(transform.position + rayOffset, hit.point + rayOffset, out yeet, wallMask))
                {
                    if (yeet.collider.transform != null)
                    {
                        cant = true;
                    }
                }
                if (!canAttackAfterAuto && !cant)
                {
                    bufferedPosition = hit.point;
                    StartCoroutine(waitToMove());
                    isBuffering = true;
                }
                else if (!cant)
                {
                    canAttackAfterAuto = true;
                    NewPosition = hit.point;
                    EndAttack();
                }
            }
            cant = false;
            if (Physics.Raycast(raymond, out hit, Mathf.Infinity, creepsOnly) && hit.transform.tag == "Creep")
            {
                Creep touchedCreep = hit.transform.gameObject.GetComponent<Creep>();
                creepSelected = touchedCreep;
                if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack)
                {
                    NewPosition = transform.position;
                    stopMoving();
                    transform.LookAt(touchedCreep.transform.position);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    if (QBuff)
                    {
                        Anim.SetTrigger("Q");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                    }
                    else if (autoOn <= 3)
                    {
                        Anim.SetTrigger("Attack" + autoOn);
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                    }
                    else
                    {
                        autoOn = 1;
                        Anim.SetTrigger("Attack" + autoOn);
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
            isMoving = true;
            Anim.SetBool("isIdle", false);
            Anim.SetBool("isWalking", true);
            Vector3 lookPos = NewPosition - transform.position;
            lookPos.y = 0;
            transform.position = Vector3.MoveTowards(transform.position, NewPosition, speed * Time.deltaTime);
            Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
            graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
        }
        else
        {
            stopMoving();
        }
    }
    public IEnumerator WJump(Vector3 PlaceToGo, float speed, float duration)
    {
        float startTime = Time.time;
        Vector3 center = (transform.position + PlaceToGo) * 0.5f;
        center.y += -2f;
        Vector3 startArea = transform.position - center;
        Vector3 endArea = PlaceToGo - center;
        float suck = 0;
        while (suck < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.position = Vector3.Slerp(startArea, endArea, t);
            transform.position += center;
            suck += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Anim.SetBool("WIdle", false);
        isJumping = false;
        Anim.SetTrigger("WSlam");
    }
    void ultTimer()
    {
        foreach (GameObject obj in ultObjects)
        {
            obj.SetActive(true);
            ascended = false;
        }
    }
    public override void abilityDescription()
    {
        Qdesc = "Steven empowers his next basic attack, dealing an additional " + QDamage + "<color=orange> (+" + (int)(AttackDamage * 0.2) + ")</color> (+" + extraQDamage + ") damage. Killing a unit with this ability permanently increases its damage by 4.";
        Wdesc = "Steven jumps to a target location, dealing " + WDamage + "<color=#32fb93> (+" + (int)(AbilityPower * 0.7f) + ")</color> magic damage to enemies hit when he lands.";
        Edesc = "Steven uppercuts in front of him, knocking enemies hit upwards for 1.3 seconds and dealing " + EDamage + "<color=orange> (+" + (int)(AttackDamage * 0.8) + ")</color> physical damage as well.";
        Rdesc = "Steven channels the inner gorilla, ascending to new power. For 15 seconds, he deals " + RDamage + "<color=#32fb93> (+" + (int)(AbilityPower * 0.2f) + ")</color> magic damage to nearby enemies, and heals 100% of the damage he deals with auto attacks.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Gorilla Slamma");
        IndW.updateAbilityName("(W) Gorilla Jump");
        IndE.updateAbilityName("(E) Supreme Uppercut");
        IndR.updateAbilityName("(R) Ascended Gorilla");
        string Pdesc = "Steven's basic attacks deal extra damage based on his missing health, up to 120 extra damage at 1% health.";
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityName("Gorilla Rage");
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityDescription(Pdesc);
    }
    public override void AttackCreep(Transform target)
    {
        if (!isAttacking && creepSelected != null)
        {
            isAttacking = true;
            NewPosition = transform.position;
            stopMoving();
            Quaternion transRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transRot *= new Quaternion(0, 0, 0, 0);
            graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            if (QBuff)
            {
                Anim.SetTrigger("Q");
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
            }
            else if (autoOn <= 3)
            {
                Anim.SetTrigger("Attack" + autoOn);
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
            }
            else
            {
                autoOn = 1;
                Anim.SetTrigger("Attack" + autoOn);
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
            }
        }
    }
    public override void passiveUpdate()
    {
        float perc = (maxHealth - health);
        perc /= maxHealth;
        extraAD = (int)(120f * perc);
        string Pdesc = "Steven's basic attacks deal extra damage based on his missing health, up to 120 extra damage at 1% health. \n\n\n Current Bonus: " + extraAD;
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityDescription(Pdesc);
    }
    public override void hitCreepWithAuto()
    {
        if (!isMoving)
        {
            autoOn += 1;
            canAttackAfterAuto = false;
            if (itemsHad[2])
            {
                Vector3 kb = (creepSelected.transform.position - transform.position).normalized / 2;
                creepSelected.takeKnockback(creepSelected.transform.position + kb, 5, 0.3f);
            }
            if (itemsHad[3])
            {
                numFelix += 1;
                if (numFelix >= 3)
                {
                    GameObject thunder = GameObject.Instantiate((GameObject)Resources.Load("Thunder"));
                    thunder.transform.position = creepSelected.transform.position + new Vector3(0, 10, 0);
                    numFelix = 0;
                }
            }
            if (itemsHad[5])
            {
                if (numGorilla <= 10)
                {
                    StartCoroutine(gorilla());
                }
            }
            if (itemsHad[7])
            {
                creepSelected.shredResist(10, 3);
            }
            if (itemsHad[8])
            {
                int rando = UnityEngine.Random.Range(0, 99);
                if (rando >= 80)
                {
                    creepSelected.takeDamage(AttackDamage + extraAD);
                }
            }
            if (itemsHad[10])
            {
                takeDamage((int)(AttackDamage * -0.2), false, false);
            }
            if (hexagonAttack)
            {
                creepSelected.takeDamage(AttackDamage + 100);
                GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("StevenAutoFlash"));
                effect.transform.position = creepSelected.transform.position;
            }
            if (QBuff)
            {
                if(creepSelected.takeDamage( (int)(AttackDamage * 0.2) + extraQDamage + AttackDamage + QDamage + extraAD))
                {
                    extraQDamage += 4;
                }
                QBuff = false;
                GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("StevenQFlash"));
                effect.transform.position = creepSelected.transform.position;
                abilityDescription();
            }
            else
            {
                creepSelected.takeDamage(AttackDamage + extraAD);
                GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("StevenAutoFlash"));
                effect.transform.position = creepSelected.transform.position;
            }
            if (ascended)
            {
                takeDamage(-AttackDamage, true, false);
            }
        }

    }
    public void endDamageAuto()
    {
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
}
