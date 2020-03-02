using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepProjectile : MonoBehaviour {
    public GameObject playerTarget;
    public float speed;
    public int damage;
    public bool magicDamage = true;
    Vector3 offset = new Vector3(0, 2, 0);
    public void Update()
    {        
        Vector3 NewPosition = playerTarget.transform.position + offset;
        if (Vector3.Distance(NewPosition, transform.position) > 0.7f)
        {
            Vector3 lookPos = NewPosition - transform.position;
            lookPos.y = 0;
            transform.position = Vector3.MoveTowards(transform.position, NewPosition, speed * Time.deltaTime);
            Quaternion transRot = Quaternion.LookRotation(lookPos, Vector3.up);
            this.transform.rotation = Quaternion.Slerp(transRot, this.transform.rotation, 0.2f);
        }
        else
        {
            playerTarget.GetComponent<PlayerBase>().takeDamage(damage, magicDamage, false);
            Destroy(gameObject);
        }
    }
}
