using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Wall")
        {
            Destroy(collider.transform.parent.gameObject);
        }
    }
}
