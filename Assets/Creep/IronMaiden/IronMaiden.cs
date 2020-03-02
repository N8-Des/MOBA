using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronMaiden : Creep {

    bool isOpen;
    int numObjects = 14;
    public GameObject projectile;
    bool isShooting;
    Vector3 projectileOffset = new Vector3(0, 3, 0);
    Vector3 healthOffset = new Vector3(0, 6, 0);
    int numOn = 0;
    float shootSpeed = 5.3f;
    bool closed;
    public override void knockUp(float knockTime) { }
    public override void takeKnockback(Vector3 PlaceToGo, float speed, float duration) { }
    public override void takeQ(GameObject hook) { }
    public override void takeStun(float duration) { }
    public override void AI()
    {
        if (playerInRadius)
        {
            if (!isAttacking && canMove && !closed)
            {
                open();
            }
        } else if(canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 2 * Time.deltaTime);
            anim.SetBool("IsWalking", true);
        }
    }
    public override bool takeDamage(int damage)
    {
        if (health <= 200 && !closed)
        {
            closed = true;
            canMove = false;
            close();
        }
        return base.takeDamage(damage);
    }
    public override bool takeMagicDamage(int damage)
    {
        if (health <= 200 && !closed)
        {
            closed = true;
            canMove = false;
            close();
        }
        return base.takeMagicDamage(damage);
    }
    public override void Start()
    {
        base.Start();
        healthbar.localOffset = healthOffset;
    }
    void close()
    {
        closed = true;
        anim.SetBool("OpenIdle", false);
        anim.SetBool("IsWalking", false);
        anim.SetTrigger("close");
        isOpen = false;
        armor = 250;
        magicResist = 250;
        StartCoroutine(healToFull());
    }
    IEnumerator healToFull()
    {
        yield return new WaitForSeconds(18);
        health = maxHealth;
        takeDamage(0);
        open();
        closed = false;
        canMove = true;
    }
    void open()
    {
        if (!isOpen)
        {
            anim.SetTrigger("open");
            anim.SetBool("IsWalking", false);
            anim.SetBool("CloseIdle", false);
            anim.SetBool("OpenIdle", false);
            isOpen = true;
            armor = 25;
            magicResist = 25;
            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
            isShooting = true;
        }        
    }
    IEnumerator shoot()
    {
        yield return new WaitForSeconds(shootSpeed);
        if (isOpen && canAttack)
        {
            StartCoroutine(fire());
            StartCoroutine(shoot());
        } else
        {
            isShooting = false;
            shootSpeed = 6;
        }
    }
    public void setBool(int swap)
    {
        if (swap == 0)
        {
            anim.SetBool("CloseIdle", true);
            anim.SetBool("OpenIdle", false);
        }
        else
        {
            anim.SetBool("CloseIdle", false);
            anim.SetBool("OpenIdle", true);
        }
    }
    IEnumerator fire()
    {
        Vector3 center = transform.position;
        float indAng = 360 / numObjects;
        for (int amt = 0; amt < 3; amt++)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < numObjects; i++)
            {
                Vector3 pos = RandomCircle(center + projectileOffset, 1.2f, indAng);
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
                Vector3 newRotation = new Vector3(0, rot.eulerAngles.y, 0);
                rot.eulerAngles = (newRotation);
                Instantiate(projectile, pos, rot);
                numOn++;
            }
        }
        numOn = 0;
    }
    Vector3 RandomCircle(Vector3 center, float radius, float induvidualAngle)
    {
        float ang = induvidualAngle * numOn;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        return pos;
    }
}
