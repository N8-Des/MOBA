using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTrail : MonoBehaviour
{
    public GameObject trailSpawn;
    GameObject parentObject;
    void Start()
    {
        parentObject = gameObject.transform.parent.gameObject;
        StartCoroutine(spawnTrail());
    }
    IEnumerator spawnTrail()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject trail = Instantiate(trailSpawn, transform.position, parentObject.transform.rotation);

        StartCoroutine(spawnTrail());
    }
}
