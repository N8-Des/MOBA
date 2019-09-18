using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInRadius : MonoBehaviour
{
    public Transform indication;
    public LayerMask hitLayers;
    public Quaternion rotation;
    public void Start()
    {
        rotation = transform.rotation;
    }
    public void Update()
    {
        Vector3 mouse = Input.mousePosition;
        transform.rotation = rotation;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers))
        {
            moveIndication(hit.point);
        }
    }
    public virtual void moveIndication(Vector3 hitPoint)
    {
        indication.transform.position = hitPoint;
    }
}
