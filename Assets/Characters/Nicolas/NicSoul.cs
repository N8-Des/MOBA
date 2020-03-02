using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicSoul : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        NicPlayer player = collider.GetComponent<NicPlayer>();
        if (player != null)
        {
            player.TouchSoul();
            Destroy(gameObject);
        }
    }
}
