using UnityEngine;

public class Tentacles : MonoBehaviour
{
    public Transform[] segments; // 4개의 원형 Primitive (Sphere)
    public Transform body; // 문어의 본체
    public float followSpeed = 10f; // 다리의 따라오는 속도
    public float damping = 2f; // 흔들림 감쇠

    void FixedUpdate()
    {
        // 첫 번째 세그먼트는 바디를 따라감
        segments[0].position = Vector3.Lerp(segments[0].position, body.position, followSpeed * Time.deltaTime);

        // 나머지 세그먼트는 앞의 세그먼트를 따라가도록 설정
        for (int i = 1; i < segments.Length; i++)
        {
            Vector3 targetPos = segments[i - 1].position;
            segments[i].position = Vector3.Lerp(segments[i].position, targetPos, followSpeed * Time.deltaTime);

            // 흔들림 효과 추가
            segments[i].rotation = Quaternion.Lerp(segments[i].rotation, Quaternion.identity, damping * Time.deltaTime);
        }
    }
}
