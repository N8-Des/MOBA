using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Transform[] mapParts;
    public GameManager gm;
    GameObject player;
    public GameObject[] playerHeads;
    public RectTransform mapFollow;
    bool hasEverything;
    public GameObject Wall;
    public GameObject BButton;
    Vector3 offset = new Vector3(860, 546, 0);
    PlayerBase p;
    public void gameStart()
    {
        Invoke("gatherParts", 2f);
        player = gm.acplayer.gameObject;
    }
    void gatherParts()
    {
        mapParts = GetComponentsInChildren<Transform>();
        GameObject playerHead = Instantiate(playerHeads[gm.acplayer.charNum - 1]);
        playerHead.transform.parent = transform;
        playerHead.transform.localPosition = new Vector3(player.transform.position.x * 2.033f, player.transform.position.z * 2.033f, 0);
        mapFollow.transform.position = Vector3.zero;
        p = player.GetComponent<PlayerBase>();
        gm.spawnBossPedestal();
        foreach (Transform t in mapParts)
        {
            t.SetParent(mapFollow, false);
            if (t != mapParts[1])
            {
                t.gameObject.SetActive(false);
            }
            mapParts[1].gameObject.SetActive(true);
        }
        mapParts[2].gameObject.SetActive(true);
        hasEverything = true;
        gm.blackCanvas.gameObject.SetActive(false);
    }
    void Update()
    {
        if (hasEverything)
        {
            mapFollow.transform.localPosition = new Vector3(p.currentRoom.transform.position.x * -2.033f, p.currentRoom.transform.position.z * -2.033f, 0);
        }
    }
}
