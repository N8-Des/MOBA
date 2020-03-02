using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    //1 needs bottom
    //2 needs top
    //3 needs left
    //4 needs right
    private RoomTemplates templates;
    Room parRoom;
    int rand;
    bool spawned;
    public bool hasimp;
    int maxRooms = 16;
    void Start()
    {
        if (!hasimp)
        {
            templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
            parRoom = gameObject.GetComponentInParent<Room>();
            Invoke("Spawn", 0.05f);
        }   
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Spawnpoint" && !hasimp)
        {
            RoomSpawner conSpawner = collider.GetComponent<RoomSpawner>();
            if (parRoom != null)
            {
                parRoom.otherRooms.Add(conSpawner.transform.parent.gameObject);
            }
            if (conSpawner != null)
            {
                if (collider.GetComponent<RoomSpawner>().spawned == false && spawned == false)
                {
                    Instantiate(templates.wall, transform.position, Quaternion.identity);
                    parRoom.SpawnWall(openingDirection);
                    Destroy(gameObject);
                }
            }
            spawned = true;
        }
    }
    void Spawn() {
        if (!spawned && !hasimp)
        {
            if (templates.rooms.Count <= maxRooms)
            {
                if (openingDirection == 1)
                {
                    rand = Random.Range(0, 3);
                    Instantiate(templates.topRooms[rand], gameObject.transform.position, templates.topRooms[rand].transform.rotation);
                    spawned = true;
                }
                else if (openingDirection == 2)
                {
                    rand = Random.Range(0, 3);
                    Instantiate(templates.botRooms[rand], gameObject.transform.position, templates.botRooms[rand].transform.rotation);
                    spawned = true;
                }
                else if (openingDirection == 3)
                {
                    rand = Random.Range(0, 3);
                    Instantiate(templates.rightRooms[rand], gameObject.transform.position, templates.rightRooms[rand].transform.rotation);
                    spawned = true;
                }
                else if (openingDirection == 4)
                {
                    rand = Random.Range(0, 3);
                    Instantiate(templates.leftRooms[rand], gameObject.transform.position, templates.leftRooms[rand].transform.rotation);
                    spawned = true;
                }
            }
            else
            {
                Instantiate(templates.wall, transform.position, Quaternion.identity);
                //Destroy(gameObject);
                parRoom.SpawnWall(openingDirection);
                spawned = true;
            }
        }
    }
}
