using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    public void Death()
    {
        Destroy(gameObject);
    }
}
