using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleRotate : MonoBehaviour
{
    //this should in theory only be used for Samson
    public Transform orb;
    public Transform o2;
    public float radius;
    public Transform pivot;

    void Start()
    {
        pivot = orb.transform;
        transform.position += Vector3.up * radius;
    }

    void Update()
    {
        Vector3 orbVector = o2.transform.position;
        orbVector = Input.mousePosition - orbVector;
        float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;
        pivot.position = orb.position;
        transform.rotation = Quaternion.AngleAxis(angle, -Vector3.up);
    }
}
