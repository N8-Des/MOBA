using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public wave[] waves;
    public float delayWaves;
    public float delaySpawn;
    public int currentWave = -1;
    public GameManager gameManager;
    public GameObject basicHealthbar;
    #region wave lists
    [System.Serializable]
    public class wave{
        public wavePart[] wavePart;
    }
    [System.Serializable]
    public class wavePart
    { 
        public int numEnemies;
        public GameObject enemy;
    }
    #endregion
    public void startGame()
    {
        currentWave += 1;
        StartCoroutine(waveSpawn(waves[currentWave]));
    }
    public void startWave()
    {
        StartCoroutine(roundDelay());
    }
    public IEnumerator waveSpawn(wave waveOn)
    {
        int count = 0;
        float addition = 0;
        
        foreach (wavePart wavePartOn in waveOn.wavePart)
        {
            while (count < wavePartOn.numEnemies)
            {
                addition = Random.Range(-7, 7);
                GameObject newEnemy = Instantiate(wavePartOn.enemy);
                newEnemy.transform.position = new Vector3((transform.position.x + addition), transform.position.y - 3, (transform.position.z + Random.Range(-3, 3)));
                Creep creepyBoy = newEnemy.gameObject.GetComponent<Creep>();
                creepyBoy.player = gameManager.acplayer;
                creepyBoy.Nexus = gameManager.Nexus;
                creepyBoy.canvas1 = gameManager.canvas1;
                creepyBoy.gameManager = gameManager;
                gameManager.creeps.Add(creepyBoy);
                GameObject healthbar = Instantiate(basicHealthbar);
                healthbar.gameObject.GetComponent<AnchorUI>().objectToFollow = newEnemy.transform;
                creepyBoy.healthbar = healthbar.gameObject.GetComponent<AnchorUI>();
                healthbar.gameObject.transform.SetParent(gameManager.canvas1.transform);
                creepyBoy.gameObject.SetActive(true);
                count += 1;
                yield return new WaitForSeconds(delaySpawn);
            }
            count = 0;
        }
        //StartCoroutine(roundDelay());
    }
    public IEnumerator roundDelay()
    {
        yield return new WaitForSeconds(delayWaves);
        currentWave += 1;
        StartCoroutine(waveSpawn(waves[currentWave]));

    }
}
