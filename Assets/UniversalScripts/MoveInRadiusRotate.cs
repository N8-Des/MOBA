using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInRadiusRotate : MoveInRadius {
    public GameObject Player;
    public Transform orb;
    public float radius;
    public Transform pivot;

    void Awake()
    {
        pivot = orb.transform;
        transform.parent = pivot;
        indication.transform.position += Vector3.up * radius;
    }

    public override void moveIndication(Vector3 hitPoint)
    {
        Vector3 orbVector = Camera.main.WorldToScreenPoint(orb.position);
        orbVector = Input.mousePosition - orbVector;
        float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

        pivot.position = orb.position;
        indication.transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.up);
        indication.transform.rotation *= Quaternion.Euler(270, 0, 180);
        indication.transform.position = hitPoint;
    }
}
