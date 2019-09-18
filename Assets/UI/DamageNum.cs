using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNum : MonoBehaviour {
    Vector3 scroll = new Vector3(0, 0.6f, 0);
    float duration = 1.5f;
    public Text guiText;
    public Transform objectToFollow;
    public Vector3 localOffset;
    public Vector3 screenOffset;
    RectTransform _myCanvas;
    public string damageText;
    void Start()
    {
        guiText.text = damageText;
        _myCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector3 worldPoint = objectToFollow.TransformPoint(localOffset);
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(worldPoint);
        viewportPoint -= 0.5f * Vector3.one;
        viewportPoint.z = 0;
        Rect rect = _myCanvas.rect;
        viewportPoint.x *= rect.width;
        viewportPoint.y *= rect.height;
        transform.localPosition = viewportPoint + screenOffset;
    }
    void Update()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition + scroll, 90 * Time.deltaTime);
    }

}
