using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRadius : MonoBehaviour {
    public bool InCreep;
    public GameObject creepOn;
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Creep")
        {
            InCreep = true;
            creepOn = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Creep")
        {
            InCreep = false;
        }
    }
}
