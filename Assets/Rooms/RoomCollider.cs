using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    public Room parRoom;
    void Start()
    {
        parRoom = gameObject.GetComponentInParent(typeof(Room)) as Room;
    }
    void OnTriggerEnter(Collider collider)
    {


        PlayerBase player = collider.GetComponent<PlayerBase>();
        if (player != null)
        {
            parRoom.ReactToPlayer();
        }
    }
}
