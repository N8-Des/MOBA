using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GavinPlayer : PlayerBase
{
    public GavinPassiveHitbox passiveHitbox;
    private bool WEmp = false;
    private int damageE;
    public GameObject WLoc;
    public float WOffset;
    //public GameObject autoPart;
    public Hurtbox QHB;
    int numattacks = 0;
    public GameObject EHitbox;
    float damageReduction;
    int extraAD = 0;
    float div;
    string Pdesc;
    public AudioManager aManager;
    public override void activateW()
    {
        CooldownW = false;
        WEmp = true;
        if (itemsHad[6])
        {
            StartCoroutine(hexagon());
        }
    }
    public void playSound(int soundNum)
    {
        aManager.audioList[soundNum].Play();
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
        ehb.damage = (int)(AttackDamage * 0.55f) + EDamage;
        StartCoroutine(dashE(gameObject, dashLoc, 0.6f));

    }

    public IEnumerator dashE(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds && !stopDash)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        stopDash = false;
        //objectToMove.transform.position = end;
        NewPosition = transform.position;
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
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityName("Public Embarrasment");
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityDescription(Pdesc);
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
        extraAD = 5 * (passiveHitbox.creeps.Count);
        string Pdesc = "Gavin gains increased damage based on the enemies around him. \n\n\nCurrent auto attack damage bonus: " + extraAD;
        passiveDesc.GetComponentInParent<AbilityIndicator>().updateAbilityDescription(Pdesc);
        if (IndR.levelNum > 0)
        {
            div = (maxHealth - health);
            div /= (float)maxHealth;
            damageReduction = 45 * div;
            Debug.Log(damageReduction);
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
                    creepSelected.takeDamage(AttackDamage + extraAD);
                }
            }
            if (itemsHad[10])
            {
                takeDamage((int)(AttackDamage * -0.2), false, false);
            }
            if (hexagonAttack)
            {
                creepSelected.takeDamage(AttackDamage + 100 + extraAD);
                GameObject effect2 = GameObject.Instantiate((GameObject)Resources.Load("GavinAutoHitFX"));
                effect2.transform.position = creepSelected.transform.position;
            } else
            {
                creepSelected.takeDamage(AttackDamage + extraAD);
            }
            GameObject effect = GameObject.Instantiate((GameObject)Resources.Load("GavinAutoHitFX"));
            effect.transform.position = creepSelected.transform.position;
        }
    }
    public void dealDamageW()
    {
        hitCreepWithAuto();
        creepSelected.takeMagicDamage(WDamage + (int)(AbilityPower * 0.45) + (int)(AttackDamage * 0.8));
        WEmp = false;
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

    public override void takeDamage(int damage, bool magic, bool sound)
    {
        if (damageReduction != 0)
        {
            damage = (int)(damage * (float)(damageReduction / 100.0f));
            if (damage <= 0)
            {
                damage = 1;
            }
        }
        if (!magic)
        {
            if (hasShield)
            {
                currentShield -= damage;
                float div = (float)health / ((float)maxHealth + (float)currentShield);
                healthbar.fillAmount = div;
                shieldbar.fillAmount = ((float)health + (float)currentShield) / ((float)maxHealth + (float)currentShield);
                healthdiv.text = health.ToString() + "/" + (maxHealth + currentShield).ToString();
                if (currentShield <= 0)
                {
                    hasShield = false;
                    takeDamage(0, false, false);
                }
            }
            else
            {
                if (damage <= -25)
                {
                    health -= damage;
                    GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextPlayerHeal"));
                    dmgNum.transform.SetParent(canvasPlayer.transform);
                    dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
                    dmgNum.GetComponent<DamageNum>().damageText = Mathf.Abs(damage).ToString();
                }
                else if (damage > 0)
                {
                    float physRes = 100.0f / (Armor + 100.0f);
                    newDamage = (int)(damage * physRes);
                    health -= newDamage;
                    GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextPlayer"));
                    dmgNum.transform.SetParent(canvasPlayer.transform);
                    dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
                    dmgNum.GetComponent<DamageNum>().damageText = newDamage.ToString();
                }
                else
                {
                    newDamage = damage;
                    health -= newDamage;
                    float div = (float)health / (float)maxHealth;
                    healthbar.fillAmount = div;
                    healthdiv.text = health.ToString() + "/" + maxHealth.ToString();
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
        else
        {

            if (hasShield)
            {
                currentShield -= damage;
                float div = (float)health / ((float)maxHealth + (float)currentShield);
                healthbar.fillAmount = div;
                shieldbar.fillAmount = ((float)health + (float)currentShield) / ((float)maxHealth + (float)currentShield);
                healthdiv.text = health.ToString() + "/" + (maxHealth + currentShield).ToString();
                if (currentShield <= 0)
                {
                    hasShield = false;
                    takeDamage(0, false, false);
                }
            }
            else
            {
                float magRes = 100.0f / (Armor + 100.0f);
                newDamage = (int)(damage * magRes);
                if (damage <= -25)
                {
                    GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextPlayerHeal"));
                    dmgNum.transform.SetParent(canvasPlayer.transform);
                    health -= damage;
                    dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
                    dmgNum.GetComponent<DamageNum>().damageText = Mathf.Abs(damage).ToString();
                }
                else if (damage > 0)
                {
                    health -= newDamage;
                    GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextPlayerMagic"));
                    dmgNum.transform.SetParent(canvasPlayer.transform);
                    dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
                    dmgNum.GetComponent<DamageNum>().damageText = newDamage.ToString();
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
    }
}
