using UnityEngine;
using System.Collections; // 이 라인을 추가하세요.

public class KrakenMove : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public Transform tentacle; // Tentacle (근접 공격을 위한 객체)

    public float speed = 3f; // 이동 속도
    public float rotationSpeed = 2f; // 회전 속도
    public float forceDuration = 0.5f; // 밀리 어택을 위한 힘이 지속되는 시간
    public float forceStrength = 15f; // 밀리 어택의 힘

    public float maxTentacleDistance = 5f; // Tentacle이 플레이어를 향해 이동할 수 있는 최대 거리
    public float minTentacleDistance = 2f; // Tentacle이 플레이어와 너무 가까운 경우, 다리가 본체를 따라 이동하도록 하는 최소 거리

    private Rigidbody2D tentacleRb; // Tentacle의 Rigidbody2D
    private bool isAttacking = false; // 공격 상태 체크

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

        // Tentacle을 찾고 초기화
        if (tentacle == null)
        {
            GameObject tentacleObj = GameObject.FindGameObjectWithTag("Tentacle");
            if (tentacleObj != null)
            {
                tentacle = tentacleObj.transform;
            }
        }

        // 텐타클의 Rigidbody2D를 찾습니다.
        tentacleRb = tentacle.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player != null)
        {
            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > maxTentacleDistance)
            {
                // 플레이어와 멀면 접근하고 본체 회전
                Move();
            }
            else if (distanceToPlayer < minTentacleDistance && !isAttacking)
            {
                // 플레이어와 가까우면 밀리 어택
                StartCoroutine(MeleeAttack());
                player.GetComponent<PlayerHealth>().TakeDamage(10);
            }
        }
    }

    // 플레이어가 멀리 있을 때 회전하며 접근
    private void Move()
    {
        // 플레이어 방향
        Vector2 direction = (player.position - transform.position).normalized;

        // 본체 회전 (회전 속도를 추가)
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 방향으로 이동
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Tentacle도 플레이어를 향해 이동 (근접 공격용 객체)
        if (tentacle != null)
        {
            Vector2 tentacleDirection = (player.position - tentacle.position).normalized;
            tentacle.position = Vector2.MoveTowards(tentacle.position, player.position, speed * Time.deltaTime);
            
            // 텐타클에 힘을 추가하여 플레이어를 향하게 만듬
            ApplyTentacleForce(tentacleDirection);
        }
    }

    // 플레이어 방향으로 힘을 적용 (일시적으로)
    private void ApplyTentacleForce(Vector2 direction)
    {
        if (tentacleRb != null)
        {
            // 힘을 일시적으로 추가하고 일정 시간이 지나면 제거
            StartCoroutine(ApplyForceTemporarily(direction));
        }
    }

    private IEnumerator ApplyForceTemporarily(Vector2 direction)
    {
        // 텐타클에 힘을 가합니다
        tentacleRb.AddForce(direction * forceStrength, ForceMode2D.Impulse);

        // 일정 시간 동안 힘을 지속합니다
        yield return new WaitForSeconds(forceDuration);

        // 힘을 제거하기 위해 리셋 (힘을 없애고 원래 상태로 돌아감)
        tentacleRb.linearVelocity = Vector2.zero;  // 속도를 0으로 리셋
    }

    // 밀리 어택을 위한 근접 공격
    private IEnumerator MeleeAttack()
    {
        isAttacking = true;

        // 밀리 어택으로 텐타클에 힘을 주는 동작
        Vector2 attackDirection = (player.position - tentacle.position).normalized;
        ApplyTentacleForce(attackDirection);

        // 잠시 후 공격 완료 상태로 돌아가기
        yield return new WaitForSeconds(forceDuration);

        isAttacking = false;

        // 공격 후 텐타클이 다시 플레이어를 향해 움직이도록 처리
        if (tentacle != null)
        {
            Vector2 tentacleDirection = (player.position - tentacle.position).normalized;
            tentacle.position = Vector2.MoveTowards(tentacle.position, player.position, speed * Time.deltaTime);
        }
    }
}
