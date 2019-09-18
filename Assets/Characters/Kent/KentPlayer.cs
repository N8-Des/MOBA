using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KentPlayer : PlayerBase
{
    public AutoRangeChecker rad2;
    AutoRangeChecker backUp;
    public GameObject backUpW;
    private bool WEmp = false;
    private int damageE;
    public GameObject WLoc;
    public GameObject windi2;
    public float WOffset;
    public GameObject BucketParticle;
    //public GameObject autoPart;
    public Hurtbox QHB;
    int numattacks = 0;
    public Hurtbox EHitbox;
    float damageReduction;
    float div;
    int autoOn = 1;
    int numWON;
    bool rangedProj;
    public GameObject projSpawn;
    public List<GameObject> KentUI = new List<GameObject>();
    public Hurtbox EHurt2;
    bool isSwimming;
    public UltDoT RHurtbox;
    public GameObject swimFX;
    float speedHold;
    float extraSpeedR = 2.5f;
    public List<GameObject> meshes = new List<GameObject>();
    public void Awake()
    {
        StartCoroutine(waitingW());
    }
    public override void activateQ()
    {
        if (canAttack)
        {
            if (isSwimming)
            {
                isSwimming = false;
                StartCoroutine(RLoop());
            }
            base.activateQ();
        }
    }
    public override void activateW()
    {
        if (canAttack)
        {
            if (isSwimming)
            {
                isSwimming = false;
                StartCoroutine(RLoop());
            }
            if (numWON >= 8)
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
                if (itemsHad[6])
                {
                    StartCoroutine(hexagon());
                }
            }
            else
            {
                CooldownW = false;
                IndW.StartCooldown(CDW, this, 2);
                backUp = achecker;
                achecker = rad2;
                rad2 = backUp;
                rangedProj = !rangedProj;
            }
        }
    }
    public override void activateE()
    {
        if (canAttack)
        {
            if (isSwimming)
            {
                isSwimming = false;
                StartCoroutine(RLoop());
            }
            base.activateE();
        }
    }
    public override void activateR()
    {
        if (canAttack)
        {
            CooldownR = false;
            IndR.StartCooldown(CDR, this, 4);
            forceStopMoving();
            isSwimming = !isSwimming;
            StartCoroutine(RLoop());
            RHurtbox.Damage = RDamage + (int)(AbilityPower * 0.1);
            foreach (GameObject kentMesh in meshes)
            {
                kentMesh.SetActive(!kentMesh.active);
            }
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
        }
    }
    public void alllstarDash()
    {
        canAttack = false;
        creepSelected = null;       
        Vector3 position = transform.position;
        Vector3 direction = -transform.forward;
        float dash = 10;
        Vector3 dashLoc = position - direction * dash;
        NewPosition = dashLoc;
        EHitbox.gameObject.SetActive(true);
        EHitbox.player = this;
        EHitbox.damage = (int)(AttackDamage * 0.6f);
        EHitbox.damage += EDamage;
        StartCoroutine(dashE(gameObject, dashLoc, 0.2f));
    }
    IEnumerator RLoop()
    {
        if (isSwimming)
        {
            speedHold = speed;
            float newSpeed = speed + extraSpeedR;
            while (isSwimming)
            {
                speed = newSpeed;
                RHurtbox.gameObject.SetActive(true);
                swimFX.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
        }else
        {
            while (!isSwimming)
            {
                Anim.SetBool("isSwimming", false);
                speed = speedHold;
                RHurtbox.gameObject.SetActive(true);
                swimFX.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public void KnockUpAllstar()
    {
        EHurt2.gameObject.SetActive(true);
        EHurt2.damage = (int)(AbilityPower * 1.2f);
        EHurt2.damage += (int)(EDamage * 1.2);
    }
    public IEnumerator dashE(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
        EHitbox.gameObject.SetActive(false);
    }
    public override void WTest()
    {
        if (CooldownW && IndW.levelNum > 0)
        {
            if (numWON >= 8)
            {
                WIndicator = backUpW;
            }else
            {
                WIndicator = windi2;
            }
            stoppingAttack = Input.GetKeyUp(KeyCode.W);
            Ab2 = Input.GetKey(KeyCode.W);
            bool leftClick = Input.GetMouseButton(0);
            bool touch = Input.GetKeyDown(KeyCode.W);
            if (Ab2 && !stoppingAttack && anythingWorks)
            {
                WIndicator.SetActive(true);
                WPressed = true;
            }
            else if ((WPressed || (leftClick && Ab2)) && anythingWorks)
            {
                WIndicator.SetActive(false);
                WPressed = false;
                isInvisible = false;
                activateW();
            }
            else
            {
                WIndicator.SetActive(false);
                WPressed = false;
            }

            if (Ab2 && endingAttack)
            {
                anythingWorks = false;
            }
            if (touch)
            {
                anythingWorks = true;
                endingAttack = false;
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
                if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack && canAttackAfterAuto)
                {
                    NewPosition = transform.position;
                    stopMoving();
                    transform.LookAt(touchedCreep.transform.position);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    if (rangedProj)
                    {
                        if (isSwimming)
                        {
                            isSwimming = false;
                            StartCoroutine(RLoop());
                        }
                        if (numWON > 0)
                        {
                            Anim.SetTrigger("AttackW");
                            Anim.SetBool("isAttacking", true);
                            Anim.SetBool("isIdle", false);
                        }
                    }
                    else
                    {
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
        int BaseEDamage = EDamage + (int)(AttackDamage * 0.55);
        Qdesc = "Kent throws a football forwards, dealing " + QDamage + "<color=orange> (+" + (int)(AttackDamage * 0.75) + ")</color> physical damage to all enemies it passes through";
        Wdesc = "Kent switches to his gun, or back to melee. His bullets deal" 
            + "<color=orange> (" + (int)(AttackDamage * 0.9) + ")</color> damage instead, but slow the enemy by 60%. If Kent has 8 bullets, he consumes them all, and instead fires a massive"
            + " bullet that passes through enemies and deals " + WDamage + "<color=orange> (+" + (int)(AttackDamage * 1.4) + ")</color> physical damage.";
        Edesc = "Kent charges forwards, dealing " + EDamage + "<color=orange> (+" + (int)(AttackDamage * 0.7) + ")</color> physical damage, and knocking back all enemies hit. At the end of the dash, " +
            "Kent knocks up all enemies hit, dealing " + (int)(EDamage * 1.25) + "<color=#32fb93> (+" + (int)(AbilityPower * 1.2) + ")</color> magic damage.";
        Rdesc = "Kent changes into swimming form, gaining movespeed and dealing " + RDamage + "<color=#32fb93> (+" + (int)(AbilityPower * 0.05) + ")</color> magic damage every half second to nearby enemies. \n\nKent cannot attack while this is active.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Firey Football");
        IndW.updateAbilityName("(W) Nerf or Nothing");
        IndE.updateAbilityName("(E) All-Star Dash");
        IndR.updateAbilityName("(R) Feat of the Swimmer");
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
            if (rangedProj)
            {
                if (numWON > 0)
                {
                    Anim.SetTrigger("AttackW");
                    Anim.SetBool("isAttacking", true);
                    Anim.SetBool("isIdle", false);
                }
            }
            else
            {
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
    }
    public override void Q()
    {
        GameObject football = GameObject.Instantiate((GameObject)Resources.Load("KentFootball"));
        football.transform.rotation = QuindRot;
        football.transform.position = transform.position;
        Hurtbox hb = football.GetComponent<Hurtbox>();
        Rigidbody rb = football.GetComponent<Rigidbody>();
        hb.player = this;
        hb.damage = QDamage + (int)(AttackDamage * 0.75);
        rb.velocity = football.transform.forward * 25;
    }
    IEnumerator waitingW()
    {
        yield return new WaitForSeconds(3);
        if (numWON < 8)
        {
            numWON += 1;
            KentUI[numWON - 1].SetActive(true);
        }
        StartCoroutine(waitingW());
    }
    public void shootGun()
    {
        if (!isMoving)
        {
            canAttackAfterAuto = false;
            if (itemsHad[5])
            {
                if (numGorilla <= 10)
                {
                    StartCoroutine(gorilla());
                }
            }
            numWON -= 1;
            KentUI[numWON].SetActive(false);
            GameObject bullet = GameObject.Instantiate((GameObject)Resources.Load("KentBullet"));
            GameObject sound = GameObject.Instantiate((GameObject)Resources.Load("KentNerfSound"));

            AA_Projectile aa = bullet.GetComponent<AA_Projectile>();
            aa.speed = 20;
            aa.damage = (int)(AttackDamage * 0.9);
            aa.KentSlow = true;
            aa.player = this;
            aa.creepTarget = creepSelected.gameObject;
            bullet.transform.position = projSpawn.transform.position;
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
            if (IndQ.levelNum >= 1)
            {
                numattacks += 1;
                if (numattacks >= 3)
                {
                    creepSelected.takeMagicDamage(QDamage + (int)(AbilityPower * 0.35));
                    numattacks = 0;
                }
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
                creepSelected.takeDamage(AttackDamage);
            }
            GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("KentAutoFX"));
            GameObject hitNoise = GameObject.Instantiate((GameObject)Resources.Load("KentAutoNoise"));
            hitNoise.GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.3f);
            effect.transform.position = creepSelected.transform.position;
        }
    }
    public void shootGiantBullet()
    {
        GameObject bigBullet = GameObject.Instantiate((GameObject)Resources.Load("KentBigBullet"));
        bigBullet.transform.rotation = WindRot;
        bigBullet.transform.position = transform.position;
        Hurtbox hb = bigBullet.GetComponent<Hurtbox>();
        Rigidbody rb = bigBullet.GetComponent<Rigidbody>();
        hb.player = this;
        hb.damage = WDamage + (int)(AttackDamage * 1.4);
        rb.velocity = bigBullet.transform.forward * 35;
        numWON = 0;
        foreach (GameObject ui in KentUI)
        {
            ui.SetActive(false);
        }
    }
    public void endDamageAuto()
    {
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
}
