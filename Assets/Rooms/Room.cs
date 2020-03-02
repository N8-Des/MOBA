using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameManager gm;
    public bool playerEntered;
    RoomSpawner[] spawnpts;
    public List<spawnPoints> spawnLocs = new List<spawnPoints>();
    [SerializeField]
    GameObject door;
    int amtEnemies;
    public bool isStart;
    public List<GameObject> otherRooms = new List<GameObject>();
    public bool isToggled = false;
    public Renderer[] rendos;
    public List<GameObject> creepsPoss = new List<GameObject>();
    bool roomCleared;
    [SerializeField]
    int minSpawn;
    [SerializeField]
    int maxSpawn;
    Vector3 spawn = Vector3.zero;
    Rect rect;
    GameObject wallSpawn;
    List<GameObject> doors = new List<GameObject>();
    public GameObject mapShow;
    public bool isBossRoom;
    GameObject wallMap;
    [SerializeField]
    GameObject MapVis;
    [System.Serializable]
    public class spawnPoints{
        public Vector3 spoint;
        public int positionNum;
    }
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        spawnpts = transform.GetComponentsInChildren<RoomSpawner>();
        rendos = transform.GetComponentsInChildren<Renderer>();
        creepsPoss = gm.diffSpawns[gm.stageOn].creeps;
        mapShow = Instantiate(MapVis);
        mapShow.transform.parent = gm.minimap.transform;
        mapShow.transform.localPosition = new Vector3(transform.position.x * 2.033f, transform.position.z * 2.033f, 0);
        wallSpawn = gm.minimap.GetComponent<Minimap>().Wall;
        foreach (RoomSpawner spawn in spawnpts)
        {
            spawnPoints sp = new spawnPoints();
            sp.spoint = spawn.transform.position;
            sp.positionNum = spawn.openingDirection;
            spawnLocs.Add(sp);
        }
    }
    public void setBossRoom()
    {
        GameObject warpButton = Instantiate(gm.minimap.GetComponent<Minimap>().BButton);
        warpButton.transform.SetParent(mapShow.transform);
        warpButton.transform.localPosition = Vector3.zero;
        warpButton.GetComponent<WarpButton>().gameManager = gm;
        warpButton.GetComponent<WarpButton>().roomAssociated = gameObject;

    }
    public void SpawnWall(int side)
    {
        wallMap = Instantiate(wallSpawn);
        wallMap.transform.parent = gm.minimap.transform;
        wallMap.transform.localPosition = new Vector3(transform.position.x * 2.033f, transform.position.z * 2.033f, 0);
        switch (side)
        {
            case 1:
                wallMap.transform.localEulerAngles = new Vector3(0, 0, 180);
                break;
            case 2:
                //nothing
                break;
            case 3:
                wallMap.transform.localEulerAngles = new Vector3(0, 0, -90);
                break;
            case 4:
                wallMap.transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
        }
    }
    void Update()
    {
        if (playerEntered && !roomCleared)
        {
            if (gm.creeps.Count <= 0)
            {
                roomCleared = true;
                foreach(GameObject go in doors)
                {
                    Destroy(go);
                }
                gm.increaseProg();
            }
        }
    }
    /*public void toggleVis()
    {
        foreach (Renderer ren in rendos)
        {
            ren.enabled = isToggled;
        }
    }*/
    public void ReactToPlayer()
    {
        gm.acplayer.currentRoom = this;
        if (!isStart && !playerEntered)
        {
            mapShow.SetActive(true);
            GameObject decoyCreep = GameObject.Instantiate((GameObject)Resources.Load("Decoy"));
            Creep decoyBoy = decoyCreep.GetComponent<Creep>();
            decoyBoy.gameManager = gm;
            gm.creeps.Add(decoyBoy);
            foreach (spawnPoints sp in spawnLocs)
            {
                GameObject newDoor = Instantiate(door);
                switch (sp.positionNum)
                {
                    case 1:
                        newDoor.transform.position = new Vector3(sp.spoint.x, sp.spoint.y, sp.spoint.z + 22.5f);
                        doors.Add(newDoor);
                        break;
                    case 2:
                        newDoor.transform.position = new Vector3(sp.spoint.x, sp.spoint.y, sp.spoint.z - 22.5f);
                        doors.Add(newDoor);
                        break;
                    case 3:
                        newDoor.transform.position = new Vector3(sp.spoint.x + 22.5f, sp.spoint.y, sp.spoint.z);
                        doors.Add(newDoor);
                        newDoor.transform.eulerAngles = new Vector3(0, 90, 0);
                        break;
                    case 4:
                        newDoor.transform.position = new Vector3(sp.spoint.x - 22.5f, sp.spoint.y, sp.spoint.z);
                        doors.Add(newDoor);
                        newDoor.transform.eulerAngles = new Vector3(0, 90, 0);
                        break;
                }
            }
        }
        if (!playerEntered && !isStart)
        {
            int amtEnemies = Random.Range(minSpawn, maxSpawn);
            for (int i = 0; i < amtEnemies; i++)
            {
                float randX = Random.Range(-22.5f, 22.5f);
                float randZ = Random.Range(-22.5f, 22.5f);
                spawn.x = randX; spawn.z = randZ; spawn.y = 1;
                int numPoss = creepsPoss.Count;
                int randCreep = Random.Range(0, gm.diffSpawns[gm.stageOn].creeps.Count);
                GameObject warn = Instantiate(gm.warningSpawn, transform.position + spawn, Quaternion.identity);
                warn.GetComponent<CreepWarn>().gm = gm;
                warn.GetComponent<CreepWarn>().player = gm.acplayer;
                warn.GetComponent<CreepWarn>().creepSpawn = creepsPoss[randCreep];
            }
            playerEntered = true;
        }

    }
}
