using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltDoT : MonoBehaviour
{
    public Quaternion rotation;
    public int Damage;
    void Start()
    {
        rotation = transform.rotation;
    }
    void Update()
    {
        transform.rotation = rotation;
    }
    void OnTriggerEnter(Collider collider)
    {
        Creep creep = collider.GetComponent<Creep>();
        if (creep != null)
        {
            creep.takingDamage = true;
            creep.takeDamageOverTime(Damage);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        Creep creep = collider.GetComponent<Creep>();
        if (creep != null)
        {
            creep.takingDamage = false;
        }
    }
}