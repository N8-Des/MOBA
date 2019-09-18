using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMaterial : MonoBehaviour {
    Renderer water;
    Vector2 offset = new Vector2(0.001f, 0.001f);
    void Start()
    {
        water = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        if (water.material.mainTextureOffset.x <= 2)
        {
            water.material.SetTextureOffset("_MainTex", water.material.mainTextureOffset + offset);
        }else
        {
            water.material.SetTextureOffset("_MainTex", Vector2.zero);

        }
    }
}
