using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class TentacleProjectile : MonoBehaviour
{
    public float speed = 6f; // �̵� �ӵ�
    public float returnSpeed = 6f;
    public Vector3 targetPosition; // Ŭ���� ��ġ
    public Vector3 startPosition; // ���� ��ġ 
    public GameObject tail;    // �� ���� �κ�

    public GameObject Kraken;

    public float acceleration = 2f; // ���ӵ�
    private float currentSpeed = 0f; // ���� �ӵ�

    private Vector3 originalScale; // ���� ũ��
    private bool isMoving = false;
    private bool isReturn = false;
    private float reloadingTime;
    private Bbb10311031_PlayerAttack _playerAttack;


    GameObject enemy;
    GameObject ownerObj;

    Transform[] points = new Transform[2];


    private void Awake()
    {
        ownerObj = gameObject;
        _playerAttack = ownerObj.GetComponent<Bbb10311031_PlayerAttack>();
    }

    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f); // 3D ��ǥ�� 2D�� ����
        returnSpeed = returnSpeed * StateManager.Instance.ReloadingTime(); // ���ƿ��� �ӵ�
        SetTail();

        isMoving = true;
    }

    void SetTail()
    {
        points[0] = Kraken.transform;
        points[1] = gameObject.transform;
        tail.GetComponent<LineController>().SetUpLine(points);
    }



    void Update()
    {

        if (isMoving)
        {
            transform.up = (targetPosition - startPosition).normalized; // �ۻ� ��ü ����

            // �̵�
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            // ������ �����ϸ� ����
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
                isReturn = true; // ������ ��������
                isMoving = false;
                GetComponent<CapsuleCollider2D>().enabled = false;
                //Destroy(gameObject);
            }
        }
        else if (isReturn)
        {
            transform.up = Vector3.Lerp(transform.up, (targetPosition - ownerObj.transform.position).normalized, Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, ownerObj.transform.position, returnSpeed * Time.deltaTime);


            SpearRotation();

            if (Vector3.Distance(transform.position, ownerObj.transform.position) < 0.1f)
            {
                _playerAttack.AttackCountUp();
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isMoving) // ���� �浹�ϸ�
        {
            Destroy(other.gameObject);
            ReturnStart();
        }
        if (other.CompareTag("Obstacle") && isMoving) // ��ֹ��� �浹�ϸ�
        {
            ReturnStart();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && isMoving) // ���� �浹�ϸ�
        {
            Destroy(collision.gameObject);
            ReturnStart();
        }
        if (collision.gameObject.CompareTag("Obstacle") && isMoving) // ��ֹ��� �浹�ϸ�
        {
            ReturnStart();
        }
    }
    
    void SpearRotation()
    {
        Vector2 direction = transform.position - ownerObj.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    // ���ƿ��� ������ �� �浹 ���� �ȳ����� ����
    void ReturnStart()
    {
        isMoving = false;
        isReturn = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
