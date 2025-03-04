using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_Boundary : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적과 충돌하면
        {
            Destroy(other.gameObject);
        }
    }
}
