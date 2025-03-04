using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_Arrow : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float speed = 3f; // 이동 속도
    Vector2 direction;
    void Start()
    {
        // 만약 플레이어가 설정되지 않았다면 태그로 자동 할당
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                direction = (player.position - transform.position).normalized;
                transform.up = direction;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }
}
