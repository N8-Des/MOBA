using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NateMark : MonoBehaviour
{
    public Creep creepOn;
    public NatePlayer player;
    public bool isDead;
    public GameObject markPart;
    void Start()
    {
        StartCoroutine(die());
    }
    IEnumerator die()
    {
        yield return new WaitForSeconds(8);
        isDead = true;
        markPart.SetActive(false);
        creepOn.isMarkedByNate = false;
    }
}
