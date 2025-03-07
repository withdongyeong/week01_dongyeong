using UnityEngine;

public class Harpoon : MonoBehaviour
{    
    public GameObject bloodParticlePrefab;

    [Header("Movement Settings")]
    public float speed = 6f;            // 발사 시 이동 속도
    public float returnSpeed = 6f;      // 회수 시 이동 속도
    public float pullSpeed = 1f;        // 몬스터 당기는 속도

    [Header("Positions")]
    public Vector3 targetPosition;      // 발사 목표 위치
    public Vector3 startPosition;       // 발사 시작 위치

    [Header("Tail / Line Renderer")]
    public GameObject tail;             // 밧줄(Line Renderer가 포함된 오브젝트)

    [Header("Harpoon Duration")]
    public float harpoonDuration = 5f;  // 연결 상태 최대 유지 시간 (초)

    // 내부 상태 변수
    private bool isMoving = false;      // 발사 상태
    private bool isPulling = false;     // 몬스터 당기는 상태
    private bool isReturn = false;      // 회수 상태

    private float pullTimer = 0f;       // 당기는 상태에서 경과 시간

    private PlayerAttack _playerAttack;
    private GameObject playerObj;
    private GameObject enemy;           // 당겨지는 몬스터 참조

    // 몬스터와 충돌 시, 작살과 몬스터 사이의 오프셋
    private Vector3 hitOffset;

    // 라인 렌더러 연결용 배열 (여기서는 start는 작살의 위치, end는 플레이어)
    Transform[] points = new Transform[2];

    private void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        _playerAttack = playerObj.GetComponent<PlayerAttack>();
    }

    void Start()
    {
        startPosition = transform.position;
        // 2D 평면으로 고정 (z = 0)
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
        returnSpeed = returnSpeed * StateManager.Instance.ReloadingTime();
        SetTail();
        isMoving = true;
    }

    // 초기 라인 렌더러 설정 (start: 작살의 transform, end: 플레이어)
    void SetTail()
    {
        points[0] = transform;
        points[1] = playerObj.transform;
        tail.GetComponent<LineController>().SetUpLine(points);
    }

    void Update()
    {
        // 1. 발사 상태: 목표를 향해 날아감
        if (isMoving)
        {
            transform.up = (targetPosition - startPosition).normalized;
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            // 목표 지점에 도달했으나 몬스터와 충돌하지 않은 경우
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                StateManager.Instance.CoinPlus();
                isReturn = true;
                isMoving = false;
                GetComponent<CapsuleCollider2D>().enabled = false;
            }
        }
        // 2. 당기는 상태: 몬스터를 플레이어 쪽으로 끌어당김
        else if (isPulling)
        {
            pullTimer += Time.deltaTime;
            // 일정 시간이 지나거나 몬스터가 죽으면 연결 해제 후 회수 상태로 전환
            if (pullTimer >= harpoonDuration || enemy == null)
            {
                isPulling = false;
                isReturn = true;
            }
            else
            {
                // 몬스터를 플레이어 방향으로 이동시킴
                enemy.transform.position = Vector3.MoveTowards(
                    enemy.transform.position,
                    playerObj.transform.position,
                    pullSpeed * Time.deltaTime
                );

                // 작살의 위치를 몬스터의 현재 위치 + hitOffset으로 업데이트
                transform.position = enemy.transform.position + hitOffset;

                // 밧줄 업데이트: start는 작살의 transform, end는 플레이어
                Transform[] pullingPoints = new Transform[2];
                pullingPoints[0] = transform;
                pullingPoints[1] = playerObj.transform;
                tail.GetComponent<LineController>().SetUpLine(pullingPoints);

                // 몬스터가 플레이어에 충분히 가까워지면 당기는 상태 종료 후 회수 상태로 전환
                if (Vector3.Distance(enemy.transform.position, playerObj.transform.position) < 0.5f)
                {
                    isPulling = false;
                    isReturn = true;
                }
            }
        }
        // 3. 회수 상태: 작살이 플레이어 쪽으로 이동
        else if (isReturn && playerObj != null)
        {
            // 밧줄 업데이트: start는 작살의 transform, end는 플레이어
            Transform[] returnPoints = new Transform[2];
            returnPoints[0] = transform;
            returnPoints[1] = playerObj.transform;
            tail.GetComponent<LineController>().SetUpLine(returnPoints);

            transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, returnSpeed * Time.deltaTime);

            HarpoonRotation();

            if (Vector3.Distance(transform.position, playerObj.transform.position) < 0.1f)
            {
                _playerAttack.ReloadHarpoon();
                Destroy(gameObject);
            }
        }
    }

    // 충돌 이벤트 처리 (Trigger 방식)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isMoving)
        {
            StateManager.Instance.CoinPlus();
            enemy = other.gameObject;
            isMoving = false;
            isPulling = true;
            pullTimer = 0f;  // 타이머 초기화
            GetComponent<CapsuleCollider2D>().enabled = false;

            // 저장: 충돌 시의 오프셋(작살과 몬스터 사이)
            hitOffset = transform.position - enemy.transform.position;

            // 밧줄 업데이트: start는 작살, end는 플레이어
            Transform[] newPoints = new Transform[2];
            newPoints[0] = transform;
            newPoints[1] = playerObj.transform;
            tail.GetComponent<LineController>().SetUpLine(newPoints);

            // 출혈 이펙트 (원하는 경우)
            Vector3 bloodDirection = -transform.up;
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Vector3 spawnPos = collisionPoint;
            float baseAngle = Mathf.Atan2(bloodDirection.y, bloodDirection.x) * Mathf.Rad2Deg;
            float newAngle = baseAngle - 60f;
            if (bloodParticlePrefab != null)
            {
                Instantiate(bloodParticlePrefab, spawnPos, Quaternion.Euler(0, 0, newAngle));
            }
        }
        if (other.CompareTag("Obstacle") && isMoving)
        {
            ReturnStart();
        }
        if (other.CompareTag("Boss") && isMoving)
        {
            GameManager.Instance.DamagedBossHP(2);
            ReturnStart();
        }
    }

    // 충돌 이벤트 처리 (Collision 방식)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && isMoving)
        {
            enemy = collision.gameObject;
            isMoving = false;
            isPulling = true;
            pullTimer = 0f;
            GetComponent<CapsuleCollider2D>().enabled = false;
            hitOffset = transform.position - enemy.transform.position;

            Transform[] newPoints = new Transform[2];
            newPoints[0] = transform;
            newPoints[1] = playerObj.transform;
            tail.GetComponent<LineController>().SetUpLine(newPoints);
        }
        if (collision.gameObject.CompareTag("Obstacle") && isMoving)
        {
            ReturnStart();
        }
    }

    // 작살 회전 업데이트: 플레이어를 향해 회전
    void HarpoonRotation()
    {
        Vector2 direction = transform.position - playerObj.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    // 장애물 충돌 등으로 회수 상태 전환
    void ReturnStart()
    {
        isMoving = false;
        isPulling = false;
        isReturn = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
