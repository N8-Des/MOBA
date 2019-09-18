using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatIndicator : MonoBehaviour {

    public Transform orb;
    public float radius;
    public Transform pivot;
    public LayerMask hitLayers;
    public GameObject middleInd;
    public GameObject emptyLeft;
    public GameObject emptyRight;
    public GameObject leftBoatSide;
    public GameObject rightBoatSide;


    void Start()
    {
        pivot = orb.transform;
        transform.parent = pivot;
        transform.position += Vector3.up * radius;
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers))
        {
            middleInd.transform.position = hit.point;
        }
        float distanceMid = Vector3.Distance(orb.position, middleInd.transform.position);
        //Mathf.Abs(distanceMid);
        Vector3 orbVector = Camera.main.WorldToScreenPoint(orb.position);        
        orbVector = middleInd.transform.position - orbVector;
        Vector3 orbVectorRot = Camera.main.WorldToScreenPoint(orb.position);
        orbVectorRot = Input.mousePosition - orbVector;
        float angleRot = Mathf.Atan2(orbVectorRot.y, orbVectorRot.x) * Mathf.Rad2Deg;
        float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;
        //Debug.Log("Left: " + orbVectorLeft + "Right: " + orbVectorRight);
        pivot.position = orb.position;
        transform.rotation = Quaternion.AngleAxis(angleRot, -Vector3.up);
        //rightBoatSide.transform.rotation = Quaternion.AngleAxis(angle + distanceMid, -Vector3.up);
        //leftBoatSide.transform.rotation = Quaternion.AngleAxis(angle - distanceMid, -Vector3.up);


    }
}
