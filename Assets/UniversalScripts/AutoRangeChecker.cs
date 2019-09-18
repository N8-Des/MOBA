using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRangeChecker : MonoBehaviour {
    public List<Creep> EnemiesInRadius = new List<Creep>();

    public void OnTriggerStay(Collider collider)
    {
        Creep creepHit = collider.GetComponent<Creep>();
        if (creepHit != null)
        {
            if (EnemiesInRadius.Count == 0)
            {
                EnemiesInRadius.Add(creepHit);
            }
            else
            {
                if (EnemiesInRadius.Contains(creepHit))
                {
                    //don't add it to the list
                } else
                {
                    EnemiesInRadius.Add(creepHit);
                }
            }

        }
    }
    public void OnTriggerExit(Collider collider)
    {
        Creep creepHit = collider.GetComponent<Creep>();
        if (creepHit != null)
        {
            EnemiesInRadius.Remove(creepHit);
        }
    }

}
