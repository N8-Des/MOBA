using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonicaAggro : MonoBehaviour {

    public MonicaController monica;

    void OnTriggerEnter(Collider collider)
    {
        Creep creepTarget = collider.GetComponent<Creep>();
        if (creepTarget != null)
        {
            monica.addCreep(creepTarget);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        Creep creepTarget = collider.GetComponent<Creep>();
        if (creepTarget != null)
        {
            monica.removeCreep(creepTarget);
        }
    }
}
