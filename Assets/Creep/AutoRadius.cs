using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRadius : MonoBehaviour
{
    public Creep parent;
    void Start()
    {
        parent = transform.parent.GetComponent<Creep>();
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == parent.player.gameObject)
        {
            parent.playerInAutoRadius = true;
        }
        else if (collider.gameObject.name == "MonicaBart(Clone)")
        {
            parent.monicaInAutoRadius = true;
            parent.monica = collider.GetComponent<MonicaController>();
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == parent.player.gameObject)
        {
            parent.playerInAutoRadius = false;
        }
        else if (collider.gameObject.name == "MonicaBart(Clone)")
        {
            parent.monicaInAutoRadius = false;
        }
    }
}
