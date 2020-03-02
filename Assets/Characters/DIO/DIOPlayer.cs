using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIOPlayer : PlayerBase
{
    public AutoRangeChecker rad2;
    AutoRangeChecker backUp;
    public GameObject backUpW;
    private bool WEmp = false;
    private int damageE;

    public GameObject BucketParticle;
    //public GameObject autoPart;
    public Hurtbox QHB;
    int numattacks = 0;
    public Hurtbox EHitbox;
    int autoOn = 1;
    public Hurtbox WHB;
    public GameObject WorldMuda;
    GameObject CurrentWorld;
    bool rangedProj;
    public GameObject projSpawn;
    public List<GameObject> KentUI = new List<GameObject>();
    public Hurtbox EHurt2;
    bool isSwimming;
    float speedHold;
    float extraSpeedR = 2.5f;
    public GameObject Eind;
    Animator AnimKnife;
    public GameObject knifeProj;
    public bool TheWorld;
    [SerializeField]
    GameObject knifeSpawn;
    public override void W()
    {
        transform.rotation = WindRot;
        WHB.damage = WDamage + (int)(AbilityPower * 0.9);
    }
    public void theWorld()
    {
        gameManager.TheWorld(this);
        TheWorld = true;
        Invoke("endWorld", RDamageScale[IndR.levelNum]);
    }
    void endWorld()
    {
        TheWorld = false;
        gameManager.endWorld();
        Anim.SetTrigger("EndWorld");
        if (AnimKnife != null)
        {
            AnimKnife.enabled = true;
        }
    }
    public override void activateE()
    {
        if (canAttack)
        {
            forceStopMoving();
            Quaternion worldRot = EIndicator.transform.rotation;
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
            CurrentWorld = Instantiate(WorldMuda);
            CurrentWorld.transform.position = this.transform.position;
            CurrentWorld.transform.rotation = EIndicator.transform.rotation;
            CurrentWorld.GetComponent<WorldFollow>().DIO = gameObject;
            CurrentWorld.GetComponent<UltDoT>().Damage = EDamage + (int)(AttackDamage * 0.1f);
            Anim.SetBool("ERun", true);
            Anim.SetBool("isIdle", true);
            Invoke("EndE", 8);
            canMove = true;
            Anim.SetBool("isAttacking", true);
            canAttack = false;
            speed = 2f;
        }
    }
    public override void activateW()
    {
        if (canAttack)
        {
            CooldownW = false;
            canMove = false;
            IndW.StartCooldown(CDW, this, 2);
            WindRot = WIndicator.transform.rotation;
            forceStopMoving();
            transform.rotation = WindRot;
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

    void EndE()
    {
        Anim.SetBool("ERun", false);
        Anim.SetBool("isAttacking", false);
        Destroy(CurrentWorld);
        speed = 3f;
        CooldownE = false;
        IndE.StartCooldown(CDE, this, 3);
        canAttack = true;
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
                if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack && canAttackAfterAuto)
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
            if (isSwimming)
            {
                Anim.SetBool("isSwimming", true);
                Anim.SetBool("isWalking", true);
            }
            else
            {
                Anim.SetBool("isWalking", true);
            }
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
    public override void abilityDescription()
    {
        Qdesc = "DIO throws 5 knives in a cone, dealing " + QDamage + "<color=orange> (+" + (int)(AttackDamage * 0.35) + ")</color> physcial damage per knife. \n\nIf time is stopped, the knives will remain stationary.";
        Wdesc = "DIO fires two laser beams from his eyes in a small line, dealing " + WDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.9f) + ")</color> magic damage to all hit enemies.";
        Edesc = "DIO summons his stand to punch rapidly in front of him, dealing " + EDamage + "<color=orange> (+" + (int)(AttackDamage * 0.1) + ")</color> physical damage per half-second for 8 seconds. \nYou can move at two-thirds speed while this ability is being used.";
        Rdesc = "DIO stops time for " + RDamage + " seconds.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Knife Throw");
        IndW.updateAbilityName("(W) Eye Lasers");
        IndE.updateAbilityName("(E) Muda Rush");
        IndR.updateAbilityName("(R) The World");
        string Pdesc = "DIO Regenerates 2 health every 3 seconds.";
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityName("Vampiric Regeneration");
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityDescription(Pdesc);
    }
    public override void Q()
    {
        GameObject Knives = Instantiate(knifeProj);
        AnimKnife = Knives.GetComponent<Animator>();
        if (TheWorld)
        {
            AnimKnife.enabled = false;
        }
        Knives.transform.position = knifeSpawn.transform.position;
        Knives.transform.rotation = QuindRot;
        Knives.GetComponent<KnifeDam>().giveDamage(QDamage + (int)(AttackDamage * 0.35));
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
    public override void hitCreepWithAuto()
    {
        if (!isMoving)
        {
            canAttackAfterAuto = false;
            autoOn += 1;
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
                    creepSelected.takeDamage(AttackDamage);
                }
            }
            if (itemsHad[10])
            {
                takeDamage((int)(AttackDamage * -0.2), false, false);
            }
            if (hexagonAttack)
            {
                creepSelected.takeDamage(AttackDamage + 100);
                hexagonAttack = false;
            }
            else
            {
                creepSelected.takeDamage(AttackDamage);
            }
            GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("MUDA"));
            effect.transform.position = creepSelected.transform.position;
        }
    } 
    public void endDamageAuto()
    {
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
}
