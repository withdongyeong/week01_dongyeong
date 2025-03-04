using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_EnemyMove : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float speed = 3f; // 이동 속도

    Vector2 direction;

    private float attackTime = 0;

    public GameObject enemyBullet;

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

        attackTime += Time.deltaTime;

        if (attackTime > 2.5f)
        {
            attackTime = 0;
            Bbb10311031_SoundManager.instance.PlaySFX("Canon");
            GameObject bullet1 = Instantiate(enemyBullet, transform.position, Quaternion.identity);
            bullet1.GetComponent<Bbb10311031_EnemyCanon>().startPosition = transform.position; 
        }


        if (player != null)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }
}
