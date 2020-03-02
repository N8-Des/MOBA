using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUI : MonoBehaviour
{
    public GameManager gm;

    public void yes()
    {
        //startBoss
    }
    public void no()
    {
        Destroy(gameObject);
    }
}
