using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance { get { return _instance; } private set { } }

    [Header("컨텐츠")]
    public bool isGameOver;
    public bool isGameClear;
    public float playTime = 0;  // 플레이 시간
    public int Level = 1;
    public int startTime = 5;
    public bool isStartGame = false;
    GameObject playerObject;
    public float sharkSpawnInterval = 2f;

    [Header("Player Settings")]
    public GameObject playerPrefab;             // 플레이어 프리팹
    public Vector3 playerSpawnPosition = new Vector3(0, -3, 0); // 원하는 소환 위치 (예시)

    [Header("소환")]
    public List<Transform> spawnTransformList = new List<Transform>();         // 프리팹 소환 장소 리스트,           0: 구름, 1: 상어, 2: 크라켄
    public List<GameObject> spawnPrefabList = new List<GameObject>();          // 프리팹 리스트,                     0: 구름, 1: 상어, 2: 크라켄
    public List<Coroutine> spawnIntervalCorouineList;                          // 프리팹 주기적 소환 코루틴 리스트,  0: 구름, 1: 상어
    public List<GameObject> sharkList = new List<GameObject>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GameStart();
    }

    void Update()
    {
        if (isGameOver)
            return;

        if(SceneManager.GetActiveScene().name == "IntegrateScene")
            UpdateTimer();

        if (playTime > bossSpawnTime && !isboss)
            BossStart();

        // 게임 시작
        if (playTime > startTime && !isStartGame)
        {
            isStartGame = true;
            GamePlaying();
        }

        if (playTime > 180)
        {
            UIManager.Instance.UpdateGameClearUI();
            return;
        }
    }

    public void UpdateTimer()
    {
        playTime += Time.deltaTime;
        UIManager.Instance.UpdateTimeText((int)playTime);
    }

    // 게임 시작
    public void GameStart()
    {
        isGameOver = false;
        isGameClear = false;
        playTime = 0;
        Level = 1;
        startTime = 5;
        isStartGame = false;
        isboss = false;

        spawnIntervalCorouineList = new List<Coroutine>();

        // 이전의 코루틴들이 존재할 시, 멈추고 비우기
        StopAllCoroutines();

        // 플레이어 소환 또는 위치 재설정
        if (playerPrefab != null)
        {
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
            if (existingPlayer == null)
            {
                playerObject = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            }
            else
            {
                playerObject = existingPlayer;
                playerObject.transform.position = playerSpawnPosition;
            }
        }
        else
        {
            Debug.LogWarning("Player prefab is not assigned in GameManager.");
        }

        // 구름 소환
        if (spawnIntervalCorouineList.Count == 0)
        {
            spawnIntervalCorouineList.Add(StartCoroutine(SpawnIntervalPrefabCoroutine(spawnPrefabList[0], 10.0f, false)));
            Debug.Log($"구름 코루틴 추가, 현재 코루틴 개수: {spawnIntervalCorouineList.Count}");
        }
    }

    // 게임 플레이 중 
    public void GamePlaying()
    {
        // 게임 진행 UI로 전환
        UIManager.Instance.UpdateGamePlayingUI();

        // 상어 소환
        if (spawnIntervalCorouineList.Count == 1)
        {
            spawnIntervalCorouineList.Add(StartCoroutine(SpawnIntervalPrefabCoroutine(spawnPrefabList[1], sharkSpawnInterval, true)));
            Debug.Log($"상어 코루틴 추가, 현재 코루틴 개수: {spawnIntervalCorouineList.Count}");
        }
    }

    // 게임오버 되었을 때
    public void GameOver()
    {
        UIManager.Instance.UpdateGameOverUI(); // 게임 오버 UI 보이기
        isGameOver = true;
    }

    // 플레이 씬으로 넘어감
    public void GoInGameScene()
    {
        SceneManager.LoadScene(0);
        GameStart();
    }

    // 상점 씬으로 넘어감
    public void GoShopScene()
    {
        UIManager.Instance.UpdateGoShopUI();
        SceneManager.LoadScene(1);
    }

    #region 보스

    public float maxBossHP = 100;
    float bossHP;
    GameObject bossObj;
    Image bossHealthBarFill;
    public bool isboss;
    public float bossSpawnTime = 45;

    public void DamagedBossHP(int value)
    {
        if (bossHP > 0)
        {
            print("보스 데미지 받음");
            bossHP -= value;
            bossHealthBarFill.fillAmount = bossHP / maxBossHP;
        }
        else BossClear();
    }

    public void BossStart()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.transform.Find("Whale").gameObject.SetActive(false);
        SharkDestory();
        playerObject.GetComponent<PlayerHealth>().UpdateCurrentHP(playerObject.transform.Find("Whale").GetComponent<Whale>().currentHealth);
        UIManager.Instance.UpdateBossStart();
        bossHealthBarFill = UIManager.Instance.gameObject.transform.GetChild(5).GetChild(1).GetComponent<Image>();
        bossHP = maxBossHP;

        if (spawnIntervalCorouineList.Count >= 2)
        {
            StopCoroutine(spawnIntervalCorouineList[1]);
            spawnIntervalCorouineList[1] = null;
        }
        isboss = true;
        bossObj = Instantiate(spawnPrefabList[2]);
    }

    public void BossClear()
    {
        isGameOver = true;
        if (bossObj != null) Destroy(bossObj);
        UIManager.Instance.UpdateGameClearUI();
        Debug.Log("보스 클리어~");
        Invoke("GoShopScene", 3f);
    }
    #endregion

    IEnumerator SpawnIntervalPrefabCoroutine(GameObject prefab, float interval, bool allowAllDirection)
    {
        while (true)
        {
            if (allowAllDirection) 
            {
                int direction = Random.Range(0, 4);
                Vector3 spawnPos = Vector3.zero;

                switch (direction) {
                    case 0: spawnPos = new Vector3(Random.Range(-7, 7), 9, 0); break;
                    case 1: spawnPos = new Vector3(Random.Range(-7, 7), -9, 0); break;
                    case 2: spawnPos = new Vector3(-18, Random.Range(-7, 7), 0); break;
                    case 3: spawnPos = new Vector3(18, Random.Range(-7, 7), 0); break;
                }

                GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
                if (prefab.name == "Shark")
                    sharkList.Add(go);
                yield return new WaitForSeconds(interval);
            } 
            else 
            {
                int randomPosX = Random.Range(-7, 7);
                Instantiate(prefab, new Vector3(randomPosX, 7, 0), Quaternion.identity);
                yield return new WaitForSeconds(interval);
            }
        }
    }

    public void SharkDestory()
    {
        foreach(GameObject shark in sharkList)
            Destroy(shark);
        sharkList.Clear();
    }
}
