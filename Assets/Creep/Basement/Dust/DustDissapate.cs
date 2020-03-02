using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustDissapate : MonoBehaviour
{
    bool isDying;
    public ParticleSystem part;
    int countdown = 400;
    ParticleSystem.EmissionModule em;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(die());
        em = part.emission;
    }

    IEnumerator die()
    {
        yield return new WaitForSeconds(5);
        isDying = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isDying)
        {       
            em.rateOverTime = em.rateOverTime.constant - 1;
            countdown--;
            if (countdown <= 0)
            {
                GetComponent<Creep>().gameManager.delayGold(0, transform, GetComponent<Creep>());
            }
        }
    }
}
