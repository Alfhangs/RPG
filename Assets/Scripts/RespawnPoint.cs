using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject enemyTarget;

    private void Start()
    {
        SpawnEnemy();
    }
    void SpawnEnemy()
    {
        Vector3 randomSpawn = new Vector3(transform.position.x + Random.Range(-100, 100), transform.position.y, transform.position.z + Random.Range(-100, 100));

        GameObject clone;
        clone = Instantiate(enemyPrefab, randomSpawn, Quaternion.identity);
        enemyTarget = clone;
        enemyTarget.GetComponent<EnemyStats>().respawnPointLoc = this.gameObject;

        RaycastHit hit;
        if(Physics.Raycast(enemyTarget.transform.position, -Vector3.up, out hit))
        {
            enemyTarget.transform.position = new Vector3(enemyTarget.transform.position.x, hit.point.y + 5, enemyTarget.transform.position.z);
        }
    }
}
