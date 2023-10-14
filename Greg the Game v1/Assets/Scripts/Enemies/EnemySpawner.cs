using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public GameObject[] guns;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Keypad5))
            SpawnEnemy(enemyPrefab);
    }
    /*
    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        int gunIndex = (int)Random.Range(0f, guns.Length - 1);

        //Wait interval seconds to spawn enemy
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, transform.position, Quaternion.identity);

        //give enemy a random gun from guns list
        GameObject gunContainer = newEnemy.transform.Find("GunContainer").gameObject;
        GameObject newGun = Instantiate(guns[gunIndex], gunContainer.transform.position, gunContainer.transform.rotation, gunContainer.transform);

        StartCoroutine(spawnEnemy(interval, enemy));
    }
    */
    private void SpawnEnemy(GameObject enemy)
    {
        int gunIndex = (int)Random.Range(0f, guns.Length);

        //Wait interval seconds to spawn enemy
        GameObject newEnemy = Instantiate(enemy, transform.position, Quaternion.identity);

        //give enemy a random gun from guns list
        GameObject gunContainer = newEnemy.transform.Find("GunContainer").gameObject;
        GameObject newGun = Instantiate(guns[gunIndex], gunContainer.transform.position, gunContainer.transform.rotation, gunContainer.transform);
    }
}
