using UnityEngine;
using System.Collections;

public class SharkMove : MonoBehaviour
{
    public Transform target;

    public float speed = 3f;
    public float rotationSpeed = 0.5f;

    public float orbitDistance = 10f;
    public float orbitSpeed = 2f;

    public float chargeInterval = 5f;
    public float chargeDuration = 1f;
    public float chargeSpeed = 10f;

    public float recoveryDuration = 0.5f;

    private float orbitAngle = 0f;
    private float chargeTimer = 0f;

    private Vector2 fleeDirection;

    public ParticleSystem bloodParticle;

    private Harpoon attachedHarpoon; // 🛑 현재 박혀 있는 작살 저장
    private TrailRenderer trailRenderer; // 🛑 Trail Renderer 추가

    private enum SharkState
    {
        Orbiting,
        Charging,
        Fleeing,
        Stunned,
        Dead // 🛑 추가: 죽은 상태
    }

    private SharkState currentState = SharkState.Orbiting;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            target = playerObj.transform;
        }

        // 🛑 하위 오브젝트에서 Trail Renderer 찾기
        Transform fin = transform.Find("Fin/GameObject"); // Trail Renderer가 있는 오브젝트 경로
        if (fin != null)
        {
            trailRenderer = fin.GetComponent<TrailRenderer>();
        }
    }

    void Update()
    {
        if (currentState == SharkState.Dead) return; // 🛑 죽었으면 아무것도 하지 않음

        switch (currentState)
        {
            case SharkState.Orbiting:
                OrbitBehavior();
                break;

            case SharkState.Charging:
                break;

            case SharkState.Fleeing:
                break;

            case SharkState.Stunned:
                break;
        }
    }

    private void OrbitBehavior()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > orbitDistance)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            MoveAndRotate(direction, speed);
        }
        else
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= chargeInterval)
            {
                chargeTimer = 0f;
                StartCoroutine(ChargeTowardsPlayer());
            }

            Vector2 orbitCenter = target.position;
            orbitAngle += orbitSpeed * Time.deltaTime;
            Vector2 desiredPosition2D = orbitCenter + new Vector2(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle)) * orbitDistance;
            Vector3 desiredPosition = new Vector3(desiredPosition2D.x, desiredPosition2D.y, transform.position.z);
            Vector2 moveDirection = desiredPosition2D - new Vector2(transform.position.x, transform.position.y);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);
            SmoothRotate(moveDirection);
        }
    }

    private IEnumerator ChargeTowardsPlayer()
    {
        currentState = SharkState.Charging;

        Vector2 directionToPlayer = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        float elapsedRotation = 0f;
        float rotationTime = 0.5f;
        Quaternion startRotation = transform.rotation;
        while (elapsedRotation < rotationTime)
        {
            elapsedRotation += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedRotation / rotationTime);
            yield return null;
        }

        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            transform.position += (Vector3)(directionToPlayer * chargeSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentState = SharkState.Orbiting;
    }

    public void SetHarpoonedState(bool state, Vector2 playerPos)
    {
        if (state)
        {
            currentState = SharkState.Fleeing;
            fleeDirection = (transform.position - (Vector3)playerPos).normalized;
            StartCoroutine(FleeFromPlayer());
        }
        else
        {
            currentState = SharkState.Orbiting;
        }
    }

    public void SetHarpoon(Harpoon harpoon) // 🛑 작살이 박히면 연결
    {
        attachedHarpoon = harpoon;
    }

    public void SetDeadState()
    {
        currentState = SharkState.Dead; // 🛑 죽으면 모든 움직임을 멈춤

        // 🛑 Trail Renderer 끄기
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        // 🛑 박혀 있던 작살을 즉시 복귀하도록 명령
        if (attachedHarpoon != null)
        {
            attachedHarpoon.ForceReturn();
        }

        // 🛑 애니메이션 재생 후 삭제
        GetComponent<Animator>().SetTrigger("Die");
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator FleeFromPlayer()
    {
        float fleeTime = 2f;
        float fleeSpeed = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < fleeTime && currentState == SharkState.Fleeing)
        {
            transform.position += (Vector3)(fleeDirection * fleeSpeed * Time.deltaTime);
            SmoothRotate(fleeDirection);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentState = SharkState.Orbiting;
    }

    private IEnumerator DestroyAfterAnimation()
    {
        Animator animator = GetComponent<Animator>();

        // 현재 재생 중인 애니메이션의 길이를 가져와 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject); // 애니메이션이 끝나면 몬스터 삭제
    }

    void MoveAndRotate(Vector2 direction, float moveSpeed)
    {
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        SmoothRotate(direction);
    }

    void SmoothRotate(Vector2 moveDirection)
    {
        if (moveDirection != Vector2.zero)
        {
            float targetRotAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetRotAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void EatWhale() // 🛑 삭제 안 함
    {
        if (bloodParticle != null)
            bloodParticle.Play();
    }
}
