using UnityEngine;
using System.Collections;

public class SharkMove : MonoBehaviour
{
    public Transform target;
    
    // 기본 이동 속도 (배회 시)
    public float speed = 3f;
    public float rotationSpeed = 0.5f;
    
    // 배회 관련 변수
    public float orbitDistance = 10f;
    public float orbitSpeed = 2f;
    
    // 돌진 관련 변수 (배회 상태일 때만 적용)
    public float chargeInterval = 5f;    // 배회 후 돌진까지 시간 간격
    public float chargeDuration = 1f;      // 돌진 지속 시간
    public float chargeSpeed = 10f;        // 돌진 이동 속도

    // 회복 관련 변수 (돌진 후 배회 원으로 복귀)
    public float recoveryDuration = 0.5f;

    private float orbitAngle = 0f;
    private float chargeTimer = 0f;
    private bool isCharging = false;

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
        if (target != null && !isCharging)
        {
            float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                                              new Vector2(target.position.x, target.position.y));

            // 플레이어가 orbitDistance보다 멀면 천천히 접근
            if (distance > orbitDistance)
            {
                Vector2 direction = (new Vector2(target.position.x, target.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
                MoveAndRotate(direction, speed);
            }
            // 플레이어와의 거리가 orbitDistance 이내일 때: 배회 및 돌진
            else 
            {
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= chargeInterval)
                {
                    StartCoroutine(ChargeTowardsPlayer());
                    chargeTimer = 0f;
                    return;
                }
                
                Vector2 orbitCenter = new Vector2(target.position.x, target.position.y);
                orbitAngle += orbitSpeed * Time.deltaTime;
                Vector2 desiredPosition2D = orbitCenter + new Vector2(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle)) * orbitDistance;
                Vector3 desiredPosition = new Vector3(desiredPosition2D.x, desiredPosition2D.y, transform.position.z);
                Vector2 moveDirection = desiredPosition2D - new Vector2(transform.position.x, transform.position.y);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);
                SmoothRotate(moveDirection);
            }
        }
    }

    // 공통 이동 및 회전 함수
    void MoveAndRotate(Vector2 direction, float moveSpeed)
    {
        transform.position += new Vector3(direction.x, direction.y, 0f) * moveSpeed * Time.deltaTime;
        SmoothRotate(direction);
    }

    // 이동 방향에 맞춰 부드럽게 회전하는 함수
    void SmoothRotate(Vector2 moveDirection)
    {
        if (moveDirection != Vector2.zero)
        {
            float targetRotAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetRotAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private IEnumerator ChargeTowardsPlayer()
    {
        isCharging = true;
        
        // Phase 1: 돌진 시작 전, 플레이어 방향으로 부드럽게 회전
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 target2D = new Vector2(target.position.x, target.position.y);
        Vector2 directionToPlayer = (target2D - pos2D).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float rotationTime = 0.5f;
        float elapsedRotation = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        while (elapsedRotation < rotationTime)
        {
            elapsedRotation += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedRotation / rotationTime);
            yield return null;
        }
        
        // 저장된 배회 각도 (돌진 후 복귀 시 사용)
        float storedOrbitAngle = Mathf.Atan2((transform.position.y - target.position.y), (transform.position.x - target.position.x));
        
        // Phase 2: 돌진 이동 (매 프레임 플레이어 방향을 재계산)
        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            pos2D = new Vector2(transform.position.x, transform.position.y);
            target2D = new Vector2(target.position.x, target.position.y);
            directionToPlayer = (target2D - pos2D).normalized;
            transform.position += new Vector3(directionToPlayer.x, directionToPlayer.y, 0f) * chargeSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Phase 3: 회복 코루틴을 통해 저장된 돌진 시작 위치의 반대극점(antipodal point)으로 복귀
        yield return StartCoroutine(RecoverToOrbit(storedOrbitAngle));
        isCharging = false;
    }


    private IEnumerator RecoverToOrbit(float storedOrbitAngle)
    {
        // 반대극점: storedOrbitAngle에 PI를 더함
        float targetOrbitAngle = storedOrbitAngle + Mathf.PI;
        
        Vector2 orbitCenter = new Vector2(target.position.x, target.position.y);
        Vector2 desiredPosition2D = orbitCenter + new Vector2(Mathf.Cos(targetOrbitAngle), Mathf.Sin(targetOrbitAngle)) * orbitDistance;
        Vector3 desiredPosition = new Vector3(desiredPosition2D.x, desiredPosition2D.y, transform.position.z);
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        float startRotAngle = transform.eulerAngles.z;
        float targetRotAngle = targetOrbitAngle * Mathf.Rad2Deg;
        while (elapsedTime < recoveryDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / recoveryDuration;
            transform.position = Vector3.Lerp(startPos, desiredPosition, t);
            float angle = Mathf.LerpAngle(startRotAngle, targetRotAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        orbitAngle = targetOrbitAngle;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            // 필요한 경우 충돌 처리 추가
        }
    }

    public void EatWhale()
    {
        if (bloodParticle != null)
            bloodParticle.Play();
    }
}
