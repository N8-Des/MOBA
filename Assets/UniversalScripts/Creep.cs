using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Creep : MonoBehaviour {
    public int maxHealth = 300;
    public int health = 300;
    public bool isDead = false;  
    public bool canAttack = true;
    public float Speed;
    public bool takingDamage;
    public bool canMove = true;
    public bool isStunned;
    public Vector3 trailOffset;
    public GameObject Nexus;
    public MonicaController monica;
    public bool playerInRadius = false;
    public bool playerInAutoRadius = false;
    public bool monicaInRadius;
    public bool monicaInAutoRadius;
    protected Vector3 NewPosition;
    public float NexusAttackRange;
    public Animator anim;
    public GameObject canvas1;
    protected bool isMoving;
    public PlayerBase player;
    public bool isAttacking;
    public bool attackDelay;
    public int damage;
    public Vector3 offset = new Vector3(0, 1, 0);
    public AnchorUI healthbar;
    Vector3 minionDistance;
    Vector3 moveOffset;
    public PositionRadius posRad;
    public Rigidbody rb;
    bool inMinion;
    float randDiffx;
    float randDiffz;
    Vector3 randDiff;
    public int gold;
    public int armor;
    public int magicResist;
    bool armorShredded;
    bool mrShredded;
    public bool isQdNic;
    public GameManager gameManager;
    protected bool attackingMonica;
    Vector3 movePos = new Vector3(0, -10, 0);
    public bool isSlowed;
    public bool isRanged;
    protected GameObject trailSpawn;
    public bool isMarkedByNate;
    public GavinPassiveHitbox gavHb;
    public virtual void Update()
    {
        AI();
        if (canvas1 == null)
        {
            canvas1 = player.canvasPlayer;
        }
    }
    public virtual bool takeDamage(int damage)
    {
        int newDamage = (int)(damage * (100.0f / (100.0f + armor)));
        health -= newDamage;
        GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageText"));
        dmgNum.transform.SetParent(canvas1.transform);
        dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;
        dmgNum.GetComponent<DamageNum>().damageText = newDamage.ToString();
        healthbar.takeDamage(health, maxHealth);
        if (health <= 0)
        {
            Destroy(healthbar.gameObject);
            if (gavHb != null)
            {
                gavHb.creepDied(this);
            }
            gameManager.delayGold(gold, this.transform, this);
            player.changeGold(gold);

            return true;
        } else
        {
            return false;
        }
    }

    protected IEnumerator MagicDelay(int dmg)
    {
        yield return new WaitForSeconds(0.15f);
        GameObject dmgNum = GameObject.Instantiate((GameObject)Resources.Load("DamageTextMagic"));
        dmgNum.transform.SetParent(canvas1.transform);
        dmgNum.GetComponent<DamageNum>().objectToFollow = this.gameObject.transform;    
        dmgNum.GetComponent<DamageNum>().damageText = dmg.ToString();
    }
    public virtual bool takeMagicDamage(int damage)
    {
        health -= (int)(damage * (100.0f / (100.0f + magicResist)));
        StartCoroutine(MagicDelay((int)(damage * (100.0f / (100.0f + magicResist))))); 
        healthbar.takeDamage(health, maxHealth);
        if (health <= 0)
        {
            Destroy(healthbar.gameObject);
            if (gavHb != null)
            {
                gavHb.creepDied(this);
            }
            gameManager.delayGold(gold, this.transform, this);
            player.changeGold(gold);
            //transform.position = movePos;

            return true;
        }
        else
        {
            return false;
        }
    }
    public void canMoveAgain()
    {
        anim.enabled = true;
        canMove = true;
    }
    #region Knock-Up
    public virtual void knockUp(float knockTime)
    {
        canMove = false;
        canAttack = false;
        Vector3 upLocation = transform.position;
        StartCoroutine(MoveOverSeconds(gameObject, transform.position + new Vector3(0, 2, 0), knockTime));
    }
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds / 2)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds) * 2);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
        elapsedTime = 0;
        while (elapsedTime < seconds / 2)
        {
            objectToMove.transform.position = Vector3.Lerp(transform.position, startingPos, (elapsedTime / seconds) * 2);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canAttack = true;
        canMove = true;
    }
    IEnumerator takeHook(GameObject hook)
    {
        while (isQdNic)
        {
            transform.position = hook.transform.position;
            yield return new WaitForFixedUpdate();
        }
    }
    public void takeRoot(float duration)
    {
        canMove = false;
        anim.enabled = false;
        Invoke("canMoveAgain", duration);
    }
    public virtual void takeQ(GameObject hook)
    {
        isQdNic = true;
        StartCoroutine(takeHook(hook));
    }
    #endregion
    #region Knockback
    public virtual void takeKnockback(Vector3 PlaceToGo, float speed, float duration)
    {
        canMove = false;
        canAttack = false;
        StartCoroutine(KnockBack(PlaceToGo, speed, duration));
    }
    public void shredResist(int amt, float time)
    {
        StartCoroutine(shredArmor(amt, time));
        StartCoroutine(shredMR(amt, time));
    }
    public IEnumerator shredArmor(int amt, float time)
    {
        if (!armorShredded)
        {
            armorShredded = true;
            armor -= amt;
            yield return new WaitForSeconds(time);
            armor += amt;
            armorShredded = false;
        }
    }
    public IEnumerator shredMR(int amt, float time)
    {
        if (!mrShredded)
        {
            mrShredded = true;
            magicResist -= amt;
            yield return new WaitForSeconds(time);
            magicResist += amt;
            mrShredded = false;
        }
    }
    public IEnumerator KnockBack(Vector3 PlaceToGo, float speed, float duration)
    {
        float startTime = Time.time;
        Vector3 center = (transform.position + PlaceToGo) * 0.5f;
        center.y += -2f;
        Vector3 startArea = transform.position - center;
        Vector3 endArea = PlaceToGo - center;
        float suck = 0;
        while (suck < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.position = Vector3.Slerp(startArea, endArea, t);
            transform.position += center;
            suck += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canAttack = true;
        canMove = true;
    }
    #endregion
    public virtual void takeStun(float duration)
    {
        isStunned = true;
        canMove = false;
    }
    public IEnumerator stun(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
        canMove = true;
        isQdNic = false;
    }
    public void slow(float percent, float duration)
    {
        trailSpawn = GameObject.Instantiate((GameObject)Resources.Load("TrailSpawner"));
        trailSpawn.transform.parent = this.transform;
        trailSpawn.transform.localPosition = transform.position + trailOffset;
        trailSpawn.GetComponent<DeathTimer>().timeAlive = duration;
        float originalSpeed = Speed;
        Speed *= (percent * 0.01f);
        isSlowed = true;
        StartCoroutine(takeSlow(originalSpeed, duration));
    }
    public IEnumerator takeSlow(float originalSpeed, float duration)
    {
        yield return new WaitForSeconds(duration);
        isSlowed = false;
        Speed = originalSpeed;
    }
    public virtual void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        health = maxHealth;
        randDiffx = Random.Range(-2, 2);
        randDiffz = Random.Range(-2, 2);
        randDiff = new Vector3(randDiffx, 0, randDiffz);
    }
    public void takeDamageOverTime(int damage, bool isMagic, bool isDio)
    {
        if (takingDamage)
        {
            StartCoroutine(takeDoT(damage, isMagic, isDio));
        }
    }
    public IEnumerator takeDoT(int damage, bool isMagic, bool isDio)
    {
        if (takingDamage)
        {
            if (isMagic)
            {
                takeMagicDamage(damage);
            }else
            {
                takeDamage(damage);
            }
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(takeDoT(damage, isMagic, isDio));
        }
    }

    public virtual void AI()
    {
        if (monica == null)
        {
            monicaInAutoRadius = false;
            monicaInRadius = false;
        }
        if (canMove)
        {
            if (monicaInRadius)
            {
                if (!monicaInAutoRadius)
                {
                    if (attackDelay)
                    {
                        StartCoroutine(AttackDelay());
                    }
                    else
                    {
                        NewPosition = monica.gameObject.transform.position + offset;
                        isMoving = true;
                        Vector3 lookPos = NewPosition - transform.position;
                        lookPos.y = 0;
                        transform.position = Vector3.MoveTowards(transform.position, NewPosition + randDiff, Speed * Time.deltaTime);
                        Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                    }
                }
                else
                {
                    //auto attack
                    if (canAttack && !isAttacking)
                    {
                        anim.SetTrigger("Attack");
                        isAttacking = true;
                        canAttack = false;
                        attackDelay = true;
                        attackingMonica = true;
                    }
                    NewPosition = monica.gameObject.transform.position;
                    Vector3 lookPos = NewPosition - transform.position;
                    lookPos.y = 0;
                    Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                }
            }
           
            if (!playerInRadius && !monicaInRadius)
            {
                isAttacking = false;
                canAttack = true;
                NewPosition = Nexus.transform.position;
                if (Vector3.Distance(NewPosition, transform.position) > NexusAttackRange)
                {
                    isMoving = true;
                    Vector3 lookPos = NewPosition - transform.position;
                    lookPos.y = 0;
                    transform.position = Vector3.MoveTowards(transform.position, NewPosition + randDiff, Speed * Time.deltaTime);
                    Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);

                }
                else
                {
                    NewPosition = transform.position;
                }
            }
            else if (!playerInAutoRadius && !monicaInRadius)
            {
                //canAttack = true;
                //isAttacking = false;
                //anim.SetTrigger("StopAttack");
                if (attackDelay)
                {
                    StartCoroutine(AttackDelay());
                }
                else
                {

                    NewPosition = player.gameObject.transform.position + offset;
                    isMoving = true;
                    Vector3 lookPos = NewPosition - transform.position;
                    lookPos.y = 0;
                    transform.position = Vector3.MoveTowards(transform.position, NewPosition + randDiff, Speed * Time.deltaTime);
                    Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
                }
            }
            else if(!monicaInRadius)
            {
                //auto attack
                if (canAttack && !isAttacking)
                {
                    anim.SetTrigger("Attack");
                    isAttacking = true;
                    canAttack = false;
                    attackDelay = true;
                    attackingMonica = false;

                }
                NewPosition = player.gameObject.transform.position;
                Vector3 lookPos = NewPosition - transform.position;
                lookPos.y = 0;
                Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
                this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
            }
        }
    }
    public void endAttack()
    {
        canAttack = true;
        isAttacking = false;
        if (!isRanged)
        {
            isAttacking = false;
            if (attackingMonica)
            {
                monica.takeDamage(this.damage);
            }
            else
            {
                player.takeDamage(this.damage, false, false);
            }
        }

    }
    public virtual IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.5f);
        attackDelay = false;
    }
}

