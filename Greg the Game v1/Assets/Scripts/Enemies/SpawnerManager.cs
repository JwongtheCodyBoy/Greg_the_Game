using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [Header("References")]
    public Transform[] spawners;
    public GameObject enemyPrefab;
    public GameObject[] guns;

    [Header("Spawn Information")]
    public float spawnTimeInterval;
    public int maxEnemies;
    public float enemyAcceleration;
    public static int currentNumEnemies = 0;

    private void Awake()
    {
        Initilization();
    }

    private void Update()
    {
        if (currentNumEnemies < maxEnemies)
            StartCoroutine(WaitThenSpawn(spawnTimeInterval));
    }

    private IEnumerator WaitThenSpawn(float interval)
    {
        currentNumEnemies++;
        //Waits Interval before spawning
        yield return new WaitForSeconds(interval);
        SpawnEnemy(enemyPrefab);
    }

    private void SpawnEnemy(GameObject enemy)
    {
        int gunIndex = (int)Random.Range(0, guns.Length);
        int spawnerIndex = (int)Random.Range(0, spawners.Length);

        //Creates a new enemy at random spawner location and rotation
        GameObject newEnemy = Instantiate(enemy, spawners[spawnerIndex].transform.position, spawners[spawnerIndex].transform.rotation);
        newEnemy.GetComponentInChildren<BasicEnemyAI>().SetAccelearation(enemyAcceleration);

        //give enemy a random gun from guns list
        GameObject gunContainer = newEnemy.transform.Find("GunContainer").gameObject;
        GameObject newGun = Instantiate(guns[gunIndex], gunContainer.transform.position, gunContainer.transform.rotation, gunContainer.transform);

        //Reset walk point
        newEnemy.GetComponentInChildren<BasicEnemyAI>().SearchWalkPoint();
        newEnemy.GetComponentInChildren<BasicEnemyAI>().accel = enemyAcceleration;

        //Debuging
        Debug.Log("spawner Number: " + spawnerIndex + "\nGun Number: " + gunIndex);
    }

    public void SubtractEnemy()
    {
        currentNumEnemies--;
    }

    public void Initilization()
    {
        currentNumEnemies = 0;
        while (currentNumEnemies < maxEnemies)
        {
            SpawnEnemy(enemyPrefab);
            currentNumEnemies++;
        }
    }
}
