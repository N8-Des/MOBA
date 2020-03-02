using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepWarn : MonoBehaviour
{
    public GameManager gm;
    public PlayerBase player;
    public GameObject creepSpawn;
    void Start()
    {
        Invoke("spawnCreep", 2.5f);
    }

    void spawnCreep()
    {
        GameObject newCreep = Instantiate(creepSpawn, transform.position, Quaternion.identity);
        Creep creep = newCreep.GetComponent<Creep>();
        gm.creeps.Add(creep);
        creep.gameManager = gm;
        creep.player = player;
        GameObject healthbar = Instantiate(gm.basicHealthbar);
        healthbar.transform.position = creep.transform.position;
        healthbar.GetComponent<AnchorUI>().objectToFollow = creep.transform;
        creep.healthbar = healthbar.GetComponent<AnchorUI>();
        healthbar.gameObject.transform.SetParent(gm.acplayer.canvasPlayer.transform);
        Destroy(gameObject);
    }
}
