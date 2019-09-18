using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonicaAuto : MonoBehaviour
{

    public MonicaController monica;

    void OnTriggerEnter(Collider collider)
    {
        Creep creepTarget = collider.GetComponent<Creep>();
        if (creepTarget != null)
        {
            monica.addCreepAtk(creepTarget);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        Creep creepTarget = collider.GetComponent<Creep>();
        if (creepTarget != null)
        {
            monica.removeCreepAtk(creepTarget);
        }
    }
}
