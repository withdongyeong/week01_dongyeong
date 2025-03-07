using UnityEngine;

public class Spear : MonoBehaviour
{
    public int spearDamage = 3;
    public float speed = 6f; // 이동 속도
    public float returnSpeed = 6f; // 되돌아오는 속도
    public float maxDistance = 5f; // 최대 사정거리

    public Vector3 targetPosition; // 목표 위치

    private Vector3 startPosition; // 시작 위치
    private Vector3 targetDirection; // 초기 투사 방향
    private bool isReturning = false; // 되돌아오는 중인지 여부

    public GameObject bloodParticlePrefab;

    private PlayerAttack _playerAttack;
    private GameObject playerObj;

    private CameraController cameraController;
        
    private void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        _playerAttack = playerObj.GetComponent<PlayerAttack>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Start()
    {
        startPosition = transform.position;
        // 2D 평면 상에서 z는 0으로 고정
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
        targetDirection = (targetPosition - startPosition).normalized;
    }

    void Update()
    {
        if (!isReturning)
        {
            // 투사 시에는 목표 방향으로 회전 후 이동
            transform.up = targetDirection;
            transform.position += targetDirection * speed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                isReturning = true;
            }
        }
        else
        {
            // 플레이어의 현재 위치를 기준으로 매 프레임 이동 방향 계산
            Vector3 returnDirection = (playerObj.transform.position - transform.position).normalized;
            // 회전은 업데이트하지 않고 위치만 변경
            transform.position += returnDirection * returnSpeed * Time.deltaTime;

            // 플레이어에게 도달하면 스피어 재장전 및 삭제 처리
            if (Vector3.Distance(transform.position, playerObj.transform.position) < 0.1f)
            {
                _playerAttack.ReloadSpear();
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isReturning)
        {
            // 작살의 후방 방향 계산 (-transform.up)
            Vector3 bloodDirection = -transform.up;
            
            // 충돌 위치(출혈 이펙트 발생 위치)
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Vector3 spawnPos = collisionPoint;
            
            // 출혈 이펙트의 방향 전환
            float baseAngle = Mathf.Atan2(bloodDirection.y, bloodDirection.x) * Mathf.Rad2Deg;
            float newAngle = baseAngle - 60f;

            

            // 혈액 파티클이 작살의 후방으로 튀도록 회전값을 적용하여 Instantiate
            if (bloodParticlePrefab != null)
            {
                Instantiate(bloodParticlePrefab, spawnPos, Quaternion.Euler(0, 0, newAngle));
            }

            // 적과 충돌 시 데미지 처리 후 즉시 돌아오기 시작
            isReturning = true;
            if (cameraController != null)
                StartCoroutine(cameraController.ShakeCamera());
            
            if (other.GetComponent<Monster>() != null)
            {
                // 예를 들어 20의 데미지를 준다고 가정
                other.GetComponent<Monster>().TakeDamage(spearDamage);
            }
        }
        else if (other.CompareTag("Boss") && !isReturning)
        {
            GameManager.Instance.DamagedBossHP(2);
            isReturning = true;
        }
    }
}
