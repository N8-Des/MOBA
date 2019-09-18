using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    [SerializeField]
    GameObject player;
    Vector3 newPosition = new Vector3(0, 0, 0);
    Vector3 offset;
    void Start()
    {
        offset = new Vector3((transform.position.x - player.transform.position.x), transform.position.y, (transform.position.z - player.transform.position.z));
    }
	void Update () {
        newPosition.x = player.transform.position.x + offset.x;
        newPosition.y = transform.position.y;
        newPosition.z = player.transform.position.z + offset.z;

        transform.position = newPosition;
	}
}
