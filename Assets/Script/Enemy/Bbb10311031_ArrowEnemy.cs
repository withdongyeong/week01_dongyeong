using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_ArrowEnemy : MonoBehaviour
{
    public float speed = 0.2f; // 아래로 이동하는 속도

    public GameObject EnemyPrefab; // 생성할 오브젝트 프리팹
    
    public float spawnInterval = 2f; // 생성 간격 (초)

    void Start()
    {
        // 일정 시간마다 SpawnObject() 실행
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

     void SpawnObject()
    {
        if (EnemyPrefab != null)
        {
            Bbb10311031_SoundManager.instance.PlaySFX("Arrow");
            Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
