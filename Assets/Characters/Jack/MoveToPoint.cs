using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : MonoBehaviour {
    public float speed;
    public Transform target;
    void Start()
    {
        StartCoroutine(moveToPos());
    }
    IEnumerator moveToPos()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        dist = Mathf.Abs(dist);
        while (dist > 0.2f)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            dist = Vector3.Distance(transform.position, target.position);
            dist = Mathf.Abs(dist);
        }
        Destroy(gameObject);
    }

}
