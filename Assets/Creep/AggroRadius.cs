using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroRadius : MonoBehaviour {
    public Creep parent;
	void Start ()
    {
        parent = transform.parent.GetComponent<Creep>();
	}
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == parent.player.gameObject)
        {
            parent.playerInRadius = true;
        } else if (collider.gameObject.name == "MonicaBart(Clone)")
        {
            parent.monicaInRadius = true;
            parent.monica = collider.GetComponent<MonicaController>();
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == parent.player.gameObject || collider.gameObject.name == "MonicaBart(clone)")
        {
            parent.playerInRadius = false;
        } else if (collider.gameObject.name == "MonicaBart(Clone)")
        {
            parent.monicaInRadius = false;
        }
    }
}
