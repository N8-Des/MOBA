using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhilPlayer : PlayerBase {
    public int numberE = 0;
    private int damageE;
    public GameObject WLoc;
    public float WOffset;
    public GameObject RightBoat;
    public GameObject LeftBoat;
    Quaternion LeftBoatRotation;
    Quaternion RightBoatRotation;
    public GameObject BucketParticle;
    public GameObject autoPart;
    public Hurtbox QHB;
    public override void W()
    {
        GameObject puddle = GameObject.Instantiate((GameObject)Resources.Load("SealPuddle"));
        puddle.GetComponent<SlowHurtbox>().slowPotency = WDamage;
        puddle.transform.position = WLoc.transform.position - new Vector3(0, WOffset, 0);        
    }
    public override void R()
    {
        GameObject Boat = GameObject.Instantiate((GameObject)Resources.Load("Boat"));
        Boat.transform.rotation = RindRot;
        Boat.transform.position = RIndicator.transform.position + new Vector3(0, 0.5f, 0);
        Rigidbody rb = Boat.GetComponent<Rigidbody>();
        Boat boatScript = Boat.GetComponent<Boat>();
        boatScript.hurtboxRight.player = this;
        boatScript.hurtboxLeft.player = this;
        boatScript.hurtboxRight.damage = RDamage + (int)(AttackDamage * 0.7);
        boatScript.hurtboxLeft.damage = RDamage + (int)(AttackDamage * 0.7);
        rb.velocity = Boat.transform.forward * 17;
    }
    public void ISawedThisBoatInHalf()
    {
        GameObject Audio = GameObject.Instantiate((GameObject)Resources.Load("I sawed"));
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
            if (Physics.Raycast(raymond, out hit, layerMask))
            {
                NewPosition = hit.point;
                NewPosition.y = 0.6f;
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
                    switch (numberE)
                    {
                        case 0:
                            Anim.SetTrigger("Attack");
                            Anim.SetBool("isAttacking", true);
                            Anim.SetBool("isIdle", false);
                            break;
                        case 1:
                            Anim.SetTrigger("Thats");
                            Anim.SetBool("isAttacking", true);
                            Anim.SetBool("isIdle", false);
                            break;
                        case 2:
                            Anim.SetTrigger("Alotta");
                            Anim.SetBool("isAttacking", true);
                            Anim.SetBool("isIdle", false);
                            break;
                        case 3:
                            Anim.SetTrigger("Damage");
                            Anim.SetBool("isAttacking", true);
                            Anim.SetBool("isIdle", false);
                            break;
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
    public override void Q()
    {
        QHB.damage = QDamage + (int)(AbilityPower * 0.5);
        QHB.player = this;
    }
    public override void abilityDescription()
    {
        int BaseEDamage = EDamage + (int)(AttackDamage * 0.55);
        Qdesc = "Phil launches Flex Seal out of his bucket, spraying all enemies hit. The spray deals " + QDamage + "<color=#32fb93> (+" +
            (int)(AbilityPower * 0.5) + ")</color> magic damage.";
        Wdesc = "Phil throws out some Flex Seal on the ground, slowing all enemies in an area by " + WDamage + "%.";
        Edesc = "Phil does a lotta damage with his next 3 auto attacks. The first attack deals " + EDamage + " <color=orange>(+" + (int)(AttackDamage * 0.55) + ")</color> damage, the second one"
            + " does twice of that (" + (BaseEDamage * 2) + "), and the final attack does double the damage of the second (" + (BaseEDamage * 4) + ").";
        Rdesc = "Phil saws a boat in half, sending it forwards. The boat deals " + RDamage + "<color=orange> (+" + (int)(AttackDamage * 0.4) + ")</color> damage, and knocks enemies up for 2 seconds.";

        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Flex Spray");
        IndW.updateAbilityName("(W) Sticky Flex");
        IndE.updateAbilityName("(E) A LOTTA DAMAGE");
        IndR.updateAbilityName("(R) Boat in Halves");
    }
    public override void AttackCreep(Transform target)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            NewPosition = transform.position;
            stopMoving();
            Quaternion transRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transRot *= new Quaternion(0, 0, 0, 0);
            graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.2f);
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            if (numberE == 0)
            {

                if (autoNum == 1)
                {
                    Anim.SetTrigger("Attack");
                    autoNum = 2;
                }
                else if (autoNum == 2)
                {
                    Anim.SetTrigger("Attack2");
                    autoNum = 1;
                }
            } else
            {
                switch (numberE)
                {
                    case 0:
                        Anim.SetTrigger("Attack");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                        break;
                    case 1:
                        Anim.SetTrigger("Thats");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                        break;
                    case 2:
                        BucketParticle.SetActive(false);
                        Anim.SetTrigger("Alotta");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                        break;
                    case 3:
                        Anim.SetTrigger("Damage");
                        Anim.SetBool("isAttacking", true);
                        Anim.SetBool("isIdle", false);
                        break;
                }
            }
        }
    }
    public void autoFX()
    {
        Instantiate(autoPart, creepSelected.transform.position, autoPart.transform.rotation);
    }
    public void endDamageAuto()
    {
        creepSelected.takeDamage(damageE);
        numberE += 1;
        EndAutoAttack();
        if (numberE <= 3)
        {
            damageE *= 2;
        }
        else
        {
            IndE.StartCooldown(CDE, this, 3);
            numberE = 0;
            Anim.SetBool("isIdle", true);
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
        }
    }

    public override void activateE()
    {
        if (canAttack && numberE == 0)
        {
            BucketParticle.SetActive(true);
            CooldownE = false;
            //IndE.StartCooldown(CDE, this, 3);
            numberE = 1;
            damageE = EDamage + (int)(AttackDamage * 0.55);
            forceStopMoving();
        }
    }
}
