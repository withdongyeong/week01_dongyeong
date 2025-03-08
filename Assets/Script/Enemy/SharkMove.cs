using UnityEngine;
using System.Collections;

public class SharkMove : MonoBehaviour
{
    public Transform target;

    public float speed = 3f;
    public float rotationSpeed = 0.5f;

    public float orbitMinDistance = 8f;
    public float orbitMaxDistance = 12f;
    public float orbitSafeDistance = 5f;
    public float orbitChangeInterval = 2f;

    public float chargeMinInterval = 3f;
    public float chargeMaxInterval = 7f;
    public float chargeDuration = 1.5f;
    public float chargeSpeed = 12f;
    public float chargeDistance = 10f;

    private float chargeTimer = 0f;
    private float nextChargeTime;
    private Vector2 fleeDirection;
    private Vector2 orbitTargetPosition;

    public ParticleSystem bloodParticle;
    private Harpoon attachedHarpoon;
    private TrailRenderer trailRenderer;
    private Rigidbody2D rb;

    private float nextOrbitChangeTime = 0f;
    private Vector2 chargeTargetPosition;

    private enum SharkState
    {
        Orbiting,
        PreparingCharge,
        Charging,
        Fleeing,
        Stunned,
        Dead
    }

    private SharkState currentState = SharkState.Orbiting;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            target = playerObj.transform;
        }

        Transform fin = transform.Find("Fin/GameObject");
        if (fin != null)
        {
            trailRenderer = fin.GetComponent<TrailRenderer>();
        }

        rb = GetComponent<Rigidbody2D>();

        nextChargeTime = Time.time + Random.Range(chargeMinInterval, chargeMaxInterval);

        SetNewOrbitTarget();
    }

    void Update()
    {
        if (currentState == SharkState.Dead) return;

        switch (currentState)
        {
            case SharkState.Orbiting:
                OrbitBehavior();
                break;
            case SharkState.PreparingCharge:
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

        if (distance > orbitMaxDistance)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            MoveAndRotate(direction, speed * 1.5f);
        }
        else
        {
            if (Time.time >= nextChargeTime)
            {
                nextChargeTime = Time.time + Random.Range(chargeMinInterval, chargeMaxInterval);
                StartCoroutine(PrepareCharge());
            }

            if (Time.time >= nextOrbitChangeTime)
            {
                SetNewOrbitTarget();
            }

            Vector2 moveDirection = (orbitTargetPosition - (Vector2)transform.position).normalized;
            MoveAndRotate(moveDirection, speed);
        }
    }

    private void SetNewOrbitTarget()
    {
        if (target == null) return;

        int maxAttempts = 5;
        for (int i = 0; i < maxAttempts; i++)
        {
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float randomDistance = Random.Range(orbitMinDistance, orbitMaxDistance);
            Vector2 randomOffset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDistance;
            Vector2 candidatePosition = (Vector2)target.position + randomOffset;

            if (Vector2.Distance(candidatePosition, target.position) > orbitSafeDistance)
            {
                orbitTargetPosition = candidatePosition;
                nextOrbitChangeTime = Time.time + orbitChangeInterval;
                return;
            }
        }

        orbitTargetPosition = (Vector2)target.position + new Vector2(orbitMinDistance, 0);
        nextOrbitChangeTime = Time.time + orbitChangeInterval;
    }

    private IEnumerator PrepareCharge()
    {
        currentState = SharkState.PreparingCharge;

        float prepareTime = 0.5f;
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(prepareTime);
        transform.localScale = originalScale;

        StartCoroutine(ChargeTowardsPlayer());
    }

    private IEnumerator ChargeTowardsPlayer()
    {
        currentState = SharkState.Charging;

        Vector2 directionToPlayer = (target.position - transform.position).normalized;
        chargeTargetPosition = (Vector2)transform.position + directionToPlayer * chargeDistance;

        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            MoveAndRotate(directionToPlayer, chargeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentState = SharkState.Orbiting;
        SetNewOrbitTarget();
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

    private IEnumerator FleeFromPlayer() // 🛑 복구 완료!
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
        SetNewOrbitTarget();
    }

    public void SetHarpoon(Harpoon harpoon) 
    {
        attachedHarpoon = harpoon;
    }

    public void SetDeadState()
    {
        currentState = SharkState.Dead;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        if (attachedHarpoon != null)
        {
            attachedHarpoon.ForceReturn();
        }

        GetComponent<Animator>().SetTrigger("Die");
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        Animator animator = GetComponent<Animator>();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
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

    public void EatWhale()
    {
        if (bloodParticle != null)
        {
            bloodParticle.Play();
        }
    }
}
