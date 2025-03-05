using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class TentacleProjectile : MonoBehaviour
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
    GameObject bossObj;
    GameObject playerObj;

    Transform[] points = new Transform[2];


    private void Awake()
    {
        bossObj = GameObject.FindGameObjectWithTag("Boss");
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, 0f); // 3D 좌표값 2D로 변경
        returnSpeed = returnSpeed * StateManager.Instance.ReloadingTime(); // 돌아오는 속도
        SetTail();

        isMoving = true;
    }

    void SetTail()
    {
        points[0] = bossObj.transform;
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
                    //SoundManager.instance.PlaySFX("Clash");
                }
                else
                {
                    //SoundManager.instance.PlaySFX("SmallCanon");
                }
                isReturn = true; // 끝까지 도달했음
                isMoving = false;
                GetComponent<CapsuleCollider2D>().enabled = false;
                //Destroy(gameObject);
            }
        }
        else if (isReturn)
        {
            transform.up = Vector3.Lerp(transform.up, (targetPosition - bossObj.transform.position).normalized, Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, bossObj.transform.position, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, bossObj.transform.position) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isMoving) // 장애물과 충돌하면
        {
            ReturnStart();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isMoving) // 장애물과 충돌하면
        {
            ReturnStart();
        }
    }
    // 돌아오기 시작할 때 충돌 판정 안나도록 설정
    void ReturnStart()
    {
        isMoving = false;
        isReturn = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
