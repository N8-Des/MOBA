using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour {
    public NicPlayer nic;
    public int monicaDamage;
    public int monicaHealth;
    Vector3 offset = new Vector3(0, -1, 0);
    public GameManager gm;
    public void SpawnMonica()
    {
        GameObject monica = GameObject.Instantiate((GameObject)Resources.Load("MonicaBart"));
        monica.transform.position = transform.position + offset;
        MonicaController newMonica = monica.GetComponent<MonicaController>();
        nic.monica = newMonica;
        nic.monicaAlive = true;
        newMonica.autoDamage = monicaDamage;
        newMonica.maxHealth = monicaHealth;
        newMonica.gameManager = gm;
    }
}
