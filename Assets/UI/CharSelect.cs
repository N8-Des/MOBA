using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelect : MonoBehaviour {
    public GameManager gameManager;
    public GameObject manager;
    public int charNum;
    public void onClick()
    {
        gameManager.startGame(charNum);
    }
}
    