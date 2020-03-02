using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehindTarget : MonoBehaviour {

    public Transform Target;
    public float RotationSpeed;
    public Transform player;
    private Quaternion _lookRotation;
    private Vector3 _direction;
    float angle;
    AlexPlayer playerScript;
    void Start()
    {
        playerScript = player.GetComponent<AlexPlayer>();
    }
    void Update()
    {
        angle = Mathf.Atan2(playerScript.creepE.transform.position.y - player.transform.position.y, Target.transform.position.x - player.transform.position.x) * 180 / Mathf.PI;
        transform.position = Target.transform.position;
        transform.localEulerAngles = new Vector3(0, angle, 0);
    }
}
