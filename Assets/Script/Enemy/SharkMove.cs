using UnityEngine;

public class SharkMove : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float speed = 3f; // 이동 속도
    public float rotationSpeed = 2f; // 회전 속도

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle); // 부드러운 회전 적용

            transform.position += transform.right * speed * Time.deltaTime; // 현재 방향 기준 이동
        }
    }
}
