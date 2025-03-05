using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Spear : MonoBehaviour
{
    public float speed = 6f; // 이동 속도
    public float returnSpeed = 6f;
    public Vector3 targetPosition; // 클릭한 위치
    public Vector3 startPosition; // 시작 위치 
    public GameObject tail;    // 끈 연결 부분

    public float acceleration = 2f; // 가속도
    private float currentSpeed = 0f; // 현재 속도

    private Vector3 originalScale; // 원래 크기
    private bool isMoving = false;
    private bool isReturn = false;
    private float reloadingTime;
    private Bbb10311031_PlayerAttack _playerAttack;


    GameObject enemy;
    GameObject playerObj;

    Transform[] points = new Transform[2];


    private void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        _playerAttack = playerObj.GetComponent<Bbb10311031_PlayerAttack>();
    }

    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f); // 3D 좌표값 2D로 변경
        returnSpeed = returnSpeed * StateManager.Instance.ReloadingTime(); // 돌아오는 속도
        SetTail();

        isMoving = true;
    }

    void SetTail()
    {
        points[0] = playerObj.transform;
        points[1] = gameObject.transform;
        tail.GetComponent<LineController>().SetUpLine(points);
    }



    void Update()
    {

        if (isMoving)
        {
            transform.up = (targetPosition - startPosition).normalized; // 작살 물체 방향

            // 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            // 목적지 도착하면 멈춤
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                    Bbb10311031_SoundManager.instance.PlaySFX("Clash");
                }
                else
                {
                    Bbb10311031_SoundManager.instance.PlaySFX("SmallCanon");
                }
                isReturn = true; // 끝까지 도달했음
                isMoving = false;
                GetComponent<CapsuleCollider2D>().enabled = false;
                //Destroy(gameObject);
            }
        }
        else if (isReturn)
        {
            transform.up = Vector3.Lerp(transform.up, (targetPosition - playerObj.transform.position).normalized, Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, returnSpeed * Time.deltaTime);


            SpearRotation();

            if (Vector3.Distance(transform.position, playerObj.transform.position) < 0.1f)
            {
                _playerAttack.AttackCountUp();
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isMoving) // 적과 충돌하면
        {
            Destroy(other.gameObject);
            ReturnStart();
        }
        if (other.CompareTag("Obstacle") && isMoving) // 장애물과 충돌하면
        {
            ReturnStart();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && isMoving) // 적과 충돌하면
        {
            Destroy(collision.gameObject);
            ReturnStart();
        }
        if (collision.gameObject.CompareTag("Obstacle") && isMoving) // 장애물과 충돌하면
        {
            ReturnStart();
        }
    }
    
    void SpearRotation()
    {
        Vector2 direction = transform.position - playerObj.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    // 돌아오기 시작할 때 충돌 판정 안나도록 설정
    void ReturnStart()
    {
        isMoving = false;
        isReturn = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
