using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCNPlayer : PlayerBase {
    public int ExtraDamageQ = 0;
    public GameObject EHitbox;
    public GameObject RVisiual;
    public GameObject RFX;
    public Hurtbox RHitbox;
    public Material darkWing;
    public Material normalWing;
    public GameObject wings;
    float slowPotentcy = 20;
    public GameObject WArea;
    public float ballSpeed = 5;
    public GameObject BallSpawn;
    public List<int> slowPotLevel = new List<int>();
    public GameObject autoPart;
    public override void Q()
    {
        GameObject DarkBall = GameObject.Instantiate((GameObject)Resources.Load("DarkBall"));
        DarkBall.transform.rotation = QuindRot;
        DarkBall.transform.position = BallSpawn.transform.position;
        DarkBall db = DarkBall.GetComponent<DarkBall>();
        Rigidbody rb = DarkBall.GetComponent<Rigidbody>();
        int extraAPScale = (int)(AbilityPower * 0.45);
        db.fcn = this;
        db.damage += ExtraDamageQ;
        db.damage += extraAPScale;
        db.player = this;
        rb.velocity = DarkBall.transform.forward * ballSpeed;
    }
    public override void W()
    {
        GameObject GroundUp = GameObject.Instantiate((GameObject)Resources.Load("Empowered Ground"));
        KnockUpHurtbox kbhb = GroundUp.GetComponent<KnockUpHurtbox>();
        kbhb.damage += (int)(AbilityPower * 0.7f);
        kbhb.damage += (int)(AttackDamage * 0.2f);
        kbhb.player = this;
        GroundUp.GetComponent<SlowHurtbox>().slowPotency = slowPotentcy;
        GroundUp.transform.position = WArea.transform.position;
    }
    public override void E()
    {
        Vector3 position = transform.position;
        Vector3 direction = -transform.forward;
        float dash = 10;
        Vector3 dashLoc = position - direction * dash;
        NewPosition = dashLoc;
        EHitbox.SetActive(true);
        Hurtbox ehb = EHitbox.GetComponent<Hurtbox>();
        ehb.player = this;
        ehb.damage += (int)(AttackDamage * 0.5f);
        ehb.damage += (int)(AbilityPower * 0.3f);
        StartCoroutine(dashE(gameObject, dashLoc, 0.6f));
    }
    public override void R()
    {
        RVisiual.SetActive(true);
        RFX.SetActive(true);
        RHitbox.damage += (int)(AbilityPower * 0.3f);
        RHitbox.player = this;
        wings.GetComponent<SkinnedMeshRenderer>().material = darkWing;
        StartCoroutine(ultEnd());
    }

    public override void uniqueWLevelUp(int level)
    {
        slowPotentcy = slowPotLevel[level];
    }
    IEnumerator ultEnd()
    {
        yield return new WaitForSeconds(20);
        RVisiual.SetActive(false);
        RFX.SetActive(false);
        wings.GetComponent<SkinnedMeshRenderer>().material = normalWing;
    }
    public override void abilityDescription()
    {
        Qdesc = "Flying Chicken Nugget summons dark energy, turning it into a projectile dealing "
            + QDamage + "<color=#32fb93> (+" + (int)(AbilityPower * 0.45) + ")</color> magic damage. Hitting an enemy with this ability will increase its max damage by 1, up to 500.";
        Wdesc = "Flying Chicken Nugget casts a rune on the ground, which explodes upward with dark energy, knocking enemies up and dealing <color=white>" + WDamage + "</color> <color=#32fb93>(+" + (int)(AbilityPower * 0.7f) + ")</color> "
            + "<color=orange>(+" + (int)(AttackDamage * 0.2f) + ")</color> magic damage, and also slows enemies by " + slowPotentcy + "%.";
        Edesc = "Flying Chicken Nugget winds up, then dashes a long distance, dealing " + EDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.3f) + ")</color> <color=orange>(+" + (int)(AttackDamage * 0.5f) + ")</color> physical damage.";
        Rdesc = "Channeling his true form, Billy Bob Joe, King of the Flying Chicken Nuggets, deals " + RDamage + " <color=#32fb93>(+" + (int)(AbilityPower * 0.3f) + ")</color> magic damage in a large circle."
            + "Then, for the next 20 seconds, Flying Chicken Nugget has an AoE field around him dealing 40 magic damage every half second.";
        IndQ.updateAbilityDescription(Qdesc);
        IndW.updateAbilityDescription(Wdesc);
        IndE.updateAbilityDescription(Edesc);
        IndR.updateAbilityDescription(Rdesc);
        IndQ.updateAbilityName("(Q) Dark Ball");
        IndW.updateAbilityName("(W) Dark Rune");
        IndE.updateAbilityName("(E) Slash n' Dash");
        IndR.updateAbilityName("(R) Final Form");
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
    public void autoFX()
    {
        Instantiate(autoPart, creepSelected.transform.position, autoPart.transform.rotation);
    }
    public override void activateR()
    {
        if (canAttack)
        {
            CooldownR = false;
            canMove = false;
            RindRot = RIndicator.transform.rotation;
            IndR.StartCooldown(CDR, this, 4);
            forceStopMoving();
            Anim.SetTrigger("R");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
        }
    }
}
