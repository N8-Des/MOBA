using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPedestal : MonoBehaviour
{
    public GameManager gm;
    public Room room;
    
    void OnTriggerEnter(Collider collider)
    {
        PlayerBase player = collider.GetComponent<PlayerBase>();
        if (player != null && gm.creeps.Count <= 0)
        {
            player.SpawnBossUI();
        }
    }
}
