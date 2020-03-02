using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GavinPassiveHitbox : MonoBehaviour
{
    public List<Creep> creeps = new List<Creep>();

    public void OnTriggerEnter(Collider collider)
    {
        Creep newCreep = collider.GetComponent<Creep>();
        if (newCreep != null && !creeps.Contains(newCreep))
        {
            creeps.Add(newCreep);
            newCreep.gavHb = this;
        }
    }
    public void OnTriggerExit(Collider collider)
    {
        Creep newCreep = collider.GetComponent<Creep>();
        if (newCreep != null && creeps.Contains(newCreep))
        {
            creeps.Remove(newCreep);
        }
    }
    public void creepDied(Creep creep)
    {
        int index = creeps.FindIndex(x => x == creep);
        creeps.RemoveAt(index);
    }
}
