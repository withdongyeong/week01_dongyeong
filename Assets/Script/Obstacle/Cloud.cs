using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float speed = 2f; // 아래로 이동하는 속도

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;

        if(transform.position.y < -10) {
            Destroy(gameObject);
        }
    }
}
