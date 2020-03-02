using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public GameObject baseCanvas;
    public PlayerBase acplayer;
    public GameObject Nexus;
    public GameObject warningSpawn;
    public GameObject basicHealthbar;
    public GameObject canvas1;
    public List<AbilityIndicator> abInd = new List<AbilityIndicator>();
    public List<Creep> creeps = new List<Creep>();
    public Spawner spawner;
    bool GameStart = false;
    public List<GameObject> imageItems = new List<GameObject>();
    public List<GameObject> characterObjects = new List<GameObject>();
    public List<charUI> uiList = new List<charUI>();
    public List<Image> images = new List<Image>();
    public CameraMove cam;
    public List<GameObject> charReq = new List<GameObject>();
    public GameObject playerControl;
    public List<GameObject> players = new List<GameObject>();
    //public NetworkConnection connect;
    public short contid = 0;
    public int[] levelReq;
    public int levelProg;
    public GameObject minimap;
    public GameObject menu;
    [SerializeField]
    GameObject spawnpoint;
    public GameObject canvasMenu;
    public GameObject blackCanvas;
    public int playerOn;
    DIOPlayer dio;
    Vector3 spawnPos = new Vector3(0, 0.93f, 0);
    int counter = 0;
    public GameObject startRoom;
    public int stageOn = 0;
    public List<stages> diffSpawns = new List<stages>();
    public GameObject bossUI;
    public GameObject bossPedestal;
    public RoomTemplates temp;
    [System.Serializable]
    public class charUI
    {
        public List<Sprite> sprites = new List<Sprite>();
    }
    [System.Serializable]
    public class stages
    {
        public List<GameObject> creeps = new List<GameObject>();
    }
    public void creepDied(Creep deadCreep)
    {
        creeps.Remove(deadCreep);
        Destroy(deadCreep.gameObject);
        if (creeps.Count == 0 && GameStart)
        {
            spawner.startWave();
            acplayer.abilityLevelNum += 1;
            acplayer.level += 1;
            acplayer.LevelUp();
        }
    }
    public void Update()
    {
        bool tab = Input.GetKey(KeyCode.Tab);
        if (tab)
        {
            minimap.SetActive(true);
        }else
        {
            minimap.SetActive(false);
        }
    }
    public void spawnBossPedestal()
    {
        int rand = Random.Range(0, temp.rooms.Count);
        Room bossPedRoom = temp.rooms[rand].GetComponent<Room>();
        GameObject bossPed = Instantiate(bossPedestal);
        bossPed.transform.position = bossPedRoom.transform.position;
        bossPedRoom.isBossRoom = true;
        bossPed.GetComponent<BossPedestal>().gm = this;
        bossPedRoom.setBossRoom();
    }
    public void startGame(int playerNum)
    {
        GameObject newPlayer = Instantiate(characterObjects[playerNum]);
        newPlayer.transform.position = spawnPos + new Vector3(0, 1, 0);
        acplayer= newPlayer.GetComponent<PlayerBase>();
        canvasMenu.SetActive(false);
        // baseCanvas.SetActive(true);
        cam.player = newPlayer;
        acplayer.gameManager = this;
        Instantiate(startRoom, spawnPos, startRoom.transform.rotation);
        canvas1 = acplayer.canvasPlayer;
        minimap.GetComponent<Minimap>().gameStart();
        blackCanvas.gameObject.SetActive(true);
        //Below is a yikes way to do this. But I can't think of a better one at this time.
        //I'm gonna comment it out, because I had a much, much better idea :)
        /*acplayer.IndQ = charReq[0].GetComponent<AbilityIndicator>();
        acplayer.IndW = charReq[1].GetComponent<AbilityIndicator>();
        acplayer.IndE = charReq[2].GetComponent<AbilityIndicator>();
        acplayer.IndR = charReq[3].GetComponent<AbilityIndicator>();
        acplayer.levelNum = charReq[4].GetComponent<Text>();
        acplayer.QDescription = charReq[5].GetComponent<Text>();
        acplayer.WDescription = charReq[6].GetComponent<Text>();
        acplayer.EDescription = charReq[7].GetComponent<Text>();
        acplayer.RDescription = charReq[8].GetComponent<Text>();
        acplayer.healthbar = charReq[9].GetComponent<Image>();
        acplayer.shieldbar = charReq[10].GetComponent<Image>();
        acplayer.healthdiv = charReq[11].GetComponent<Text>();
        acplayer.goldAmt = charReq[12].GetComponent<Text>();

        charReq[0].GetComponent<AbilityIndicator>().player = acplayer;
        charReq[1].GetComponent<AbilityIndicator>().player = acplayer;
        charReq[2].GetComponent<AbilityIndicator>().player = acplayer;
        charReq[3].GetComponent<AbilityIndicator>().player = acplayer;
        */

    }
    public void TheWorld(DIOPlayer dioplayer)
    {
        dio = dioplayer;
        foreach (Creep creep in creeps)
        {
            creep.canMove = false;
            creep.canAttack = false;
            creep.anim.enabled = false;
        }
    }
    public void delayGold(int gold, Transform t, Creep deadCreep)
    {
        StartCoroutine(GoldDelay(gold, t, deadCreep));
    }
    IEnumerator GoldDelay(int gold, Transform t, Creep deadCreep)
    {
        yield return new WaitForSeconds(0.2f);
        GameObject goldAdd = GameObject.Instantiate((GameObject)Resources.Load("GoldText"));
        goldAdd.transform.SetParent(canvas1.transform);
        goldAdd.GetComponent<DamageNum>().objectToFollow = t;
        goldAdd.GetComponent<DamageNum>().damageText = "+" + gold.ToString() + "G";
        creepDied(deadCreep);
    }
    public void endWorld()
    {
        foreach (Creep creep in creeps)
        {
            creep.canMove = true;
            creep.canAttack = true;
            creep.anim.enabled = true;
        }
    }
    public void increaseProg()
    {
        Debug.Log("?");
        levelProg++;
        if (levelProg >= levelReq[acplayer.level - 1])
        {
            acplayer.abilityLevelNum++;
            acplayer.LevelUp();
            acplayer.level++;
            levelProg = 0;
        }
    }
    void startSpawning()
    {
        spawner.startGame();
        GameStart = true;
    }

    void spawnPlayer()
    {
        acplayer.gameManager = this;
    }
}

