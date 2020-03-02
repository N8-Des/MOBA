using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sander : MonoBehaviour
{
    public bool goForward;
    public bool isRooting = false;
    public Vector3 positionToGo;
    public NatePlayer player;
    public int damage;
    public GameObject rootVis;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(moveToFirst());
    }
    void OnTriggerEnter(Collider collider)
    {
        Creep creepHit = collider.GetComponent<Creep>();
        if (creepHit != null)
        {
            GameObject hitFX = GameObject.Instantiate((GameObject)Resources.Load("NateSanderHit"));
            hitFX.transform.position = creepHit.transform.position;
            creepHit.takeMagicDamage(damage);
            if (isRooting)
            {
                creepHit.takeRoot(2);
                Instantiate(rootVis, creepHit.transform.position, Quaternion.identity);
            }
            if(player.IndR.levelNum >= 1)
            {
                if (creepHit.isMarkedByNate)
                {
                    Destroy(creepHit.gameObject.GetComponentInChildren<NateMark>().gameObject);
                    GameObject ultMark2 = GameObject.Instantiate((GameObject)Resources.Load("NateRMark"));
                    ultMark2.transform.position = creepHit.transform.position;
                    ultMark2.transform.parent = creepHit.transform;
                    ultMark2.GetComponent<NateMark>().player = player.GetComponent<NatePlayer>();
                    ultMark2.GetComponent<NateMark>().creepOn = creepHit;
                    player.marks.Add(ultMark2.GetComponent<NateMark>());
                }
                GameObject ultMark = GameObject.Instantiate((GameObject)Resources.Load("NateRMark"));
                ultMark.transform.position = creepHit.transform.position;
                ultMark.transform.parent = creepHit.transform;
                ultMark.GetComponent<NateMark>().player = player.GetComponent<NatePlayer>();
                ultMark.GetComponent<NateMark>().creepOn = creepHit;
                player.marks.Add(ultMark.GetComponent<NateMark>());
                creepHit.isMarkedByNate = true;
            }
        }
    }
    IEnumerator moveToFirst()
    {
        while (Vector3.Distance(positionToGo, transform.position) >= 0.5f && !isRooting)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.MoveTowards(transform.position, positionToGo, 35 * Time.deltaTime);
        }
        Destroy(GetComponentInChildren<ParticleSystem>());
        transform.LookAt(player.transform.position);
        transform.eulerAngles = new Vector3(-70, 0, 0);
    }
    public void moveBackToMe(Vector3 pos)
    {
        isRooting = true;
        StartCoroutine(moveBack(pos));
    }
    IEnumerator moveBack(Vector3 posi)
    {
        while (Vector3.Distance(posi, transform.position) >= 0.5f)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.MoveTowards(transform.position, posi, 45 * Time.deltaTime);
        }
        isRooting = false;
        player.waitingGrab = false;
        Destroy(gameObject);

    }
}
