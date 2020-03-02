using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoilerBoss : Creep
{
    public List<string> bossAttacks = new List<string>();
    bool isIdle = true;
    public GameObject lildude;
    [SerializeField]
    Transform lildudeSpawn;
    public bool canBite = true;
    public WaterHeater whL;
    public WaterHeater whR;
    public BossHealthbar healthbarBoss;
    public GameObject pipe1;
    public GameObject pipe2;
    public bool inHealingState;
    int pipesKilled;
    bool activatedHealing;
    public GameObject pipeJump;
    Vector3 pipOffset;
    public override void Start()
    {
        StartCoroutine(aiCycle());
    }
    public override void AI()
    {
        if (playerInAutoRadius && canBite)
        {
            anim.SetTrigger("Bite");
            canBite = false;
        }
    }
    public override void takeStun(float duration)
    {  
    }
    public override void takeKnockback(Vector3 PlaceToGo, float speed, float duration)
    {
    }
    public override void takeQ(GameObject hook)
    {
    }
    public override void knockUp(float knockTime)
    {
    }
    public void heal(int pipeOn)
    {
        if (inHealingState)
        {
            if (pipeOn == 0)
            {
                if (pipe1.activeSelf == true)
                {
                    health += 15;
                    GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextPlayerHeal"));
                    dmgNum.transform.SetParent(canvas1.transform);
                    dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
                    dmgNum.GetComponent<DamageNum>().damageText = "15";
                    healthbarBoss.takeDamage(health, maxHealth);
                }
            }
        }
        else
        {
             if (pipe2.activeSelf == true)
                {
                    health += 15;
                    GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextPlayerHeal"));
                    dmgNum.transform.SetParent(canvas1.transform);
                    dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
                    dmgNum.GetComponent<DamageNum>().damageText = "15";
                    healthbarBoss.takeDamage(health, maxHealth);
            }
        }
    }
    public override bool takeDamage(int damage)
    {
        int newDamage = (int)(damage * (100.0f / (100.0f + armor)));
        health -= newDamage;
        GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageText"));
        dmgNum.transform.SetParent(canvas1.transform);
        dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
        dmgNum.GetComponent<DamageNum>().damageText = newDamage.ToString();
        healthbarBoss.takeDamage(health, maxHealth);
        if (health <= 0)
        {
            //die
            return true;
        }
        if (health <= (maxHealth / 2) && !activatedHealing)
        {
            anim.SetTrigger("RegularPipes");
            anim.SetBool("isIdle", true);
            anim.SetBool("isAttacking", true);
            canAttack = false;
            activatedHealing = true;
            StartCoroutine(secondPhase());
            inHealingState = true;
        }
        return false;
    }
    IEnumerator secondPhase()
    {
        yield return new WaitForSeconds(2.3f);
        GameObject pip = Instantiate(pipeJump);
        pipOffset.x = player.transform.position.x;
        pipOffset.y = 0.06f;
        pipOffset.z = player.transform.position.z;
        pip.transform.position = pipOffset;
        StartCoroutine(secondPhase());
    }
    public void startHeal()
    {
        anim.SetBool("PipeHeal", true);
    }
    public override bool takeMagicDamage(int damage)
    {
        health -= (int)(damage * (100.0f / (100.0f + magicResist)));
        StartCoroutine(MagicDelay((int)(damage * (100.0f / (100.0f + magicResist)))));
        healthbarBoss.takeDamage(health, maxHealth);
        if (health <= 0)
        {
            //die
            return true;
        }
        if (health <= (maxHealth / 2) && !activatedHealing)
        {
            anim.SetTrigger("RegularPipes");
            anim.SetBool("isIdle", true);
            anim.SetBool("isAttacking", true);
            StartCoroutine(secondPhase());
            canAttack = false;
            activatedHealing = true;
            inHealingState = true;
        }
        return false;
    } 
    public void killPipe(int pipeNum)
    {
        if (pipeNum == 0)
        {
            pipe1.SetActive(false);
        }else
        {
            pipe2.SetActive(false);
        }
    }
    public void pipeAnim(bool Left)
    {
        if (Left)
        {
            anim.SetTrigger("LoseL");
            anim.SetBool("isIdle", false);
        }else
        {
            anim.SetTrigger("LoseR");
            anim.SetBool("isIdle", false);
        }
    }
    public void LookRot()
    {
        whL.lookAtPlayerPos(player.transform.position);
        whR.lookAtPlayerPos(player.transform.position);
    }
    IEnumerator aiCycle()
    {
        yield return new WaitForSeconds(2.8f);
        if (!inHealingState)
        {
            canAttack = false;
            isAttacking = true;
            isIdle = false;
            anim.SetBool("isIdle", false);
            anim.SetBool("isAttacking", true);
            //randomly select an animation to follow
            int randAnim = Random.Range(0, bossAttacks.Count);
            anim.SetTrigger(bossAttacks[randAnim]);
        }    
    }
    public void spawnDude()
    {
        GameObject go = Instantiate(lildude);
        go.transform.position = lildudeSpawn.position;
        go.GetComponent<LittleManAi>().player = player;
    }
    public void endBite()
    {
        Invoke("invokedBite", 3);
    }
    public void EndAttack()
    {
        canAttack = true;
        isAttacking = false;
        isIdle = true;
        anim.SetBool("isIdle", true);
        anim.SetBool("isAttacking", false);
        StartCoroutine(aiCycle());
    }
    void invokedBite()
    {
        canBite = true;
    }
}
