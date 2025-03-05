using UnityEngine;

public class CameraController : MonoBehaviour
{
    [field: SerializeField] public Vector2 ScreenArea { get; private set; } // 화면 크기
    void Start()
    {
        // 카메라의 화면 경계를 월드 좌표로 변환
        ScreenArea = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z));
    }
}