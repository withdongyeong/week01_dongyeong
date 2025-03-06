using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KrakenMove : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public List<Transform> tentacles; // 여러 다리 객체를 위한 리스트

    public float speed = 2f; // 이동 속도

    public float forceDuration = 1f; // 밀리 어택을 위한 힘이 지속되는 시간
    public float forceStrength = 1000f; // 밀리 어택의 힘

    public float maxTentacleDistance = 4f; // Tentacle이 플레이어를 향해 이동할 수 있는 최대 거리

    private List<Rigidbody2D> tentacleRbs = new List<Rigidbody2D>(); // 각 Tentacle의 Rigidbody2D
    private bool isAttacking = false; // 공격 상태 체크
    private int currentTentacleIndex = 0; // 현재 공격할 다리의 인덱스
    private bool isCooldown = false; // 전체 쿨타임 상태 체크

    public float tentacleCooldown = 0.5f; // 각 다리 공격 쿨타임
    public float fullCooldown = 2f; // 모든 다리 사용 후 전체 쿨타임

    public GameObject tentacleProjectilePrefab; 

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

        // Tentacle 객체들을 찾고 초기화
        foreach (Transform tentacleObj in tentacles)
        {
            if (tentacleObj != null)
            {
                tentacleRbs.Add(tentacleObj.GetComponent<Rigidbody2D>());
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // 플레이어가 4f 거리보다 멀면 가까워지려고 이동
            if (distanceToPlayer > maxTentacleDistance)
            {
                Move();
            }

            // 플레이어와의 거리가 4f 이하일 때 공격 (isAttacking, isCooldown 체크 추가)
            else if (!isAttacking && !isCooldown)
            {
                Debug.Log("공격 시작");
                StartCoroutine(MeleeAttack());
                //player.GetComponent<PlayerHealth>().TakeDamage(10);
            }
        }
    }

    // 이동 처리
    private void Move()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // 플레이어 방향으로 이동
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Tentacle도 플레이어를 향해 이동
        foreach (Transform tentacle in tentacles)
        {
            Vector2 tentacleDirection = (player.position - tentacle.position).normalized;
            tentacle.position = Vector2.MoveTowards(tentacle.position, player.position, speed * Time.deltaTime);
        }
    }

    // 플레이어 방향으로 힘을 적용 (일시적으로)
    private void ApplyTentacleForce(Transform tentacle, Vector2 direction)
    {
        Rigidbody2D tentacleRb = tentacle.GetComponent<Rigidbody2D>();
        if (tentacleRb != null)
        {
            // 힘을 일시적으로 추가하고 일정 시간이 지나면 제거
            StartCoroutine(ApplyForceTemporarily(tentacleRb, direction));
        }
    }

    private IEnumerator ApplyForceTemporarily(Rigidbody2D tentacleRb, Vector2 direction)
    {

        // 새로운 힘을 플레이어 방향으로 적용
        tentacleRb.AddForce(direction * forceStrength, ForceMode2D.Impulse);

        // 일정 시간 동안 힘을 지속
        yield return new WaitForSeconds(forceDuration);

        // // 반대 방향으로 힘을 추가하여 원래 자리로 돌아오게 함
        // Vector2 reverseDirection = -direction;
        // tentacleRb.AddForce(reverseDirection * forceStrength, ForceMode2D.Impulse);
    }


    // 밀리 어택을 위한 근접 공격
    private IEnumerator MeleeAttack()
    {
        isAttacking = true;

        // 공격할 다리 선택
        Transform tentacleToAttack = tentacles[currentTentacleIndex];

        // 탄환 생성 및 발사
        GameObject bulletObj = Instantiate(tentacleProjectilePrefab, tentacleToAttack.position, Quaternion.identity);
        bulletObj.GetComponent<TentacleProjectile>().targetPosition = player.position; 

        yield return new WaitForSeconds(tentacleCooldown);

        // 다음 다리로 변경
        currentTentacleIndex = (currentTentacleIndex + 1) % tentacles.Count;

        isAttacking = false;

        if (currentTentacleIndex == 0 && !isCooldown)
        {
            isCooldown = true;
            yield return new WaitForSeconds(fullCooldown);
            isCooldown = false;
        }
    }

}
