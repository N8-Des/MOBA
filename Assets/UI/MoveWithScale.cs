using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithScale : MonoBehaviour {
    public GameObject obj1;
    public GameObject obj2;
    float diffx;
    float diffy;

    void Update()
    {
        diffx = Mathf.Abs(obj1.transform.position.x - obj2.transform.position.x);
        diffy = Mathf.Abs(obj1.transform.position.z - obj2.transform.position.z);
        transform.position = obj1.transform.position;
        transform.localScale = new Vector3(diffx, diffy, 1);
        float yeet = Vector3.Angle(Vector3.forward, obj1.transform.position - obj2.transform.position);    
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, yeet);
    }
}
