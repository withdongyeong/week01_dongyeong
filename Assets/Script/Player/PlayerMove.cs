using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //public float maxSpeed = 5f; // 최대 속도
    //public float acceleration = 3f; // 가속도
    //public float deceleration = 5f; // 감속도
    //public float turnSpeed = 100f; // 회전 속도 (높을수록 빠르게 회전)

    //private float currentSpeed = 0f; // 현재 속도
    //private float moveInput;
    //private float turnInput;

    //void Update()
    //{
    //    // 전진/후진 입력 받기 (W/S)
    //    moveInput = Input.GetAxis("Vertical"); // W(1) / S(-1)

    //    // 좌우 회전 입력 받기 (A/D)
    //    turnInput = Input.GetAxis("Horizontal"); // A(-1) / D(1)

    //    // 가속 및 감속
    //    if (moveInput != 0)
    //    {
    //        currentSpeed = Mathf.Lerp(currentSpeed, moveInput * maxSpeed, acceleration * Time.deltaTime);
    //    }
    //    else
    //    {
    //        currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime); // 서서히 멈춤
    //    }

    //    // 이동 적용
    //    transform.position += transform.up * currentSpeed * Time.deltaTime; // 차량이 보는 방향으로 이동

    //    float turnAmount = turnInput * turnSpeed * Time.deltaTime; // 속도에 따라 회전량 조절
    //    transform.Rotate(Vector3.forward, -turnAmount); // Z축 기준으로 회전

    //}

    public float maxSpeed = 5f; // 최대 속도
    public float acceleration = 3f; // 가속도
    public float deceleration = 5f; // 감속도
    public float turnSpeed = 100f; // 회전 속도 (높을수록 빠르게 회전)

    private Rigidbody2D rb;
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 입력 받기
        moveInput = Input.GetAxis("Vertical"); // W(1) / S(-1)
        turnInput = Input.GetAxis("Horizontal"); // A(-1) / D(1)
    }

    void FixedUpdate()
    {
        // 현재 속도 가져오기
        float currentSpeed = Vector2.Dot(rb.linearVelocity, transform.up);

        // 가속 및 감속 처리
        if (moveInput != 0)
        {
            float targetSpeed = moveInput * maxSpeed;
            float speedDiff = targetSpeed - currentSpeed;
            float accelerationRate = (moveInput > 0) ? acceleration : deceleration;
            float movementForce = speedDiff * accelerationRate;
            rb.AddForce(transform.up * movementForce, ForceMode2D.Force);

            if (Mathf.Abs(currentSpeed) < maxSpeed || Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed))
            {
                rb.AddForce(transform.up * movementForce, ForceMode2D.Force);
            }
        }
        else
        {
            // 감속 시 관성을 줄이기 위한 감속 처리
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
      
        }

        // 속도 제한 적용
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        //// 회전 처리
        //float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;
        //rb.AddTorque(-turnAmount, ForceMode2D.Force);

        // 회전 처리
        float turnAmount = turnInput * turnSpeed * Time.deltaTime; // 속도에 따라 회전량 조절
        transform.Rotate(Vector3.forward, -turnAmount); // Z축 기준으로 회전
    }
}
