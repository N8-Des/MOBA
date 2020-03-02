using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CanvasNetworking : NetworkBehaviour
{
    void Update()
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Canvas>().enabled = true;
        }
    }
}
