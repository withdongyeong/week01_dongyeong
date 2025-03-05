using UnityEngine;

public class IslandSpawner : MonoBehaviour
{
    public float minDistance = 2f; // 섬 간 최소 거리
    public GameObject islandPrefab;
    public int spawnIslandCount = 2;

    public BoxCollider2D areaCollider;

    public Vector2 screenArea;

    [SerializeField] GameObject[] IslandPrefabArray = new GameObject[5];

    void Start()
    {
        Camera mainCamera = Camera.main;

        // 카메라의 화면 경계를 월드 좌표로 변환
        screenArea = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

        // 구역 크기
        areaCollider.size = screenArea;

        Debug.Log(areaCollider.size);

        SpawnIsland();
    }

    void SpawnIsland()
    {
        for(int i=0; i<spawnIslandCount; i++)
        {
            Vector2 spawnPosition;
            int attempts = 0;
            do
            {
                float x = Random.Range(-areaCollider.size.x, areaCollider.size.x);
                float y = Random.Range(-areaCollider.size.y, areaCollider.size.y);

                // 원점 기준으로 좌표 설정
                spawnPosition = new Vector2(x, y);
                attempts++;
            }
            while (Physics2D.OverlapCircle(spawnPosition, minDistance) != null && attempts < 100);

            if(attempts < 100)
            {
                int randomIdx = Random.Range(0, 5);
                Instantiate(IslandPrefabArray[randomIdx], spawnPosition, Quaternion.identity);
            }
        }
    }

    void OnDrawGizmos()
    {
        Color color = new Color(1, 0, 0, 0.25f);
        Gizmos.color = color;

        Vector2 area = areaCollider.size;
        Gizmos.DrawCube(transform.position, area);
    }

}
