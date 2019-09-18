using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    public Transform orb;
    public float radius;
    public Transform pivot;

    void Start()
    {
        pivot = orb.transform;
        transform.parent = pivot;
        transform.position += Vector3.up * radius;
    }

    void Update()
    {
        Vector3 orbVector = Camera.main.WorldToScreenPoint(orb.position);
        orbVector = Input.mousePosition - orbVector;
        float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;
        pivot.position = orb.position;
        transform.rotation = Quaternion.AngleAxis(angle - 65, -Vector3.up);
    }
}
