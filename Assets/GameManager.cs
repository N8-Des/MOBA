using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public CanvasUI canvas;
    public GameObject baseCanvas;
    public PlayerBase player;
    public GameObject Nexus;
    public GameObject canvas1;
    public List<AbilityIndicator> abInd = new List<AbilityIndicator>();
    public List<Creep> creeps = new List<Creep>();
    public Spawner spawner;
    bool GameStart = false;
    public List<GameObject> imageItems = new List<GameObject>();
    void Start()
    {
        Invoke("startSpawning", 5);
        foreach (AbilityIndicator indicator in abInd)
        {
            indicator.player = player;
        }
    }
    public void creepDied(Creep deadCreep)
    {
        creeps.Remove(deadCreep);
        Destroy(deadCreep.gameObject);
        if (creeps.Count == 0 && GameStart)
        {
            spawner.startWave();
            player.abilityLevelNum += 1;
            player.level += 1;
            player.LevelUp();
        }
    }
    void Update()
    {

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
    public void MurderObject(GameObject objectToKill, float delay)
    {
        StartCoroutine(kill(delay, objectToKill));
    }
    IEnumerator kill(float delay, GameObject deadObject)
    {
        yield return new WaitForSeconds(delay);
        Destroy(deadObject);
    }
    void startSpawning()
    {
        spawner.startGame();
        GameStart = true;
    }

    void spawnPlayer()
    {
        player.gameManager = this;

    }
}
