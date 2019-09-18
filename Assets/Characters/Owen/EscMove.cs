using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMove : MonoBehaviour {
    public float speed;
    public GameObject moveLocation;
    List<Creep> inRadius = new List<Creep>();
    public float lifetime;
    void Start()
    {
        StartCoroutine(waitToDie());
    }
    public void OnTriggerStay(Collider collider)
    {
        Creep creep = collider.GetComponent<Creep>();
        GameObject creObj = collider.gameObject;
        if (creep != null)
        {
            inRadius.Add(creep);
            creep.canMove = false;
            creObj.transform.position = Vector3.Lerp(creep.transform.position, moveLocation.transform.position, Time.deltaTime * speed);
        }
    }
    void Die()
    {
        foreach (Creep creep in inRadius)
        {
            creep.canMoveAgain();
        }
        Destroy(this.gameObject);
    }
    public IEnumerator waitToDie()
    {
        yield return new WaitForSeconds(lifetime);
        Die();
    }
}
