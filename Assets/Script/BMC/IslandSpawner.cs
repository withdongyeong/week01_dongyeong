using UnityEngine;

public class IslandSpawner : MonoBehaviour
{
    public float minDistance = 2f; // 섬 간 최소 거리
    public GameObject islandPrefab;
    public int spawnIslandCount = 2;

    public BoxCollider2D areaCollider;


    public LayerMask islandLayerMask;


    public Vector2 screenArea;

    void Start()
    {
        Camera mainCamera = Camera.main;

        // 카메라의 화면 경계를 월드 좌표로 변환
        screenArea = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

        // 구역 크기
        areaCollider.size = screenArea;

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
                float x = Random.Range(-areaCollider.size.x, areaCollider.size.x / 2);
                float y = Random.Range(-areaCollider.size.y / 2, areaCollider.size.y / 2);
                spawnPosition = new Vector2(x, y) + (Vector2)transform.position;
                //spawnPosition = new Vector2(x, y);
                attempts++;
            }
            while (Physics2D.OverlapCircle(spawnPosition, minDistance, islandLayerMask) != null && attempts < 100);

            if(attempts < 100)
            {
                Instantiate(islandPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Color color = new Color(1, 0, 0, 0.25f);
        Gizmos.color = color;

        Vector2 area = areaCollider.size;
        Gizmos.DrawCube(transform.position, area);
    }

}
