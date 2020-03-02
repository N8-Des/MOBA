using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour {
    public int speed;
    void Start()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = -transform.forward * speed;
    }
}
