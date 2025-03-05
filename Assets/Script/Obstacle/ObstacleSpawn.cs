using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawn : MonoBehaviour
{
    public GameObject[] ObstaclePrefab; // 생성할 오브젝트 프리팹
    
    public float spawnInterval = 2f; // 생성 간격 (초)

    void Start()
    {
        // 일정 시간마다 SpawnObject() 실행
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    void SpawnObject()
    {
        if (ObstaclePrefab.Length > 0)
        {
            int obstacleRandomIndex = Random.Range(0, ObstaclePrefab.Length);
            int randomIndex = Random.Range(-7, 7);  
            Instantiate(ObstaclePrefab[obstacleRandomIndex], new Vector3(randomIndex,8,0), Quaternion.identity);
        }
    }
}
