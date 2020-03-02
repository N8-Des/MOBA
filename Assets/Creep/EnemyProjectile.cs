using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public bool isMagic;
    public int damage;
    public bool diesOnHit;
    public Vector3 targetPos;
    public float speed;
    void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) >= 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        PlayerBase player = collider.GetComponent<PlayerBase>();
        if (player != null)
        {
            player.takeDamage(damage, isMagic, false);
            if (diesOnHit)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
