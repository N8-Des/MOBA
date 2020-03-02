using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexDrink : MonoBehaviour {
    public Vector3 targetPos;
    public int damage;
    public GameObject Explosion;
    void Start()
    {
        StartCoroutine(KnockBack());
    }
    public IEnumerator KnockBack()
    {
        float startTime = Time.time;
        Vector3 center = (transform.position + targetPos) * 0.5f;
        center.y += -1f;
        Vector3 startArea = transform.position - center;
        Vector3 endArea = targetPos - center;
        float suck = 0;
        while (suck < 1)
        {
            float t = (Time.time - startTime) / 1;
            transform.position = Vector3.Slerp(startArea, endArea, t);
            transform.position += center;
            suck += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GameObject puddle = GameObject.Instantiate(Explosion);
        puddle.transform.position = this.transform.position - new Vector3(0, 0.6f, 0);
        puddle.GetComponent<Hurtbox>().damage = damage;
        Destroy(this.gameObject);
    }
}
