using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamsonPlayer : PlayerBase
{
    public int autoOn = 1;
    public GameObject ELocation;
    Vector3 dashLoc = Vector3.zero;
    bool isDashing;
    float currentSpeed;
    int WNumber = 1;
    public Hurtbox EHitbox;
    public GameObject SharinganFX;
    public GameObject GfuelFX;
    int preQAD;
    float preRAS;
    float preRMS;
    public GameObject fingerSpawn;
    public int CritChance = 10;
    Vector3 pos;

    public override void activateW()
    {
        //nothing lol its a passive
    }
    public override void E()
    {
        EHitbox.damage = EDamage + (int)(AttackDamage * 0.5f) + (int)(AbilityPower);
        EHitbox.player = this;
        isDashing = true;
        StartCoroutine(Dash());
    }
    public IEnumerator Dash()
    {
        while (isDashing && !stopDash)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashLoc, Time.deltaTime * 25);
            yield return new WaitForEndOfFrame();
        }
        endDash();
    }
    public void endDash()
    {
        isDashing = false;
        NewPosition = transform.position;
    }
    public override void Q()
    {
        SharinganFX.SetActive(true);
        preQAD = AttackDamage;
        AttackDamage = (int)(AttackDamage * (1 + (QDamage * 0.01f)));
        Invoke("endSharingan", 6);
    }

    void endSharingan()
    {
        AttackDamage = preQAD;
        SharinganFX.SetActive(false);
    }
    void endR()
    {
        AttackSpeed = preRAS;
        speed = preRMS;
        GfuelFX.SetActive(false);
    }
    public override void R()
    {
        Debug.Log((1 + (RDamage * 0.01f)));
        GfuelFX.SetActive(true);
        preRAS = AttackSpeed;
        preRMS = speed;
        AttackSpeed *= (1 + (RDamage * 0.01f));
        speed *= (1 + (RDamage * 0.01f));
        Invoke("endR", 10);
    }
    public override void abilityDescription()
    {
        CritChance = 25 + (3 * level);
        Qdesc = "Samson activates his Sharingan, gaining " + QDamage + "% additional attack damage for the next 6 seconds.";
        Wdesc = "Samson's auto attacks passively climb the crescendo. Every eigth attack deals" + 
            "<color=orange> (+" + (int)(AttackDamage * 2) + ")</color> plus <color=red>" + WDamage + "%</color> of the enemy's max health as physical damage.";
        Edesc = "Samson dashes in a direction, then shoots beams out of both hands perpendicular to his dash. These beams deal " + EDamage + "<color=orange> (+" + (int)(AttackDamage * 0.5) +
            ")</color> <color=#32fb93>(+" + AbilityPower + ")</color> magic damage.";
        Rdesc = "Samson drinks some Blood Rush, gaining " + RDamage + "% bonus movement speed and attack speed";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Sharingan");
        IndW.updateAbilityName("(W) Piano Man");
        IndE.updateAbilityName("(E) Beamdash");
        IndR.updateAbilityName("(R) BLOOD RUSH!");
        string Pdesc = "Samson's auto attacks have a " + CritChance + "% chance to critically strike, dealing 50% increased damage. This cannot apply to his final shot of Piano Man.";
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityName("Natural Precision");
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityDescription(Pdesc);
        IndR.updateAbilityDescription(Rdesc);

    }
    public override void LevelUp()
    {
        base.LevelUp();
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
            if (WNumber >= 9)
            {
                Anim.SetTrigger("W");
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
    public override void activateE()
    {
        if (canAttack)
        {
            CooldownE = false;
            canMove = false;
            IndE.StartCooldown(CDE, this, 3);
            dashLoc = new Vector3(ELocation.transform.position.x, transform.position.y, ELocation.transform.position.z);
            forceStopMoving();
            graphics.transform.rotation = EIndicator.transform.rotation;
            Anim.SetTrigger("E");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            NewPosition = dashLoc;
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
            if (canAttackAfterAuto)
            {
                stopAbilityIndication();
                EndAttack();
            }
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
                if (creepSelected != touchedCreep)
                {
                    creepSelected = touchedCreep;
                    if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack && canAttackAfterAuto )
                    {
                        NewPosition = transform.position;
                        stopMoving();
                        transform.LookAt(touchedCreep.transform.position);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                        if (WNumber >= 9)
                        {
                            Anim.SetTrigger("W");
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
                else
                {
                    walkTowardsTarget(touchedCreep);
                }
            }
        }
        if (Vector3.Distance(NewPosition, transform.position) > walkRange && canAttackAfterAuto)
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
        else
        {
            stopMoving();
        }
    }
    void spawnProj(string name, int speed, int damage)
    {
        if (WNumber >= 9)
        {
            GameObject sound = GameObject.Instantiate((GameObject)Resources.Load("SamsonAutoFXW"));
            GameObject piano = GameObject.Instantiate((GameObject)Resources.Load("SamWAuto9"));
        }
        else
        {
            GameObject piano = GameObject.Instantiate((GameObject)Resources.Load("SamWAuto" + WNumber));
        }
        GameObject projectile = GameObject.Instantiate((GameObject)Resources.Load(name));
        AA_Projectile proj = projectile.GetComponent<AA_Projectile>();
        if (hexagonAttack)
        {
            proj.damage = damage + 100;
            hexagonAttack = false;
        } else
        {
            proj.damage = damage;
        }
        proj.speed = speed;
        proj.creepTarget = creepSelected.gameObject;
        proj.player = this;
        projectile.transform.position = fingerSpawn.transform.position;
        autoOn += 1;

    }
    public override void stopMoving()
    {
        base.stopMoving();
    }
    public override void hitCreepWithAuto()
    {
        if (!isMoving)
        {
            if (itemsHad[5])
            {
                if (numGorilla <= 10)
                {
                    StartCoroutine(gorilla());
                }
            }
            if (IndW.levelNum > 0)
            {
                int crit = Random.Range(0, 100);
                if (WNumber >= 9)
                {
                    spawnProj("SamsonWBullet", 25, (int)((AttackDamage * 2) + (creepSelected.maxHealth * (WDamage * 0.01f))));
                    GameObject sound = GameObject.Instantiate((GameObject)Resources.Load("SamsonAutoFXW"));
                    WNumber = 1;
                }
                else
                {
                    if (crit <= CritChance)
                    {
                        spawnProj("SamsonBullet", 15, (int)(AttackDamage * 1.5f));
                        GameObject sound = GameObject.Instantiate((GameObject)Resources.Load("SamsonAutoFX"));
                    } else
                    {
                        spawnProj("SamsonBullet", 15, AttackDamage);
                        GameObject sound = GameObject.Instantiate((GameObject)Resources.Load("SamsonAutoFX"));
                    }
                }
                WNumber += 1;
            }
            else
            {
                int crit = Random.Range(0, 100);
                if (crit <= CritChance)
                {
                    spawnProj("SamsonBullet", 15, (int)(AttackDamage * 1.5f));
                    GameObject sound = GameObject.Instantiate((GameObject)Resources.Load("SamsonAutoFX"));
                }
                else
                {
                    spawnProj("SamsonBullet", 15, AttackDamage);
                    GameObject sound = GameObject.Instantiate((GameObject)Resources.Load("SamsonAutoFX"));
                }
            }
        }
    }
}
