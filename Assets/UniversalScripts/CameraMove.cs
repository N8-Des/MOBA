using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    public GameObject player;
    Vector3 newPosition = new Vector3(0, 0, 0);
    Vector3 offset = new Vector3(6.4f, 16.7f, -8.7f);
    public void setOffset()
    {
    }
    void Update () {
        newPosition.x = player.transform.position.x + offset.x;
        newPosition.y = transform.position.y;
        newPosition.z = player.transform.position.z + offset.z;
        transform.position = newPosition;
	}
}
