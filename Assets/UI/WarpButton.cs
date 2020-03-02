using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpButton : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject roomAssociated;
    public void Warp()
    {
        if (gameManager.creeps.Count <= 0)
        {
            gameManager.acplayer.transform.position = roomAssociated.transform.position + new Vector3(0, 0, 15);
        }
    }
}
