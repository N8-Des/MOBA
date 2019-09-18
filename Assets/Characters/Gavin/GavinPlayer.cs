using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GavinPlayer : PlayerBase
{
    private bool WEmp = false;
    private int damageE;
    public GameObject WLoc;
    public float WOffset;
    //public GameObject autoPart;
    public Hurtbox QHB;
    int numattacks = 0;
    public GameObject EHitbox;
    float damageReduction;
    float div;
    public override void activateW()
    {
        CooldownW = false;
        WEmp = true;
        if (itemsHad[6])
        {
            StartCoroutine(hexagon());
        }
    }
    public void dashSword()
    {
        Vector3 position = transform.position;
        Vector3 direction = -transform.forward;
        float dash = 10;
        Vector3 dashLoc = position - direction * dash;
        NewPosition = dashLoc;
        EHitbox.SetActive(true);
        Hurtbox ehb = EHitbox.GetComponent<Hurtbox>();
        ehb.player = this;
        ehb.damage += (int)(AttackDamage * 0.55f);
        ehb.damage += EDamage;
        StartCoroutine(dashE(gameObject, dashLoc, 0.6f));

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
        EHitbox.SetActive(false);
        EndAttack();
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
                if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack)
                {
                    NewPosition = transform.position;
                    stopMoving();
                    transform.LookAt(touchedCreep.transform.position);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    if (WEmp)
                    {
                        Anim.SetTrigger("W");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                    } else
                    {
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
        int BaseEDamage = EDamage + (int)(AttackDamage * 0.55);
        Qdesc = "Every third attack, Gavin deals an additional " + QDamage + "<color=#32fb93> (+" +
            (int)(AbilityPower * 0.35) + ")</color> magic damage.";
        Wdesc = "Gavin empowers his next attack, dealing an additional " + WDamage + "<color=#32fb93> (+" + (int)(AbilityPower * 0.45) + ")</color> <color=orange>(+" + 
            (int)(AttackDamage * 0.8) + ")</color> physical damage.";
        Edesc = "Gavin dashes in a direction, dealing " + EDamage + "<color=orange> (+" + (int)(AttackDamage * 0.55) + ")</color> physical damage.";
        Rdesc = "Gavin takes reduced damaged based on his missing health, up to " + RDamage + "% damage reduction.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) 3-Hit Passive");
        IndW.updateAbilityName("(W) Spinny Strike");
        IndE.updateAbilityName("(E) Beyblade Dash");
        IndR.updateAbilityName("(R) Unkillable");
    }
    public override void AttackCreep(Transform target)
    {
        if (!isAttacking&& creepSelected != null)
        {
            isAttacking = true;
            NewPosition = transform.position;
            stopMoving();
            Quaternion transRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transRot *= new Quaternion(0, 0, 0, 0);
            graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            if (!WEmp)
            {
                Anim.SetTrigger("Attack");
                autoNum = 2;
            }
            else
            {
                Anim.SetTrigger("W");                
            }
        }
    }
    public override void activateQ()
    {
        //nothing lol its a passive
    }
    public override void activateR()
    {
        //nothing lol its a passive
    }
    public void autoFX()
    {
        //Instantiate(autoPart, creepSelected.transform.position, autoPart.transform.rotation);
    }
    public override void passiveUpdate()
    {
        if (IndR.levelNum > 0)
        {
            div = (float)health / (float)maxHealth;
            damageReduction = RDamage * div;
        }
    }
    public override void hitCreepWithAuto()
    {
        if (!isMoving)
        {
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
                GameObject effect2 = GameObject.Instantiate((GameObject)Resources.Load("GavinAutoHitFX"));
                effect2.transform.position = creepSelected.transform.position;
            } else
            {
                creepSelected.takeDamage(AttackDamage);
            }
            GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("GavinAutoHitFX"));
            effect.transform.position = creepSelected.transform.position;
        }
    }
    public void dealDamageW()
    {
        hitCreepWithAuto();
        creepSelected.takeMagicDamage(WDamage + (int)(AbilityPower * 0.45) + (int)(AttackDamage * 0.8));
    }
    public void endDamageAuto()
    {
        if (WEmp)
        {
            WEmp = false;
            IndW.StartCooldown(CDW, this, 2);
        }

        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
    public override void takeDamage(int damage)
    {
        if (damageReduction != 0)
        {
            health -= (int)(damage * (damageReduction / 100));
        }
        else
        {
            health -= damage;
        }
        if (health < 0)
        {
            //die
        }
        else
        {
            float div = (float)health / (float)maxHealth;
            healthbar.fillAmount = div;
            healthdiv.text = health.ToString() + "/" + maxHealth.ToString();
        }
    }
}
