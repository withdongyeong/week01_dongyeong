using UnityEngine;
using System.Collections;

public class Harpoon : MonoBehaviour
{
    public GameObject bloodParticlePrefab;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float returnSpeed = 6f;
    public float pullSpeed = 1f;
    public float playerPullSpeed = 0.5f;
    public float backwardDistance = 1.5f;
    public float backwardSpeed = 4f;

    [Header("Positions")]
    public Vector3 targetPosition;
    public Vector3 startPosition;

    [Header("Tail / Line Renderer")]
    public GameObject tail;

    [Header("Harpoon Duration")]
    public float harpoonDuration = 5f;

    private bool isMoving = false;
    private bool isPulling = false;
    private bool isReturn = false;
    private bool isPreparing = true;

    private float pullTimer = 0f;

    private PlayerAttack _playerAttack;
    private GameObject playerObj;
    private GameObject enemy;
    private Vector3 playerPrevPosition;
    private Vector3 hitOffset;

    private CameraController cameraController;
    private Transform[] points = new Transform[2];

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
        returnSpeed = returnSpeed * StateManager.Instance.ReloadingTime();
        playerPrevPosition = playerObj.transform.position;

        SetTail();
        transform.up = (targetPosition - startPosition).normalized;

        StartCoroutine(PrepareAndShoot());
    }

    // 초기 라인 렌더러 설정 (start: 작살의 transform, end: 플레이어)
    void SetTail()
    {
        points[0] = transform;
        points[1] = playerObj.transform;
        tail.GetComponent<LineController>().SetUpLine(points);
    }

    IEnumerator PrepareAndShoot()
    {
        float elapsedTime = 0f;
        Vector3 backwardStartPos = transform.position;
        Vector3 backwardTargetPos = backwardStartPos - transform.up * backwardDistance;

        while (elapsedTime < backwardDistance / backwardSpeed)
        {
            float progress = elapsedTime / (backwardDistance / backwardSpeed);
            Vector3 playerMovementOffset = playerObj.transform.position - playerPrevPosition;
            playerPrevPosition = playerObj.transform.position;

            backwardStartPos += playerMovementOffset;
            backwardTargetPos += playerMovementOffset;

            transform.position = Vector3.Lerp(backwardStartPos, backwardTargetPos, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = backwardTargetPos;
        isPreparing = false;

        startPosition = transform.position;
        isMoving = true;
    }

    void Update()
    {
        if (isPreparing) return;

        Vector3 playerMovementOffset = playerObj.transform.position - playerPrevPosition;
        playerPrevPosition = playerObj.transform.position;
        transform.position += playerMovementOffset;

        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) <= backwardDistance + 0.1f)
            {
                StateManager.Instance.CoinPlus();
                isReturn = true;
                isMoving = false;
                GetComponent<CapsuleCollider2D>().enabled = false;
            }
        }
        else if (isPulling)
        {
            HandlePulling();
        }
        else if (isReturn && playerObj != null)
        {
            HandleReturning();
        }
    }

    void HandlePulling()
    {
        pullTimer += Time.deltaTime;
        if (pullTimer >= harpoonDuration || enemy == null)
        {
            isPulling = false;
            isReturn = true;
            NotifyShark(false);
        }
        else
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                playerObj.transform.position,
                pullSpeed * Time.deltaTime
            );

            playerObj.transform.position = Vector3.MoveTowards(
                playerObj.transform.position,
                enemy.transform.position,
                playerPullSpeed * Time.deltaTime
            );

            hitOffset = Vector3.Lerp(hitOffset, Vector3.zero, 2f * Time.deltaTime);
            transform.position = enemy.transform.position + hitOffset;

            UpdateTail();
            if (Vector3.Distance(enemy.transform.position, playerObj.transform.position) < 0.5f)
            {
                isPulling = false;
                isReturn = true;
                NotifyShark(false);
            }
        }
    }

    void HandleReturning()
    {
        UpdateTail();
        transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, returnSpeed * Time.deltaTime);
        HarpoonRotation();
        if (Vector3.Distance(transform.position, playerObj.transform.position) < 0.1f)
        {
            Debug.Log("작살 준비 완료");
            _playerAttack.ReloadHarpoon();
            Destroy(gameObject);
        }
    }

    void UpdateTail()
    {
        Transform[] returnPoints = new Transform[2];
        returnPoints[0] = transform;
        returnPoints[1] = playerObj.transform;
        tail.GetComponent<LineController>().SetUpLine(returnPoints);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPreparing) return;

        if (other.CompareTag("Enemy") && isMoving)
        {
            HandleEnemyHit(other);
        }
        else if (other.CompareTag("Obstacle") && isMoving)
        {
            ReturnStart();
        }
        else if (other.CompareTag("Boss") && isMoving)
        {
            GameManager.Instance.DamagedBossHP(2);
            ReturnStart();
        }
    }

    void HandleEnemyHit(Collider2D other)
    {
        StateManager.Instance.CoinPlus();
        enemy = other.gameObject;
        isMoving = false;
        isPulling = true;
        pullTimer = 0f;
        GetComponent<CapsuleCollider2D>().enabled = false;
        hitOffset = transform.position - enemy.transform.position;

        StartCoroutine(cameraController.ShakeCamera());
        SpawnBloodEffect(other);
        UpdateTail();

        NotifyShark(true);

        // 🛑 적이 상어라면, Harpoon을 등록 (죽을 때 작살 복귀 가능)
        SharkMove shark = enemy.GetComponent<SharkMove>();
        if (shark != null)
        {
            shark.SetHarpoon(this);
        }
    }

    void NotifyShark(bool isHarpooned)
    {
        if (enemy != null)
        {
            SharkMove shark = enemy.GetComponent<SharkMove>();
            if (shark != null)
            {
                shark.SetHarpoonedState(isHarpooned, playerObj.transform.position);
            }
        }
    }

    void SpawnBloodEffect(Collider2D other)
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
    }

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
            UpdateTail();

            NotifyShark(true);
        }
        else if (collision.gameObject.CompareTag("Obstacle") && isMoving)
        {
            ReturnStart();
        }
    }

    void HarpoonRotation()
    {
        Vector2 direction = transform.position - playerObj.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    void ReturnStart()
    {
        isMoving = false;
        isPulling = false;
        isReturn = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        NotifyShark(false);
    }

    public void ForceReturn()
    {
        if (!isReturn) // 이미 돌아오는 중이면 중복 실행 방지
        {
            ReturnStart();

        }
    }
}
