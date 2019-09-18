using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookScale : MonoBehaviour {
    float lengthNow;
    public Transform center;
    public Transform indPos;
    Vector3 PLES = new Vector3(0, 0, 0);
    void Update()
    {
        transform.position = center.transform.position;
        lengthNow = Vector3.Distance(indPos.position, center.position) / 5;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -lengthNow);
        transform.rotation = Quaternion.LookRotation(transform.position - indPos.transform.position);
        //transform.eulerAngles = new Vector3(-transform.eulerAngles.x - 30, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
