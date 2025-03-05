using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawn : MonoBehaviour
{
    public GameObject objectPrefab; // 생성할 오브젝트 프리팹
    
    public float spawnInterval = 2f; // 생성 간격 (초)

    void Start()
    {
        // 일정 시간마다 SpawnObject() 실행
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    void SpawnObject()
    {
        if (objectPrefab != null)
        {
            int randomIndex = Random.Range(-7, 7); 
            Instantiate(objectPrefab, new Vector3(randomIndex,7,0), Quaternion.identity);
        }
    }
}
