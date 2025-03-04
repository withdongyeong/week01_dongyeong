using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_PlayerMove : MonoBehaviour
{
    public float maxSpeed = 5f; // 최대 속도
    public float acceleration = 3f; // 가속도
    public float deceleration = 5f; // 감속도
    public float turnSpeed = 100f; // 회전 속도 (높을수록 빠르게 회전)

    private float currentSpeed = 0f; // 현재 속도
    private float moveInput;
    private float turnInput;

    void Update()
    {
        // 전진/후진 입력 받기 (W/S)
        moveInput = Input.GetAxis("Vertical"); // W(1) / S(-1)
        
        // 좌우 회전 입력 받기 (A/D)
        turnInput = Input.GetAxis("Horizontal"); // A(-1) / D(1)

        // 가속 및 감속
        if (moveInput != 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveInput * maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime); // 서서히 멈춤
        }

        // 이동 적용
        transform.position += transform.up * currentSpeed * Time.deltaTime; // 차량이 보는 방향으로 이동

        float turnAmount = turnInput * turnSpeed * Time.deltaTime; // 속도에 따라 회전량 조절
        transform.Rotate(Vector3.forward, -turnAmount); // Z축 기준으로 회전

    }
}
