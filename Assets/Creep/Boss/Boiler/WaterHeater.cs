using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHeater : MonoBehaviour
{
    public GameObject lineShow;

    public void lookAtPlayerPos(Vector3 lookPos)
    {
        transform.LookAt(lookPos);
    }
}
