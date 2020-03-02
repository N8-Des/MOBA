using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltDoT : MonoBehaviour
{
    public Quaternion rotation;
    public int Damage;
    public bool magicDamage = true;
    public bool isDio;
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
            creep.takeDamageOverTime(Damage, magicDamage, isDio);
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