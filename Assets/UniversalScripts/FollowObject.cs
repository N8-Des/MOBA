using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {
    public GameObject objectToFollow;
	void Update () {
        transform.position = objectToFollow.transform.position;
        transform.rotation = objectToFollow.transform.rotation;
	}
}
