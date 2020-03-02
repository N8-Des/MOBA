using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackPlayer : PlayerBase
{
    public GameObject QLoc;
    public float WOffset;
    public GameObject EPos;
    public Hurtbox QHB;
    int numattacks = 0;
    float div;
    int autoOn = 1;
    public Hurtbox EHB;
    bool isJumping;
    int extraQDamage;
    bool ascended;
    public List<GameObject> ultObjects = new List<GameObject>();
    int numQ;
    GameObject vert1;
    GameObject vert2;
    GameObject vert3;
    public GameObject triangleShot;
    public AudioManager audManager;
    Vector3 offsetScope = new Vector3(0, 0f, 0);
    bool WActive = true;
    public int numW;
    bool invuln;
    bool isMovingE;
    Vector3 dashLoc;
    public override void activateQ()
    {
        if (canAttack)
        {
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
            numQ += 1;
            StartCoroutine(QWait());
            switch (numQ)
            {
                case 1:
                    vert1 = GameObject.Instantiate((GameObject)Resources.Load("JackQPoint"));
                    vert1.transform.position = QLoc.transform.position;
                    vert1.transform.position -= offsetScope;
                    Anim.SetTrigger("Q");
                    break;
                case 2:
                    vert2 = GameObject.Instantiate((GameObject)Resources.Load("JackQPoint"));
                    vert2.transform.position = QLoc.transform.position;
                    vert2.transform.position -= offsetScope;
                    Anim.SetTrigger("Q");
                    break;
                case 3:
                    Anim.SetTrigger("Q2");
                    vert3 = GameObject.Instantiate((GameObject)Resources.Load("JackQPoint"));
                    vert3.transform.position = QLoc.transform.position;
                    vert3.transform.position -= offsetScope;
                    GameObject projectile = Instantiate(triangleShot);
                    projectile.transform.position = new Vector3(vert1.transform.position.x, 3f, vert1.transform.position.z);
                    projectile.GetComponent<MoveToPoint>().target = vert2.transform;
                    projectile.GetComponent<Hurtbox>().damage = QDamage + (int)(AttackDamage * 0.6);
                    GameObject projectile2 = Instantiate(triangleShot);
                    projectile2.transform.position =  new Vector3(vert1.transform.position.x, 3f, vert1.transform.position.z);
                    projectile2.GetComponent<MoveToPoint>().target = vert3.transform;
                    projectile2.GetComponent<Hurtbox>().damage = QDamage + (int)(AttackDamage * 0.6);
                    GameObject projectile3 = Instantiate(triangleShot);
                    projectile3.transform.position = new Vector3(vert3.transform.position.x, 3f, vert3.transform.position.z);
                    projectile3.GetComponent<MoveToPoint>().target = vert2.transform;
                    projectile3.GetComponent<Hurtbox>().damage = QDamage + (int)(AttackDamage * 0.6);
                    audManager.audioList[0].Play();
                    Invoke("endQ", 1.3f);
                    StopAllCoroutines();
                    CooldownQ = false;
                    IndQ.StartCooldown(CDQ, this, 1);
                    numQ = 0;
                    // dont need this actually: projectile.transform.rotation = Quaternion.AngleAxis(Vector3.Angle(vert1.transform.position, vert2.transform.position), Vector3.forward);
                    break;
            }
        }
    }
    void endQ()
    {
        Destroy(vert1.gameObject);
        Destroy(vert2.gameObject);
        Destroy(vert3.gameObject);
    }
    bool isDashing;
    public UltDoT rhitbox;
    public override void activateW()
    {
        //nothing
    }
    IEnumerator QWait()
    {
        yield return new WaitForSeconds(4);
        CooldownQ = false;
        IndQ.StartCooldown(CDQ, this, 1);
        numQ = 0;
        Destroy(vert1);
        Destroy(vert2);
        Destroy(vert3);

    }
    public override void E()
    {
        isDashing = true;
        audManager.audioList[5].Play();
        StartCoroutine(Dash());
    }
    public IEnumerator Dash()
    {
        while (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashLoc, 5 * Time.deltaTime );
            yield return new WaitForEndOfFrame();
        }
    }
    void FixedUpdate()
    {
        if(CooldownW && IndW.levelNum > 0)
        {
            WActive = true;
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
    public override void activateR()
    {
        if (canAttack)
        {
            CooldownR = false;
            canAttack = false;
            RindRot = RIndicator.transform.rotation;
            graphics.transform.rotation = RIndicator.transform.rotation;
            IndR.StartCooldown(CDR, this, 4);
            Anim.SetBool("Ulting", true);
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
            rhitbox.rotation = RindRot;
            rhitbox.Damage = RDamage + (int)(AbilityPower * 0.3);
            Invoke("endR", 7);
        }
    }
    void endR()
    {
        Anim.SetBool("Ulting", false);
        canAttack = true;
    }
    public void endDash()
    {
        isDashing = false;
    }
    public override void QTest()
    {
        if (CooldownQ && IndQ.levelNum > 0)
        {
            stoppingAttack = Input.GetKeyUp(KeyCode.Q);
            Ab1 = Input.GetKey(KeyCode.Q);
            bool leftClick = Input.GetMouseButton(0);
            bool touch = Input.GetKeyDown(KeyCode.Q);
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
    public override void ETest()
    {
        if (CooldownE && IndE.levelNum > 0)
        {
            stoppingAttack = Input.GetKeyUp(KeyCode.E);
            Ab3 = Input.GetKey(KeyCode.E);
            bool leftClick = Input.GetMouseButton(0);
            bool touch = Input.GetKeyDown(KeyCode.E);
            if (Ab3 && canAttack && canMove)
            {
                Vector3 mouse = Input.mousePosition;
                Ray castPoint = Camera.main.ScreenPointToRay(mouse);
                RaycastHit hit;
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, layerMask))
                {
                    dashLoc = hit.point;
                }
                graphics.transform.LookAt(dashLoc);
                activateE();
                canMove = false;
                canAttack = false;
                NewPosition = dashLoc;
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
                    bufferedPosition.y = 1.15f;
                    StartCoroutine(waitToMove());
                    isBuffering = true;
                }
                else if (!cant)
                {
                    canAttackAfterAuto = true;
                    NewPosition = hit.point;
                    NewPosition.y = 1.15f;
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
                    if (CooldownW && WActive && IndW.levelNum > 0)
                    {
                        if (numW <= 3)
                        {
                            audManager.audioList[numW].Play();
                            Anim.SetTrigger("W");
                            Anim.SetBool("isAttacking", true);
                            Anim.SetBool("isIdle", false);
                        }
                        else
                        {
                            audManager.audioList[numW].Play();
                            Anim.SetBool("isAttacking", true);
                            Anim.SetBool("isIdle", false);
                            Anim.SetTrigger("W2");
                        }
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
    public override void abilityDescription()
    {
        Qdesc = "Jack places three points nearby, induvidually. Upon placing the third point, projectiles converge between them, dealing " + QDamage + "<color=orange> (+" + (int)(AttackDamage * 0.6) + ")</color> physical damage to enemies caught.";
        Wdesc = "Periodically, Jack enters an enraged state, in which his next 4 auto attacks will come out quickly, with the final attack dealing " + WDamage + "<color=orange> (+" + (int)(AttackDamage * 1.45) + ")</color> damage to the enemy struck.";
        Edesc = "Jack rolls in the target direction, becoming invincible for some of the duration. If Jack were to be hit while he is invincible, this ability's cooldown is set to 1 second.";
        Rdesc = "Jack summons his deepest power, launching waves of nonsense. Enemies who are in the nonsense radius will take " + RDamage + "<color=#32fb93> (+" + (int)(AbilityPower * 0.3) + ")</color> damage every half second.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Trigonometry");
        IndW.updateAbilityName("(W) Are You Kidding Me?");
        IndE.updateAbilityName("(E) Dodge Roll");
        IndR.updateAbilityName("(R) Unyielding Voice");
        string Pdesc = "Jack Bonney is immune to sound-based damage";
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityName("Yielding Ear");
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
            if (CooldownW && WActive && IndW.levelNum > 0)
            {
                if (numW <= 3)
                {
                    audManager.audioList[numW].Play();
                    Anim.SetTrigger("W");
                    Anim.SetBool("isAttacking", true);
                    Anim.SetBool("isIdle", false);
                }
                else
                {
                    audManager.audioList[numW].Play();
                    Anim.SetBool("isAttacking", true);
                    Anim.SetBool("isIdle", false);
                    Anim.SetTrigger("W2");
                }
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
    public void setInvuln(int inv)
    {
        if (inv == 0)
        {
            invuln = false;
        }else
        {
            invuln = true;
        }
    }
    public override void hitCreepWithAuto()
    {
        if (!isMoving)
        {
            if (WActive && IndW.levelNum > 0)
            {
                numW += 1;
            }
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
                GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("JackAutoFlash"));
                effect.transform.position = creepSelected.transform.position;
            }
            else if(WActive && numW >= 5 && IndW.levelNum > 0)
            {
                creepSelected.takeDamage(WDamage + (int)(AttackDamage * 1.45));
                GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("JackAutoFlashW"));
                effect.transform.position = creepSelected.transform.position;
                WActive = false;
                IndW.StartCooldown(CDW, this, 2);
                CooldownW = false;
                numW = 1;
            }

            else
            {
                creepSelected.takeDamage(AttackDamage);
                GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("JackAutoFlash"));
                effect.transform.position = creepSelected.transform.position;
            }
        }
    }
    public override void takeDamage(int damage, bool magic, bool sound)
    {
        if (invuln)
        {
            IndE.StartCooldown(1f, this, 3);
        }else if (sound)
        {
            //Yielded
        }else
        {
            base.takeDamage(damage, magic, false);
        }
    }
    public void endDamageAuto()
    {
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
}
