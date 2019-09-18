using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDash : MonoBehaviour {
    float lengthNow;
    public Transform center;
    public Transform indPos;
    Vector3 PLES = new Vector3(0, 0, 0);
    void Update()
    {
        lengthNow = Vector3.Distance(indPos.localPosition, PLES) / 1.745f;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, lengthNow);
    }
}
