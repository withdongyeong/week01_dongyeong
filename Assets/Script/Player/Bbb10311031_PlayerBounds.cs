using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_PlayerBounds : MonoBehaviour
{
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        Camera mainCamera = Camera.main;

        // 카메라의 화면 경계를 월드 좌표로 변환
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

        // 오브젝트의 반쪽 크기 저장 (충돌 영역이 아닌 실제 스프라이트 크기 기반)
        objectWidth = 0;
        objectHeight = 0;
    }

    void Update()
    {
        // 현재 플레이어 위치 가져오기
        Vector3 pos = transform.position;

        // 화면 안에서만 움직이도록 경계 설정
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);

        // 위치 적용
        transform.position = pos;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("장애물");
        }
        //Debug.Log("장애물?");
    }
}
