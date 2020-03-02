using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatePlayer : PlayerBase
{
    public AutoRangeChecker rad2;
    AutoRangeChecker backUp;
    public GameObject backUpW;
    private int damageE;
    public GameObject BucketParticle;
    //public GameObject autoPart;
    public NateHitbox QHB;
    public Hurtbox EHitbox;
    int autoOn = 1;
    public Hurtbox WHB;
    public GameObject WorldMuda;
    GameObject CurrentWorld;
    bool rangedProj;
    public GameObject projSpawn;
    public List<GameObject> KentUI = new List<GameObject>();
    public Hurtbox EHurt2;
    Vector3 QPos;
    public GameObject QPosition;
    public int[] qSlowPercent;
    [SerializeField]
    GameObject iceSpawn;
    int QSlow;
    public GameObject iceExplosion;
    public GameObject aaProj;
    bool isW;
    public AudioManager audioManager;
    public Transform sanderPos;
    [SerializeField]
    float autoAudDelay;
    [SerializeField]
    GameObject WExplosion;
    [SerializeField]
    GameObject sander;
    [SerializeField]
    Transform EPosition;
    [SerializeField]
    GameObject RFX;
    GameObject currentSander;
    bool canRecast = false;
    Vector3 offset = new Vector3(0, 2.1f, 0);
    Vector3 EPos;
    public List<NateMark> marks = new List<NateMark>();
    public bool waitingGrab;
    public override void Q()
    {
        Instantiate(iceExplosion, QPos - offset, Quaternion.identity);
        QHB = iceExplosion.GetComponent<IceExplosion>().hb;
        QHB.slowPotency = 100 - QSlow;
        QHB.slowTime = 3;
        QHB.damage = QDamage + (int)(AbilityPower * 0.55);
        QHB.p = this;
        QHB.player = this;
    }
    public override void activateQ()
    {
        if (canAttack)
        {
            CooldownQ = false;
            canMove = false;
            IndQ.StartCooldown(CDQ, this, 1);
            forceStopMoving();
            QPos = QPosition.transform.position;
            graphics.transform.LookAt(QPos);
            Anim.SetTrigger("Q");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
        }
    }
    public override void activateW()
    {
        if (canAttack)
        {
            CooldownW = false;
            IndW.StartCooldown(CDW, this, 2);
            Anim.SetBool("WArmed", true);
            isW = true;
            audioManager.audioList[1].Play();
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
        }
    }
    public void HitCreeps()
    {
        for(int i = marks.Count - 1; i >= 0; i--)
        {
            if (marks[i].isDead)
            {
                MurderObject(marks[i].gameObject, 0.1f);
                marks.RemoveAt(i);
            }
            else
            {
                Instantiate(RFX, marks[i].transform.position, Quaternion.identity);
                marks[i].creepOn.takeMagicDamage(RDamage + (int)(AbilityPower * 0.4));
                marks[i].creepOn.takeStun(1.5f);
                MurderObject(marks[i].gameObject, 0.1f);
                marks.RemoveAt(i);
            }
        }
    }
    
    public override void activateE()
    {
        if (canAttack)
        {
            canMove = false;
            forceStopMoving();            
            if (!canRecast)
            {
                EPos = EPosition.position;
                Anim.SetTrigger("E");
                graphics.transform.rotation = EIndicator.transform.rotation;
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
                canRecast = true;
                Invoke("cantEAnymore", 6);
            }else
            {
                CancelInvoke();
                graphics.transform.LookAt(currentSander.transform.position);
                Anim.SetBool("EGrab", true);
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
                currentSander.GetComponent<Sander>().moveBackToMe(sanderPos.position);
                waitingGrab = true;
                currentSander.GetComponent<Sander>().isRooting = true;
                StartCoroutine(waitGrab());
                CooldownE = false;
                IndE.StartCooldown(CDE, this, 3);
                canRecast = false;
            }
            canAttack = false;
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
        }        
    }
    public void playSound(int soundNum)
    {
        audioManager.audioList[soundNum].Play();
    }
    IEnumerator waitGrab()
    {
        while (waitingGrab)
        {
            yield return new WaitForEndOfFrame();
            Anim.SetBool("EGrab", true);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            canMove = false;
        }
        Anim.SetBool("EGrab", false);
        Anim.SetTrigger("FinishE");

    }
    void cantEAnymore()
    {
        canRecast = false;
        CooldownE = false;
        IndE.StartCooldown(CDE, this, 3);
        Destroy(currentSander);
    }
    public override void E()
    {
        currentSander = Instantiate(sander);
        currentSander.transform.position = sanderPos.position;
        Sander sandy = currentSander.GetComponent<Sander>();
        sandy.positionToGo = EPos;
        sandy.player = this;
        sandy.damage = EDamage + (int)(AbilityPower * 0.35);
    }
    void endW()
    {
        isW = false;
        Anim.SetBool("WArmed", false);
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
                    if (isW)
                    {
                        Anim.SetBool("WArmed", false);
                        audioManager.audioList[2].Play();
                        Anim.SetTrigger("W");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                    }
                    else if (autoOn <= 3)
                    {
                        audioManager.audioList[0].PlayDelayed(autoAudDelay);
                        Anim.SetTrigger("Attack" + autoOn);
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                    }
                    else
                    {
                        audioManager.audioList[0].PlayDelayed(autoAudDelay);
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
    public override void abilityDescription()
    {
        Qdesc = "Nate summons an ice explosion on the cursor, exploding after a short delay for " + QDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.55) + ")</color> magic damage. Enemies hit are slowed by " + QSlow + "% for 3 seconds.";
        Wdesc = "Nate empowers his next auto attack to deal an additional " + WDamage + " <color=#32fb93>(+" + (int)(AbilityPower * .5) + ")</color> magic damage, as well as creating an AoE around the enemy struck which deals half as much damage and slows for 30% for 3 seconds.";
        Edesc = "Nate sends a disc sanding wheel forwards, dealing " + EDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.35) + ")</color> magic damage." +
            "\nThe wheel implants itself into the ground, and for the next 6 seconds, Nate can recast this ability to come back to him, dealing the same amount of damage again and also rooting all targets struck for 2 seconds.";
        Rdesc = "Enemies hit by spells are marked for 8 seconds. Casting this spell causes all marks to be detonated, stunning enemies for 1.5 seconds and dealing " 
            + RDamage + " <color=#32fb93>(" + (int)(AbilityPower * 0.4) + ")</color> magic damage.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Ice Explosion");
        IndW.updateAbilityName("(W) Frozen Fingers");
        IndE.updateAbilityName("(E) Sanding Accident");
        IndR.updateAbilityName("(R) Mark of the Creator");
        string Pdesc = "I couldn't think of a passive so I don't have one.";
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityName("I'm overpowered anyways.");
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityDescription(Pdesc);
    }
    public override void uniqueQLevelUp(int level)
    {
        QSlow = qSlowPercent[level];
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
            if (isW)
            {
                Anim.SetBool("WArmed", false);
                audioManager.audioList[2].Play();
                Anim.SetTrigger("W");
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
            }
            else if (autoOn <= 3)
            {
                audioManager.audioList[0].PlayDelayed(autoAudDelay);
                Anim.SetTrigger("Attack" + autoOn);
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
            }
            else
            {
                audioManager.audioList[0].PlayDelayed(autoAudDelay);
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
            autoOn++;
            if (isW)
            {
                isW = false;
                creepSelected.takeMagicDamage(WDamage + (int)(AbilityPower + 0.5));
                creepSelected.takeDamage(AttackDamage);
                GameObject explode = Instantiate(WExplosion);
                explode.transform.position = creepSelected.transform.position;
                NateHitbox slow = explode.GetComponent<NateHitbox>();
                slow.damage = (int)((WDamage + (AbilityPower * 0.5)) / 2);
                slow.slowPotency = 70;
                slow.slowTime = 3;
                slow.player = this;
            }else
            {
                GameObject shot = Instantiate(aaProj);
                AA_Projectile projectile = shot.GetComponent<AA_Projectile>();
                projectile.damage = AttackDamage;
                shot.transform.position = iceSpawn.transform.position;
                projectile.creepTarget = creepSelected.gameObject;
                projectile.player = this;
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
