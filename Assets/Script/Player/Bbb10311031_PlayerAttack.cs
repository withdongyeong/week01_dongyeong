using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_PlayerAttack : MonoBehaviour
{

    public float attackTimeDelay = 1.5f;
    float attackTime = 0;
    

    public float shakeDuration = 0.3f; // 흔들리는 지속 시간
    public float shakeMagnitude = 0.2f; // 흔들림 강도

    private Vector3 originalPosition;

    public GameObject bullet;


    void Start() {
        originalPosition = Camera.main.transform.position; // 원래 카메라 위치 저장
    }
    void Update()
    {

        attackTime += Time.deltaTime;
        // 마우스 클릭 시 이동 시작
        if (Input.GetMouseButtonDown(0) && attackTime > attackTimeDelay)
        {
            attackTime = 0;
            GameObject bullet1 = Instantiate(bullet, transform.position, Quaternion.identity);
            bullet1.GetComponent<Spear>().targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Bbb10311031_SoundManager.instance.PlaySFX("Canon");
            StartCoroutine(ShakeCamera());
        }
    }

    IEnumerator ShakeCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            Camera.main.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalPosition; // 원래 위치로 복귀
    }
}
