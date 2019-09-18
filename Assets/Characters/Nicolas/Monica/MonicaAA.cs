using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonicaAA : MonoBehaviour {
    public Creep creepTarget;
    public int damage;
	void Update () {
        if (creepTarget == null)
        {
            Destroy(gameObject);
        }
        if (Mathf.Abs(Vector3.Distance(creepTarget.transform.position, transform.position)) >= 0.2f)
        {
            transform.position = Vector3.MoveTowards(creepTarget.transform.position, transform.position, Time.deltaTime * 4);
        }else
        {
            creepTarget.takeMagicDamage(damage);
            Destroy(gameObject);
        }
	}
}
