using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithBend : MonoBehaviour {
    public GameObject bend;

    void Update()
    {
        transform.localEulerAngles = bend.transform.localEulerAngles; //new Vector3(0, bend.transform.localEulerAngles.y, bend.transform.localEulerAngles.z); 
    }
}
