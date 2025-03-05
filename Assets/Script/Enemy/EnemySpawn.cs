using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] EnemyPrefab; // 생성할 오브젝트 프리팹
    
    public float spawnInterval = 2f; // 생성 간격 (초)
    float spawnTime = 0;


    void Update() {
        spawnTime+= Time.deltaTime;
        if (spawnTime > spawnInterval) {
            int enemyRandomIndex = Random.Range(0, EnemyPrefab.Length);
            int randomIndex = Random.Range(-7, 7);  
            Instantiate(EnemyPrefab[enemyRandomIndex], new Vector3(randomIndex,9,0), Quaternion.identity);
            spawnTime = 0;
        }
    }


}
