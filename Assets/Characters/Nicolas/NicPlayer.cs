using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NicPlayer : PlayerBase
{
    //public GameObject autoPart;
    public Hurtbox QHB;
    public Hurtbox EHitbox;
    float damageReduction;
    float div;
    int autoOn = 1;
    public GameObject projSpawn;
    bool QTargetted;
    Creep creepQ;
    public GameObject hook;
    public GameObject hookObject;
    float followSharp = 0.1f;
    Vector3 followOffset;
    Vector3 upwardsOffset = new Vector3(0, 3, 0);
    [SerializeField]
    GameObject Qind2;
    bool QSlamming = false;
    Vector3 hookpos;
    bool goingUp;
    bool ticAuto;
    [SerializeField]
    GameObject leftTic;
    [SerializeField]
    GameObject rightTic;
    [SerializeField]
    GameObject shieldVis;
    public List<float> extraMaxShield = new List<float>();
    public List<int> shieldBaseScale = new List<int>();
    int shieldDMG;
    float maxHPShield;
    bool movingHookE;
    void OnEnable()
    {
        shieldDMG = shieldBaseScale[0];
        maxHPShield = extraMaxShield[0];
        StartCoroutine(ticcolas());
    }
    public void hitQ(Creep creepHit)
    {
        StartCoroutine(endQ());
        creepQ = creepHit;
        followOffset = creepQ.transform.position - transform.position;
        QTargetted = true;
        Anim.SetBool("hooked", true);
        hook.SetActive(true);
    }
    public override void activateQ()
    {
        if (canAttack)
        {
            if (QTargetted)
            {
                hookpos = hookObject.transform.position;
                QSlamming = true;
                canMove = false;
                forceStopMoving();
                hookObject.transform.position = hookpos;
                graphics.transform.LookAt(Qind2.transform.position);
                Anim.SetTrigger("QSlam");
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
                //QHB.damage = QDamage + (int)(AbilityPower * 0.75);
                canAttack = false;
            }else
            {
                QSlamming = false;
                canMove = false;
                forceStopMoving();
                QuindRot = QIndicator.transform.rotation;
                graphics.transform.rotation = QIndicator.transform.rotation;
                Anim.SetTrigger("Q");
                Anim.SetBool("isAttacking", true);
                Anim.SetBool("isIdle", false);
                QHB.damage = QDamage + (int)(AbilityPower * 0.75);
                canAttack = false;
                if (itemsHad[6])
                {
                    StartCoroutine(hexagon());
                }
            }
        }
    }
    public override void uniqueWLevelUp(int level)
    {

    }
    IEnumerator endQ()
    {
        yield return new WaitForSeconds(6);
        endStunQ();
    }
    public void missedQ()
    {
        CooldownQ = false;
        IndQ.StartCooldown(CDQ, this, 1);
        EndAttack();
    }
    void endStunQ()
    {
        creepQ.isQdNic = false;
        CooldownQ = false;
        IndQ.StartCooldown(CDQ, this, 1);
        QTargetted = false;
        Anim.SetBool("hooked", false);
        hook.SetActive(false);
        creepQ.isStunned = false;
        creepQ.canMove = true;
        QSlamming = false;
    }
    void LateUpdate()
    {
        if (QTargetted && !movingHookE)
        {
            if (!QSlamming)
            {
                Vector3 targetPos = transform.position - followOffset;
                targetPos.y = hookObject.transform.position.y - 0.5f;
                hookObject.transform.position += (targetPos - hookObject.transform.position) * -1.1f;
            }else
            {
                if (goingUp)
                {
                    hookObject.transform.position = hookpos + upwardsOffset;
                }else
                {
                    hookObject.transform.position = hookpos;
                }

            }
        }else
        {
            Qind2.SetActive(false);
        }
    }
    public override void QTest()
    {
        if (CooldownQ && IndQ.levelNum > 0)
        {
            stoppingAttack = Input.GetKeyUp(KeyCode.Q);
            Ab1 = Input.GetKey(KeyCode.Q);
            bool leftClick = Input.GetMouseButton(0);
            bool touch = Input.GetKeyDown(KeyCode.Q);
            if (QTargetted)
            {
                if (Ab1 && !stoppingAttack && anythingWorks)
                {
                    Qind2.SetActive(true);
                    Qind2.transform.position = creepQ.transform.position - new Vector3(0, -1, 0);
                    QPressed = true;
                }
                else if (QPressed || (leftClick && Ab1))
                {
                    Qind2.SetActive(false);
                    QPressed = false;
                    isInvisible = false;
                    activateQ();
                }
                else
                {
                    Qind2.SetActive(false);
                    QPressed = false;
                }
                if (Ab1 && endingAttack)
                {
                }

                if (touch)
                {
                    anythingWorks = true;
                    endingAttack = false;
                }
            }
            else
            {
                if (Ab1 && !stoppingAttack && anythingWorks)
                {
                    QIndicator.SetActive(true);
                    QPressed = true;
                }
                else if (QPressed || (leftClick && Ab1))
                {
                    QIndicator.SetActive(false);
                    QPressed = false;
                    isInvisible = false;
                    activateQ();
                }
                else
                {
                    QIndicator.SetActive(false);
                    QPressed = false;
                }
                if (Ab1 && endingAttack)
                {
                }

                if (touch)
                {
                    anythingWorks = true;
                    endingAttack = false;
                }
            }
        }
    }
    public void MoveHook()
    {
        QSlamming = true;
        goingUp = true;
        hookObject.transform.position = hookpos + upwardsOffset;
    }
    IEnumerator goDown()
    {
        goingUp = false;
        hookObject.transform.position = hookpos;
        creepQ.gameObject.transform.position = hookpos;
        yield return new WaitForSeconds(0.1f);
        GameObject QCrater = GameObject.Instantiate((GameObject)Resources.Load("NicQHB"));
        QCrater.transform.position = hookObject.transform.position;
        NicSpecialHitbox qhb2 = QCrater.GetComponent<NicSpecialHitbox>();
        qhb2.damage = QDamage + (int)(AbilityPower * .75);
        qhb2.player = this;
        qhb2.creepTarget = creepQ;
        QSlamming = false;
        endStunQ();
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
                if (!canAttackAfterAuto)
                {
                    bufferedPosition = hit.point;
                    bufferedPosition.y = 0.5f;
                    StartCoroutine(waitToMove());
                    isBuffering = true;
                }
                else
                {
                    canAttackAfterAuto = true;
                    NewPosition = hit.point;
                    NewPosition.y = 0.5f;
                    EndAttack();
                }
            }
            if (Physics.Raycast(raymond, out hit, Mathf.Infinity, creepsOnly) && hit.transform.tag == "Creep")
            {
                Creep touchedCreep = hit.transform.gameObject.GetComponent<Creep>();
                creepSelected = touchedCreep;
                if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack && !QTargetted)
                {
                    NewPosition = transform.position;
                    stopMoving();
                    transform.LookAt(touchedCreep.transform.position);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    if (autoOn <= 2)
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
    public override void W()
    {
        addShield(shieldDMG + (int)(maxHealth * maxHPShield), 7);
        StartCoroutine(sheildLoop());
    }
    public override void E()
    {
        movingHookE = true;
        EHitbox.damage = EDamage + (int)(AbilityPower * 0.4);
        if (QTargetted)
        {
            EHitbox.damage += (int)(creepQ.maxHealth * 0.07);
        }
    }
    public override void EndAttack()
    {
        movingHookE = false;
        base.EndAttack();
    }
    IEnumerator ticcolas()
    {
        yield return new WaitForSeconds(8);
        Anim.SetTrigger("Tic");
        if (IndW.levelNum > 0)
        {
            ticAuto = true;
            leftTic.SetActive(true);
            rightTic.SetActive(true);

        }
        StartCoroutine(ticcolas());
    }
    public override void abilityDescription()
    {
        int BaseEDamage = EDamage + (int)(AttackDamage * 0.55);
        Qdesc = "Nic throws his hook, latching onto and stunning the first enemy hit for 6 seconds, and dealing " + QDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.75) + ")</color> magic damage."
            + "\nWhile the enemy is stunned, Nic can reactivate this ability to slam it into the ground, dealing the same damage again to all nearby units, and knocking them up for 1 second.";
        Wdesc = "TICCOLAS: Every 8 seconds, Nic tics, empowering his next auto attack to deal an additional " + WDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.65) + ")</color> magic damage." +
            "\nTHICCOLAS: Nic shields himself for " + shieldDMG + "<color=red> (+" + (int)(maxHealth * maxHPShield) + ")</color> damage for 7 seconds.";
        Edesc = "Nic spins his hook in a circle, knocking back all hit enemy units and dealing " + EDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.4) + ")</color> magic damage. If an enemy is currently hooked, "
            + "this attack deals additional damage equal to 7% of the hooked enemy's maximum health.";
        Rdesc = "Gavin takes reduced damaged based on his missing health, up to " + RDamage + "% damage reduction.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.abilityDescription.GetComponentInChildren<Text>().fontSize = 48;
        IndQ.updateAbilityName("(Q) Grab of Death");
        IndW.updateAbilityName("(W) Ticcolas/Thiccolas");
        IndE.updateAbilityName("(E) Flayer");
        IndR.updateAbilityName("(R) Summon: Monica");
    }
    public override void AttackCreep(Transform target)
    {
        if (!isAttacking && creepSelected != null && !QTargetted)
        {
            isAttacking = true;
            NewPosition = transform.position;
            stopMoving();
            Quaternion transRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transRot *= new Quaternion(0, 0, 0, 0);
            graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            if (autoOn <= 2)
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
                takeDamage((int)(AttackDamage * -0.2));
            }
            if (hexagonAttack)
            {
                creepSelected.takeDamage(AttackDamage + 100);
                hexagonAttack = false;
            }
            else
            {
                if (ticAuto)
                {
                    creepSelected.takeMagicDamage(WDamage + (int)(AbilityPower * 0.65));
                    ticAuto = false;
                    leftTic.SetActive(false);
                    rightTic.SetActive(false);
                }
                creepSelected.takeDamage(AttackDamage);
                ticAuto = false;
            }
            GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("NicAutoFlash"));
            //GameObject hitNoise = GameObject.Instantiate((GameObject)Resources.Load("KentAutoNoise"));
            //hitNoise.GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.3f);
            effect.transform.position = creepSelected.transform.position;
        }
    }
    IEnumerator sheildLoop()
    {
        while (hasShield)
        {
            shieldVis.SetActive(true);
            yield return new WaitForEndOfFrame();
        }
        shieldVis.SetActive(false);
    }
    public void endDamageAuto()
    {
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
}
