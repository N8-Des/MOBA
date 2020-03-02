using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCreepExtra : MonoBehaviour {
    public AlexPlayer player;
    public Transform indication;
    public Quaternion rotation;
    public LayerMask hitLayers;
    public LayerMask creepsOnly;
    public GameObject knockVis;
    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        indication.transform.rotation = rotation;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers))
        {
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, creepsOnly) && hit.transform.tag == "Creep")
            {
                moveIndication(hit.point);
                player.creepE = hit.transform.gameObject.GetComponent<Creep>();
            }else
            {
                knockVis.SetActive(false);
            }
        }

    }
    public virtual void moveIndication(Vector3 hitPoint)
    {
        indication.transform.position = hitPoint;
        knockVis.SetActive(true);
    }
    void onDisable()
    {
        knockVis.SetActive(false);
    }
}
