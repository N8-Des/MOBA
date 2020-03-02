using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexPlayer : PlayerBase
{
    public GameObject WLoc;
    public float WOffset;
    public GameObject EPos;
    public Hurtbox QHB;
    int numattacks = 0;
    public GameObject EHitbox;
    float damageReduction;
    float div;
    int autoOn = 1;
    public GameObject AlexLigger;
    Vector3 liggerPos;
    AlexDrink drink;
    public Transform liggerSpawn;
    public GameObject WPos;
    public Creep creepE;
    bool running;
    Vector3 targetPosE;
    public GameObject kickPosition;
    Vector3 kickLoc;
    public Hurtbox RHB;
    bool hasSpeed;
    float originalSpeed;
    public GameObject speedTrail;
    public override void Q()
    {
        QHB.damage = QDamage + (int)(AttackDamage * 0.45);
    }
    public override void EndAttack()
    {
        Anim.SetBool("isQ", false);
        base.EndAttack();
    }
    public override void W()
    {
        GameObject ligger = GameObject.Instantiate(AlexLigger);
        ligger.transform.position = liggerSpawn.position;
        drink = ligger.GetComponent<AlexDrink>();
        drink.targetPos = liggerPos;
        drink.damage = WDamage + (int)(AbilityPower * 0.8f);
    }
    public override void R()
    {
        RHB.damage = RDamage + (int)(AttackDamage * 0.6);
    }
    public override void E()
    {
        creepE.takeKnockback(kickLoc, 10, 1);
        creepE.takeDamage(EDamage + (int)(AttackDamage * 1.1));
        GameObject KickFX = GameObject.Instantiate((GameObject)Resources.Load("AlexEFX"));
        KickFX.transform.position = creepE.transform.position;
        GameObject hitbox = GameObject.Instantiate((GameObject)Resources.Load("KickHitbox"));
        FriendlyHurtbox friend = hitbox.GetComponent<FriendlyHurtbox>();
        friend.freindCreep = creepE;
        friend.damage = (EDamage + (int)(AttackDamage * 1.1));
        hitbox.transform.position = creepE.transform.position;
        hitbox.transform.parent = creepE.transform;
    }
    public override void activateE()
    {
        if (canAttack)
        {
            kickLoc = kickPosition.transform.position;

            CooldownE = false;
            canMove = false;
            IndE.StartCooldown(CDE, this, 3);
            forceStopMoving();
            transform.LookAt (EPos.transform.position - transform.position);
            Anim.SetBool("ERun", true);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            running = true;
            targetPosE = creepE.transform.position;
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
        }
    }
    void FixedUpdate()
    {
        if (running)
        {
            targetPosE = creepE.transform.position;
            targetPosE.y = 0;
            NewPosition = targetPosE;
            if (Vector3.Distance(targetPosE, transform.position) > 2.3f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosE, 19 * Time.deltaTime);
            } else
            {
                Anim.SetBool("ERun", false);
                Anim.SetTrigger("E");
                NewPosition = transform.position;
                running = false;
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
                if (!canAttackAfterAuto) 
                {
                    bufferedPosition = hit.point;
                    StartCoroutine(waitToMove());
                    isBuffering = true;
                }
                else if(!cant)
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
                    if (autoOn <= 3)
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
            transform.position = Vector3.MoveTowards(transform.position, NewPosition, speed* Time.deltaTime);
            Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
            graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
        }
        else
        {
            stopMoving();
        }


    }
    public override void abilityDescription()
    {
        Qdesc = "Alex spins blades around him, dealing " + QDamage + "<color=orange> (+" + (int)(AttackDamage * 0.45) + ")</color> physical damage to all nearby enemies";
        Wdesc = "Alex throws ligger on the ground, dealing " + WDamage + "<color=#32fb93> (+" + (int)(AbilityPower * 0.8) + ")</color> magic damage to enemies hit, and leaving behind a puddle "
            + "that slows by 70% for 3 seconds.";
        Edesc = "Alex runs up and kicks a target enemy, knocking them back and dealing " + EDamage + "<color=orange> (+" + (int)(AttackDamage * 1.1) + ")</color> damage to the enemy.";
        Rdesc = "Alex swings his sword in a cone, dealing " + RDamage + "<color=orange> (+" + (int)(AttackDamage * 0.6) + ")</color> physical damage. \n\nIf the enemies hit are currently slowed, they are stunned for 2 seconds as well.";      
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Dance of the Blades");
        IndW.updateAbilityName("(W) Pass the Ligger");
        IndE.updateAbilityName("(E) Cross-Country Kick");
        IndR.updateAbilityName("(R) Gay Strike");
        string Pdesc = "After every autoattack, Alex gains 60% bonus movespeed for 4 seconds";
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityName("Fresh Pair of Yeezy's");
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
            if (autoOn <= 3)
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

    public override void activateW()
    {
        if (canAttack)
        {
            CooldownW = false;
            canMove = false;
            IndW.StartCooldown(CDW, this, 2);
            liggerPos = WPos.transform.position;
            graphics.transform.LookAt(liggerPos);
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
    public override void ETest()
    {
        if (CooldownE && IndE.levelNum > 0)
        {
            stoppingAttack = Input.GetKeyUp(KeyCode.E);
            Ab3 = Input.GetKey(KeyCode.E);
            bool leftClick = Input.GetMouseButton(0);
            bool touch = Input.GetKeyDown(KeyCode.E);
            if (Ab3 && !stoppingAttack && anythingWorks)
            {
                EIndicator.SetActive(true);
                EPressed = true;
            }
            else if ((EPressed || (leftClick && Ab3)) && anythingWorks)
            {
                EIndicator.SetActive(false);
                EPressed = false;
                EIndicator.GetComponent<TargetCreepExtra>().knockVis.SetActive(false);
                isInvisible = false;
                activateE();
            }
            else
            {
                EIndicator.SetActive(false);
                EIndicator.GetComponent<TargetCreepExtra>().knockVis.SetActive(false);
                EPressed = false;
            }

            if (Ab3 && endingAttack)
            {
                anythingWorks = false;
                EIndicator.GetComponent<TargetCreepExtra>().knockVis.SetActive(false);

            }
            if (touch)
            {
                anythingWorks = true;
                endingAttack = false;
            }
        }
    }
    public override void QTest()
    {
        if (CooldownQ && IndQ.levelNum > 0)
        {
            stoppingAttack = Input.GetKeyUp(KeyCode.Q);
            Ab1 = Input.GetKey(KeyCode.Q);
            if (Ab1)
            {
                QPressed = true;
                activateQ();
                Anim.SetBool("isQ", true);
            }
        }
    }
    void Passive(bool speedy)
    {
        if (!hasSpeed)
        {
            Invoke("runSlow", 4);
            speed *= 1.6f;
            hasSpeed = true;
        } else
        {
            CancelInvoke("runSlow");
            Invoke("runSlow", 4);
            hasSpeed = true;
        }     
        speedTrail.SetActive(true);
    }
    void runSlow()
    {
        if (hasSpeed)
        {
            speed /= 1.6f;
            speedTrail.SetActive(false);
            hasSpeed = false;
        }
    }
    public override void hitCreepWithAuto()
    {
        bool didKill = false;
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
                int rando = Random.Range(0, 99);
                if (rando >= 80)
                {
                    didKill = creepSelected.takeDamage(AttackDamage);
                }
            }
            if (itemsHad[10])
            {
                takeDamage((int)(AttackDamage * -0.2), false, false);
            }
            if (hexagonAttack)
            {
                creepSelected.takeDamage(AttackDamage + 100);
                GameObject effect2 = GameObject.Instantiate((GameObject)Resources.Load("AlexAutoFlash"));
                effect2.transform.position = creepSelected.transform.position;
            }
            else
            {
                didKill = creepSelected.takeDamage(AttackDamage);
            }
            GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("AlexAutoFlash"));
            effect.transform.position = creepSelected.transform.position;
        }
        originalSpeed = speed;
        Passive(didKill);
    }
    public void endDamageAuto()
    {
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
 
}
