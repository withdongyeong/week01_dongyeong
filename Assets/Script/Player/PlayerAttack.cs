using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bbb10311031_PlayerAttack : MonoBehaviour
{
    public GameObject bullet;
    public float shakeDuration = 0.3f; //
                                       // 흔들리는 지속 시간
    public float shakeMagnitude = 0.2f; // 흔들림 강도

    private Vector3 originalPosition;
    private int maxAttackCount = 1;
    public int attackCount;

    void Start() {
        originalPosition = Camera.main.transform.position; // 원래 카메라 위치 저장
        maxAttackCount += StateManager.Instance.SpearCount();
        attackCount = maxAttackCount;
    }
    void Update()
    {

        // 마우스 클릭 시 이동 시작
        if (Input.GetMouseButtonDown(0) && attackCount > 0)
        {
            AttackCountDown();
            GameObject bulletObj = Instantiate(bullet, transform.position, Quaternion.identity);
            bulletObj.GetComponent<Spear>().targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Bbb10311031_SoundManager.instance.PlaySFX("Canon");
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

    public void AttackCountUp()
    {
        attackCount += 1;
    }

    public void AttackCountDown()
    {
        if (attackCount != 0)
        {
            attackCount -= 1;
        }
    }

}
