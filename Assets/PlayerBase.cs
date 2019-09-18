using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    public AutoRangeChecker achecker;
    protected Vector3 NewPosition;
    public float speed;
    public float walkRange = 0.012f;
    public int health;
    public int maxHealth;
    public Vector3 offsetY = new Vector3(0, 0.09f, 0);
    public GameObject graphics;
    public Animator Anim;
    public Transform center;
    public GameObject QIndicator;
    public GameObject WIndicator;
    public GameObject EIndicator;
    public GameObject RIndicator;
    protected int QDamage;
    protected int WDamage;
    protected int EDamage;
    protected int RDamage;
    public List<int> QDamageScale = new List<int>();
    public List<int> WDamageScale = new List<int>();
    public List<int> EDamageScale = new List<int>();
    public List<int> RDamageScale = new List<int>();
    protected bool QPressed;
    protected bool WPressed;
    protected bool EPressed;
    protected bool RPressed;
    public bool isMoving = false;
    protected bool Ab1;
    protected bool Ab2;
    protected bool Ab3;
    protected bool Ab4;
    protected bool isUpgrading = false;
    protected bool stoppingAttack;
    protected bool endingAttack;
    public bool isAttacking = false;
    public bool canAttack = true;
    protected Quaternion QuindRot;
    protected Quaternion WindRot;
    protected Quaternion RindRot;
    public bool canMove = true;
    public int abilityLevelNum;
    public int level = 0;
    public int AbilityPower;
    public int AttackDamage;
    public int Armor;
    public int MagicResist;
    public int Mana;
    public float AttackSpeed;
    public float CDReduction;
    public int healthPerLevel;
    public float CDQ;
    public float CDW;
    public float CDE;
    public float CDR;
    public int ADPerLevel;
    public float ASPerLevel;
    public Creep creepSelected;
    public bool isWalkingToTarget = false;
    protected int autoNum = 1;
    public int layerMask = 512;
    public bool CooldownQ = true;
    public bool CooldownW = true;
    public bool CooldownE = true;
    public bool CooldownR = true;
    public AbilityIndicator IndQ;
    public AbilityIndicator IndW;
    public AbilityIndicator IndE;
    public AbilityIndicator IndR;
    public Text levelNum;
    public Text QDescription;
    public Text WDescription;
    public Text EDescription;
    public Text RDescription;
    protected string Qdesc;
    protected string Wdesc;
    protected string Edesc;
    protected string Rdesc;
    public bool isInvisible;
    protected bool anythingWorks = true;
    public float groundOffset;
    public Image healthbar;
    public Image shieldbar;
    public Text healthdiv;
    protected LayerMask groundOnly;
    protected LayerMask creepsOnly;
    public bool canAttackAfterAuto = true;
    public Vector3 bufferedPosition;
    protected bool isBuffering;
    public List<Item> items = new List<Item>();
    public int gold = 0;
    public Text goldAmt;
    public GameManager gameManager;
    int itemAD;
    int itemAP;
    int itemHealth;
    float itemAtkSpeed;
    public int hpPerSec;
    List<Item> itemsToDestroy = new List<Item>();
    public List<bool> itemsHad = new List<bool>();
    public bool canBubble;
    public int numFelix;
    public bool hexagonAttack;
    public int numGorilla;
    protected bool hasShield;
    int currentShield;
    void Start()
    {
        NewPosition = transform.position;
        Anim = gameObject.GetComponent<Animator>();
        abilityLevelNum = 1;
        creepsOnly = LayerMask.GetMask("Creep");
        groundOnly = LayerMask.GetMask("Ground");
        level = 1;
        LevelUp();
        updateAbilDamage();
        abilityDescription();
        center = this.transform;
        StartCoroutine(regen());
        health = maxHealth;
        takeDamage(0);
        addList();
    }
    void updateAbilDamage()
    {
        QDamage = QDamageScale[0];
        WDamage = WDamageScale[0];
        EDamage = EDamageScale[0];
        RDamage = RDamageScale[0];
    }
    protected virtual void Update()
    {
        upgradeTick();
        AbilityUpdate();
        TargetUpdate();
        ClickUpdate();
        passiveUpdate();
        UpdateAtkSpeed();
        itemUpdate();
    }
    protected virtual void UpdateAtkSpeed()
    {
        Anim.SetFloat("AtkSpeed", AttackSpeed);
    }
    public virtual void EndAttack()
    {
        canAttackAfterAuto = true;
        canMove = true;
        canAttack = true;
        isAttacking = false;
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isAttacking", false);
    }
    protected void addShield(int shieldAmt, float duration)
    {
        hasShield = true;
        currentShield = shieldAmt;
        takeDamage(1);
        StartCoroutine(shieldDelay(duration));
    }
    IEnumerator shieldDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        hasShield = false;
        currentShield = 0;
        takeDamage(0);
    }
    void addList()
    {
        for (int i = 0; i < 11; i++)
        {
            itemsHad.Add(new bool());
        }
    }
    void itemUpdate()
    {

    }
    void itemChange()
    {
        if (items.Find(x => (x.name == "AscendedBrain(Clone)")))
        {
            itemsHad[0] = true;
        }
        else
        {
            itemsHad[0] = false;
        }
        if (items.Find(x => (x.name == "CeasarsBandana(Clone)")))
        {
            itemsHad[1] = true;
            StartCoroutine(CeasarUpdate());
        } else
        {
            itemsHad[1] = false;
        }
        if (items.Find(x => (x.name == "DiamondSword(Clone)")))
        {
            itemsHad[2] = true;
        }
        else
        {
            itemsHad[2] = false;
        }
        if (items.Find(x => (x.name == "FelixsTrident(Clone)")))
        {
            itemsHad[3] = true;
        }
        else
        {
            itemsHad[3] = false;
        }
        if (items.Find(x => (x.name == "Gfuel(Clone)")))
        {
            itemsHad[4] = true;
        }
        else
        {
            itemsHad[4] = false;
        }
        if (items.Find(x => (x.name == "GorillaAngryBlade(Clone)")))
        {
            itemsHad[5] = true;
        }
        else
        {
            itemsHad[5] = false;
        }
        if (items.Find(x => (x.name == "HexagonForce(Clone)")))
        {
            itemsHad[6] = true;
        }
        else
        {
            itemsHad[6] = false;
        }
        if (items.Find(x => (x.name == "LentPencil(Clone)")))
        {
            itemsHad[7] = true;
        }
        else
        {
            itemsHad[7] = false;
        }
        if (items.Find(x => (x.name == "Minigun(Clone)")))
        {
            itemsHad[8] = true;
        }
        else
        {
            itemsHad[8] = false;
        }
        if (items.Find(x => (x.name == "PizzaAtPool(Clone)")))
        {
            itemsHad[9] = true;
        }
        else
        {
            itemsHad[9] = false;
        }
        if (items.Find(x => (x.name == "StoneMask(Clone)")))
        {
            itemsHad[10] = true;
        }
        else
        {
            itemsHad[10] = false;
        }
    }
    public virtual void EndAutoAttack()
    {
        canAttackAfterAuto = true;
        
        if (creepSelected != null && achecker.EnemiesInRadius.Contains(creepSelected) && !isBuffering) {
            isAttacking = false;
            AttackCreep(creepSelected.transform);
        }
        else
        {
            autoNum = 1;
            canMove = true;
            canAttack = true;
            isAttacking = false;
            Anim.SetBool("isIdle", true);
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isAttacking", false);
        }
    }
    public void forceStopMoving()
    {
        NewPosition = transform.position;
    }
    
    public virtual void stopMoving()
    {
        isMoving = false;
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isWalking", false);
    }

    public virtual void TargetUpdate()
    {
        if (achecker.EnemiesInRadius.Contains(creepSelected) && isWalkingToTarget && creepSelected != null)
        {
            AttackCreep(creepSelected.transform);
            isWalkingToTarget = false;
        }
    }
    public virtual void AbilityUpdate()
    {
        QTest();
        WTest();
        ETest();
        RTest();
        passiveUpdate();
    }
    public void LevelUp()
    {
        levelNum.text = level.ToString();
        isUpgrading = true;

        if (abilityLevelNum >= 1)
        {
            AttackDamage += ADPerLevel;
            AttackSpeed += ASPerLevel;
            UpdateAtkSpeed();
            IndQ.levelUp();
            IndW.levelUp();
            IndE.levelUp();
            abilityLevelNum -= 1;
            if ((level >= 6 && IndR.levelNum <= 0) || (level >= 11 && IndR.levelNum <= 1) || (level >= 16 && IndR.levelNum <= 2))
            {
                IndR.levelUp();
            }
            abilityDescription();
            maxHealth += healthPerLevel;
            takeDamage(-healthPerLevel);
        }
        else
        {
            isUpgrading = false;
            IndQ.endLevelUp();
            IndW.endLevelUp();
            IndE.endLevelUp();
            IndR.endLevelUp();
        }
    }
    IEnumerator regen()
    {
        yield return new WaitForSeconds(1);
        if (health + hpPerSec < maxHealth)
        {
            takeDamage(-hpPerSec);
        }
        StartCoroutine(regen());
    }
    public void buyItem(Item newItem)
    {
        string badProgramming;
        if (newItem.gold <= gold && items.Count < 6)
        {
            foreach (Item purchasedItem in items)
            {
                foreach (Item builtItem in newItem.build)
                {
                    badProgramming = builtItem.gameObject.name + "(Clone)";
                    if (string.Equals(badProgramming, purchasedItem.gameObject.name))
                    {
                        itemsToDestroy.Add(purchasedItem);
                    }
                }
            }
            foreach (Item destroyed in itemsToDestroy)
            {
                gameManager.MurderObject(destroyed.gameObject, 0.1f);
                items.Remove(destroyed);               
            }
            itemsToDestroy.RemoveRange(0, itemsToDestroy.Count);
            changeGold(-newItem.gold);
            GameObject newItemAdd = GameObject.Instantiate((GameObject)Resources.Load(newItem.name));
            items.Add(newItemAdd.GetComponent<Item>());
            newItemAdd.transform.parent = gameManager.imageItems[items.Count - 1].transform;
            newItemAdd.transform.localPosition = Vector3.zero;
            newItemAdd.transform.localScale = new Vector3(0.16f, 0.293f, 0.36f);
            updateItemStats();
        }
    }
    IEnumerator CeasarUpdate()
    {
        yield return new WaitForSeconds(10);
        if (itemsHad[1] == true)
        {
            canBubble = true;
            StartCoroutine(CeasarUpdate());
        }
    }
    void updateItemStats()
    {
        itemAD = 0;
        itemAP = 0;
        itemAtkSpeed = 0;
        itemHealth = 0;
        Armor = 0;
        MagicResist = 0;
        foreach (Item statItem in items)
        {
            itemAD += statItem.attackDamage;
            itemAP += statItem.abilityPower;
            itemAtkSpeed += statItem.attackSpeed * 0.01f;
            itemHealth += statItem.health;
            MagicResist += statItem.magicResist;
            Armor = statItem.armor;
        }
        AttackDamage += itemAD;
        AbilityPower += itemAP;
        AttackSpeed += itemAtkSpeed;
        maxHealth += itemHealth;
        if (itemsHad[0])
        {
            AbilityPower = (int)(AbilityPower * 1.3f);
        }
        if (itemsHad[4])
        {
            maxHealth += (int)(AttackDamage * 3f);
        }
        health += itemHealth;
        itemChange();
        abilityDescription();
    }
    public virtual void abilityDescription() { }
    public void updateDamage(int abNum, int ablvl)
    {
        ablvl -= 1;
        switch (abNum)
        {
            case 1:
                QDamage = QDamageScale[ablvl];
                abilityDescription();
                IndQ.updateAbilityDescription(Qdesc);
                uniqueQLevelUp(ablvl);
                break;
            case 2:
                WDamage = WDamageScale[ablvl];
                abilityDescription();
                IndW.updateAbilityDescription(Wdesc);
                uniqueWLevelUp(ablvl);
                break;
            case 3:
                EDamage = EDamageScale[ablvl];
                abilityDescription();
                IndE.updateAbilityDescription(Edesc);
                uniqueELevelUp(ablvl);
                break;
            case 4:
                RDamage = RDamageScale[ablvl];
                abilityDescription();
                IndR.updateAbilityDescription(Rdesc);
                uniqueRLevelUp(ablvl);
                break;
        }
    }


    public virtual void passiveUpdate() { }
    public virtual void uniqueQLevelUp(int level) { }
    public virtual void uniqueWLevelUp(int level) { }
    public virtual void uniqueELevelUp(int level) { }
    public virtual void uniqueRLevelUp(int level) { }


    public virtual void ClickUpdate()
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
                    NewPosition = hit.point;
                    NewPosition.y = 0.5f;
                    EndAttack();
                }
            }
            if (Physics.Raycast(raymond, out hit, creepsOnly) && hit.transform.tag == "Creep")
            {
                Creep touchedCreep = hit.transform.gameObject.GetComponent<Creep>();
                creepSelected = touchedCreep;
                if (achecker.EnemiesInRadius.Contains(touchedCreep) && canAttack)
                {
                    NewPosition = transform.position;
                    stopMoving();
                    transform.LookAt(touchedCreep.transform.position);
                    transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    Anim.SetTrigger("Attack");
                    Anim.SetBool("isAttacking", true);
                    Anim.SetBool("isIdle", false);

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
    public virtual void QTest()
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
    public virtual void WTest()
    {
        if (CooldownW && IndW.levelNum > 0)
        {
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
    public virtual void ETest()
    {
        if (CooldownE && IndE.levelNum > 0)
        {
            stoppingAttack = Input.GetKeyUp(KeyCode.E);
            Ab3 = Input.GetKey(KeyCode.E);
            bool leftClick = Input.GetMouseButton(0);
            bool touch = Input.GetKeyDown(KeyCode.E);
            if (Ab3 && !stoppingAttack && anythingWorks)
            {
                EIndicator.SetActive(true);
                EPressed = true;
            }
            else if ((EPressed || (leftClick && Ab3)) && anythingWorks)
            {
                EIndicator.SetActive(false);
                EPressed = false;
                isInvisible = false;
                activateE();
            }
            else
            {
                EIndicator.SetActive(false);
                EPressed = false;
            }

            if (Ab3 && endingAttack)
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
    public virtual void RTest()
    {
        if (CooldownR && IndR.levelNum > 0)
        {
            stoppingAttack = Input.GetKeyUp(KeyCode.R);
            bool leftClick = Input.GetMouseButton(0);
            bool touch = Input.GetKeyDown(KeyCode.R);
            if (Ab4 && !stoppingAttack && anythingWorks)
            {
                RIndicator.SetActive(true);
                RPressed = true;
            }
            else if (RPressed || (leftClick && Ab4))
            {
                RIndicator.SetActive(false);
                RPressed = false;
                isInvisible = false;
                activateR();
            }
            else
            {
                RIndicator.SetActive(false);
                RPressed = false;
            }
            if (Ab4 && endingAttack)
            {
            }
            if (touch)
            {
                anythingWorks = true;
                endingAttack = false;
            }
        }
    }
    void upgradeTick()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl);
        Ab1 = Input.GetKey(KeyCode.Q);
        Ab2 = Input.GetKey(KeyCode.W);
        Ab3 = Input.GetKey(KeyCode.E);        
        Ab4 = Input.GetKey(KeyCode.R);
        if (Ab1 && ctrl && isUpgrading)
        {
            IndQ.chooseLevelUp(1);
        }  if (Ab2 && ctrl && isUpgrading)
        {
            IndW.chooseLevelUp(2);
        }  if (Ab3 && ctrl && isUpgrading)
        {
            IndE.chooseLevelUp(3);
        } if (Ab4 && ctrl && isUpgrading)
        {
            IndR.chooseLevelUp(4);
        }
        IndQ.updateCooldownNum(CDQ);
        IndW.updateCooldownNum(CDW);
        IndE.updateCooldownNum(CDE);
        IndR.updateCooldownNum(CDR);
    }
    public void walkTowardsTarget(Creep touchedCreep)
    {
        NewPosition = touchedCreep.transform.position;
        isWalkingToTarget = true;
    }
    public virtual void AttackCreep(Transform target)
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
            Anim.SetTrigger("Attack");
        }
    }
    protected IEnumerator hexagon()
    {
        hexagonAttack = true;
        yield return new WaitForSeconds(3);
        hexagonAttack = false;
    }
    protected IEnumerator gorilla()
    {
        numGorilla += 1;
        AttackSpeed += 0.04f;
        UpdateAtkSpeed();
        yield return new WaitForSeconds(4);
        AttackSpeed -= 0.04f;
        numGorilla -= 1;
    }
    public virtual void activateQ()
    {
        if (canAttack)
        {
            CooldownQ = false;
            canMove = false;
            IndQ.StartCooldown(CDQ, this, 1);
            forceStopMoving();
            QuindRot = QIndicator.transform.rotation;
            graphics.transform.rotation = QIndicator.transform.rotation;
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

    public virtual void activateW()
    {
        if (canAttack)
        {      
            CooldownW = false;
            canMove = false;
            IndW.StartCooldown(CDW, this, 2);
            WindRot = WIndicator.transform.rotation;
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
    }

    public virtual void activateE()
    {
        if (canAttack)
        {
            CooldownE = false;
            canMove = false;
            IndE.StartCooldown(CDE, this, 3);
            forceStopMoving();
            graphics.transform.rotation = EIndicator.transform.rotation;
            Anim.SetTrigger("E");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
        }
    }
    public virtual void activateR()
    {
        if (canAttack)
        {
            CooldownR = false;
            canMove = false;
            RindRot = RIndicator.transform.rotation;
            graphics.transform.rotation = RIndicator.transform.rotation;
            IndR.StartCooldown(CDR, this, 4);
            forceStopMoving();
            Anim.SetTrigger("R");
            Anim.SetBool("isAttacking", true);
            Anim.SetBool("isIdle", false);
            canAttack = false;
            if (itemsHad[6])
            {
                StartCoroutine(hexagon());
            }
        }
    }
    public void changeGold(int addedGold)
    {
        gold += addedGold;
        goldAmt.text = gold.ToString() + "G";
    }
    public virtual void Q()
    {
        //spawn, move, etc
    }
    public virtual void W()
    {
        //spawn, move, etc
    }
    public virtual void E()
    {
        //spawn, move, etc
    }
    public virtual void R()
    {
        //spawn, move, etc
    }
    public virtual void hitCreepWithAuto()
    {
        if (!isMoving)
        {
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
            canAttackAfterAuto = true;
            if (hexagonAttack)
            {
                creepSelected.takeDamage(AttackDamage + 100);
                hexagonAttack = false;
            }else
            {
                creepSelected.takeDamage(AttackDamage);
            }
        }
    }
    public virtual void stopAbilityIndication()
    {
        endingAttack = true;
        Ab1 = false;
        Ab2 = false;
        Ab3 = false;
        Ab4 = false;
        QPressed = false;
        WPressed = false;
        EPressed = false;
        RPressed = false;
        anythingWorks = false;
    }
    protected virtual IEnumerator waitToMove()
    {
        while (!canAttackAfterAuto || isMoving)
        {
            yield return new WaitForEndOfFrame();
        }
        isBuffering = false;
        canAttackAfterAuto = true;
        NewPosition = bufferedPosition;
        creepSelected = null;
    }
    public virtual void takeDamage(int damage)
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
                takeDamage(0);
            }
        }
        else
        {
            health -= damage;
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
