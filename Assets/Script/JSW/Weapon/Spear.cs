using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Spear : MonoBehaviour
{
    public float speed = 6f; // 이동 속도
    public float returnSpeed = 3f;
    public Vector3 targetPosition; // 클릭한 위치
    public Vector3 startPosition; // 시작 위치 

    public float acceleration = 2f; // 가속도
    private float currentSpeed = 0f; // 현재 속도

    private Vector3 originalScale; // 원래 크기
    private bool isMoving = false;
    private bool isReturn = false;
    private float journeyLength;
    private float reloadingTime;

    float distanceCovered = 0f;
    float fractionOfJourney = 0f;

    GameObject enemy;
    GameObject playerObj;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        startPosition = transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f); // 3D 좌표값 2D로 변경
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        isMoving = true;
    }



    void Update()
    {

        if (isMoving)
        {
            distanceCovered += Time.deltaTime * speed; // 이동 거리
            transform.up = (targetPosition - startPosition).normalized; // 작살 물체 방향

            // 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed*Time.deltaTime);

            // 목적지 도착하면 멈춤
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                    SoundManager.instance.PlaySFX("Clash");
                }
                else
                {
                    SoundManager.instance.PlaySFX("SmallCanon");
                }
                isReturn = true; // 끝까지 도달했음
                isMoving = false;
                //Destroy(gameObject);
            }
        }
        else if (isReturn)
        {
            transform.up = Vector3.Lerp(transform.up, (targetPosition - playerObj.transform.position).normalized, Time.deltaTime);
            //transform.up = (targetPosition - playerObj.transform.position).normalized; // 작살 물체 반대방향 
            transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, speed * Time.deltaTime);

            currentSpeed += acceleration * reloadingTime * Time.deltaTime; // 시간이 지날수록 속도 증가
            transform.position = Vector3.MoveTowards(transform.position, playerObj.transform.position, currentSpeed * Time.deltaTime);
            SpearRotation();

            if (Vector3.Distance(transform.position, playerObj.transform.position) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적과 충돌하면
        {
            enemy = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적과 충돌하면
        {
            enemy = null;
        }
    }
    void SpearRotation()
    {
        Vector2 direction = transform.position - playerObj.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
}
