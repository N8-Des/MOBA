using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : MonoBehaviour {
    public float timeAlive;

    public void Start()
    {
        StartCoroutine(die());
    }
    public IEnumerator die()
    {
        yield return new WaitForSeconds(timeAlive);
        Destroy(gameObject);
    }
}
