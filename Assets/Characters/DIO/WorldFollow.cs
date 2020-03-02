using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldFollow : MonoBehaviour
{
    public GameObject DIO;
    public Vector3 offset;
    void Update()
    {
        transform.position = DIO.transform.position + offset;
    }
}
