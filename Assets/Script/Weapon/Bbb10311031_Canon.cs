using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_Canon : MonoBehaviour
{
    public float speed = 6f; // 이동 속도
    public float maxScale = 2f; // 최대 크기 배율
    public float scaleLerpSpeed = 1f; // 크기 변화 속도
    public Vector3 targetPosition; // 클릭한 위치
    public Vector3 startPosition; // 시작 위치
    private Vector3 originalScale; // 원래 크기
    private bool isMoving = false;
    private float journeyLength;

    float distanceCovered = 0f;
    float fractionOfJourney = 0f;

    GameObject enemy;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        originalScale = transform.localScale; // 원래 크기 저장
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        isMoving = true;
    }


    void Update()
    {

        if (isMoving)
        {
            distanceCovered += Time.deltaTime * speed; // 이동 거리
            fractionOfJourney = distanceCovered / journeyLength; // 이동 진행률 (0~1)

            // 이동
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // 크기 변화 (중간까지 커지고 다시 작아짐)
            float scaleFactor = Mathf.Lerp(1f, maxScale, Mathf.Sin(fractionOfJourney * Mathf.PI)); // 사인 함수를 이용한 자연스러운 크기 변화
            transform.localScale = originalScale * scaleFactor;

            // 목적지 도착하면 멈춤
            if (fractionOfJourney >= 1f)
            {
                if (enemy != null) {
                    Destroy(enemy);
                    SoundManager.instance.PlaySFX("Clash");
                }
                else {
                    SoundManager.instance.PlaySFX("SmallCanon");
                }
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적과 충돌하면
        {
            enemy=other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적과 충돌하면
        {
            enemy=null;
        }
    }

}
