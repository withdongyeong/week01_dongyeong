using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    Transform playerTransform;
    public float shakeDuration = 0.5f; // 흔들림 지속 시간
    public float shakeMagnitude = 0.2f; // 흔들림 세기 (미세하게 줄임)

    // 흔들림 오프셋 저장 변수
    private Vector3 shakeOffset = Vector3.zero;
    private float initialZ; // 카메라의 초기 z값

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
        // 초기 카메라 z값 저장
        initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // 플레이어 위치에 흔들림 오프셋을 더하고, z값은 초기값으로 유지
            transform.position = new Vector3(playerTransform.position.x + shakeOffset.x,
                                             playerTransform.position.y + shakeOffset.y,
                                             initialZ);
        }
    }

    // 흔들림 효과: basePosition을 기준으로 무작위 오프셋을 적용, z는 고정
    public IEnumerator ShakeCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // 범위를 (-0.5f, 0.5f)로 축소하여 미세한 흔들림 효과 적용
            float offsetX = Random.Range(-0.5f, 0.5f) * shakeMagnitude;
            float offsetY = Random.Range(-0.5f, 0.5f) * shakeMagnitude;
            shakeOffset = new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 흔들림 종료 후 오프셋 초기화
        shakeOffset = Vector3.zero;
    }
}
