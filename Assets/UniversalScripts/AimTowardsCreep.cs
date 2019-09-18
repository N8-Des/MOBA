using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTowardsCreep : MonoBehaviour {
    public PlayerBase player;

    void OnEnable()
    {
        transform.LookAt(player.creepSelected.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
