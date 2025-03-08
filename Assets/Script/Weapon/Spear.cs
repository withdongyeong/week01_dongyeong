using UnityEngine;
using System.Collections;

public class Spear : MonoBehaviour
{
    public int spearDamage = 3;
    public float forwardSpeed = 6f; // 이동 속도 (인스펙터에서 조정 가능)
    public float returnSpeed = 6f; // 되돌아오는 속도
    public float maxDistance = 5f; // 최대 사정거리 (인스펙터에서 조정 가능)
    public float backwardDistance = 1.5f; // 뒤로 이동할 거리 (인스펙터에서 조정 가능)
    public float backwardSpeed = 4f; // 뒤로 이동 속도 (인스펙터에서 조정 가능)

    public Vector3 targetPosition; // 목표 위치

    private Vector3 startPosition; // 시작 위치
    private Vector3 targetDirection; // 초기 투사 방향
    private bool isReturning = false; // 되돌아오는 중인지 여부
    private bool isPreparing = true; // 초기 뒤로 이동 상태 여부

    public GameObject bloodParticlePrefab; // 출혈 이펙트

    private PlayerAttack _playerAttack;
    private GameObject playerObj;
    private Vector3 playerPrevPosition; // 플레이어의 이전 프레임 위치
    
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
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
        targetDirection = (targetPosition - startPosition).normalized;

        // 초기 플레이어 위치 저장
        playerPrevPosition = playerObj.transform.position;

        // 초기 회전 적용 (각도를 조정하여 쏘아지는 방향과 일치)
        transform.up = targetDirection;

        // 뒤로 이동 시작
        StartCoroutine(PrepareAndShoot());
    }

    IEnumerator PrepareAndShoot()
    {
        // 1. 뒤로 이동 (플레이어의 현재 위치를 기준으로)
        float elapsedTime = 0f;
        Vector3 backwardStartPos = transform.position; // 현재 작살 위치
        Vector3 backwardTargetPos = backwardStartPos - targetDirection * backwardDistance;

        while (elapsedTime < backwardDistance / backwardSpeed)
        {
            float progress = elapsedTime / (backwardDistance / backwardSpeed);

            // 현재 프레임의 플레이어 위치 변화량 계산
            Vector3 playerMovementOffset = playerObj.transform.position - playerPrevPosition;
            playerPrevPosition = playerObj.transform.position;

            // 이동하면서 플레이어 위치 변화 반영
            backwardStartPos += playerMovementOffset;
            backwardTargetPos += playerMovementOffset;

            // 부드럽게 뒤로 이동
            transform.position = Vector3.Lerp(backwardStartPos, backwardTargetPos, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = backwardTargetPos;
        
        // 2. 전진 시작
        isPreparing = false;
        
        // 전진 시작할 때 새로운 시작 위치 설정 (이동 중 짧아지는 문제 해결)
        startPosition = transform.position;
    }

    void Update()
    {
        if (isPreparing) return;

        // 플레이어가 이동하면 작살도 함께 이동
        Vector3 playerMovementOffset = playerObj.transform.position - playerPrevPosition;
        playerPrevPosition = playerObj.transform.position;
        transform.position += playerMovementOffset;

        if (!isReturning)
        {
            // 목표 방향으로 회전 후 이동
            transform.up = targetDirection;
            transform.position += targetDirection * forwardSpeed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                isReturning = true;
            }
        }
        else
        {
            // 되돌아오는 로직
            Vector3 returnDirection = (playerObj.transform.position - transform.position).normalized;
            transform.position += returnDirection * returnSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, playerObj.transform.position) < 0.1f)
            {
                _playerAttack.ReloadSpear();
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 뒤로 빠지는 중에는 공격 판정 없음
        if (isPreparing) return;

        if (other.CompareTag("Enemy") && !isReturning)
        {
            Vector3 bloodDirection = -transform.up;
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Vector3 spawnPos = collisionPoint;
            
            float baseAngle = Mathf.Atan2(bloodDirection.y, bloodDirection.x) * Mathf.Rad2Deg;
            float newAngle = baseAngle - 60f;

            if (bloodParticlePrefab != null)
            {
                Instantiate(bloodParticlePrefab, spawnPos, Quaternion.Euler(0, 0, newAngle));
            }

            isReturning = true;
            if (cameraController != null)
                StartCoroutine(cameraController.ShakeCamera());

            if (other.GetComponent<Monster>() != null)
            {
                other.GetComponent<Monster>().TakeDamage(spearDamage);
                _playerAttack.OnSpearHit(); // 공격 성공 시 PlayerAttack에게 알림
            }
        }
        else if (other.CompareTag("Boss") && !isReturning)
        {
            GameManager.Instance.DamagedBossHP(2);
            isReturning = true;
            _playerAttack.OnSpearHit(); // 보스 타격 시에도 PlayerAttack에 알림
        }
    }

    public void SetPlayerAttack(PlayerAttack attackRef)
    {
        _playerAttack = attackRef;
    }

}