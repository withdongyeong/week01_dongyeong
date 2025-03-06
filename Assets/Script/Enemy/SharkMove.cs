using UnityEngine;
using System.Collections;

public class SharkMove : MonoBehaviour
{
    public Transform target; // 플레이어의 Transform
    public float speed = 3f; // 이동 속도
    public float rotationSpeed = 0.5f; // 회전 속도
    public float collisionMoveDistance = 3f; // 충돌 후 이동 거리
    public float secondMoveDistance = 1f; // 두 번째 이동 거리
    private bool isReversing = false;

    public ParticleSystem bloodParticle;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                Transform whaleTransform = playerObj.transform.GetChild(5);
                if (whaleTransform != null)
                {
                    target = whaleTransform;
                }
            }
        }
    }

    void Update()
    {
        if (!isReversing && target != null)
        {
            // 플레이어 방향으로 이동
            Vector2 direction = (target.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle); // 부드러운 회전 적용

            transform.position += transform.right * speed * Time.deltaTime; // 현재 방향 기준 이동
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            // 충돌 시 270도 회전 후 전진하고, 90도 회전 후 전진
            StartCoroutine(ReverseAndMove());
        }
    }

    private IEnumerator ReverseAndMove()
    {
        isReversing = true;

        // 270도 회전 (부드럽게 회전)
        float targetRotation = transform.eulerAngles.z + 270f;
        float startRotation = transform.eulerAngles.z;
        float elapsedTime = 0f;

        // 2초 동안 부드럽게 270도 회전
        while (elapsedTime < 2f)
        {
            float currentRotation = Mathf.LerpAngle(startRotation, targetRotation, elapsedTime / 2f);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
            elapsedTime += Time.deltaTime * rotationSpeed;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);

        // 270도 회전 후 전진
        Vector3 startPosition = transform.position;
        elapsedTime = 0f;
        while (elapsedTime < 1f) // 전진 시간
        {
            transform.position = Vector3.Lerp(startPosition, startPosition + transform.right * collisionMoveDistance, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 90도 회전
        targetRotation = transform.eulerAngles.z + 90f;
        startRotation = transform.eulerAngles.z;
        elapsedTime = 0f;

        // 1초 동안 부드럽게 90도 회전
        while (elapsedTime < 1f)
        {
            float currentRotation = Mathf.LerpAngle(startRotation, targetRotation, elapsedTime / 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
            elapsedTime += Time.deltaTime * rotationSpeed;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);

        // 90도 회전 후 전진
        startPosition = transform.position;
        elapsedTime = 0f;
        while (elapsedTime < 1f) // 전진 시간
        {
            transform.position = Vector3.Lerp(startPosition, startPosition + transform.right * secondMoveDistance, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동이 끝나면 원래 플레이어를 추적하는 상태로 돌아감
        isReversing = false;
    }

    public void EatWhale()
    {
        if(bloodParticle != null)
            bloodParticle.Play();
    }
}
