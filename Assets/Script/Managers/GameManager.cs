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

    [Header("소환")]
    public List<Transform> spawnTransformList = new List<Transform>();         // 프리팹 소환 장소 리스트,           0: 구름, 1: 상어, 2: 크라켄
    public List<GameObject> spawnPrefabList = new List<GameObject>();          // 프리팹 리스트,                     0: 구름, 1: 상어, 2: 크라켄
    public List<Coroutine> spawnIntervalCorouineList;                          // 프리팹 주기적 소환 코루틴 리스트,  0: 구름, 1: 상어
    public bool isboss;

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

        if (playTime > 15 && !isboss)
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
        //for (int i = 0; i < spawnIntervalCorouineList.Count; i++)
        //    spawnIntervalCorouineList[i] = null;

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


    // 게임오버 됐을 때
    public void GameOver()
    {
        UIManager.Instance.UpdateGameOverUI(); // 게임 오버 UI 보이기

        //if (enemySpawner != null) enemySpawner.SetActive(false);
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
        UIManager.Instance.UpdateGoShopUI();    // 인게임 UI 정리
        SceneManager.LoadScene(1);              // 상점 씬으로 이동
    }


    #region 보스

    // 보스체력
    public float maxBossHP = 100;
    float bossHP;
    GameObject bossObj;
    Image bossHealthBarFill;

    // 보스 데미지 받는 함수
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

    // 보스전 시작
    public void BossStart()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.transform.Find("Whale").gameObject.SetActive(false);

        // 남은 미끼 체력을 보스전 시작시 적용
        playerObject.GetComponent<PlayerHealth>().UpdateCurrentHP(playerObject.transform.Find("Whale").GetComponent<Whale>().currentHealth);
        

        //  보스 UI 활성화
        UIManager.Instance.UpdateBossStart();
        bossHealthBarFill = UIManager.Instance.gameObject.transform.GetChild(6).GetChild(1).GetComponent<Image>();  // 보스 체력바
        bossHP = maxBossHP;

        // 상어 소환 중지
        if (spawnIntervalCorouineList.Count >= 2)
        {
            StopCoroutine(spawnIntervalCorouineList[1]);
            spawnIntervalCorouineList[1] = null;
        }

        // 크라켄 소환
        isboss = true;
       
        bossObj = Instantiate(spawnPrefabList[2]);
    }

    // 보스 클리어시
    public void BossClear()
    {
        isGameOver = true;
        if (bossObj != null) Destroy(bossObj);
        //  UI 활성화
        UIManager.Instance.UpdateGameClearUI();

        // (구현 예정)
        Debug.Log("보스 클리어~");


        Invoke("GoShopScene", 3f);
    }
    #endregion

    // 일정 주기로 계속 프리팹 소환
    IEnumerator SpawnIntervalPrefabCoroutine(GameObject prefab, float interval, bool allowAllDirection)
    {
        while (true)
        {
            if (allowAllDirection) {
                int direction = Random.Range(0, 4);
                Vector3 spawnPos = Vector3.zero;

                switch (direction) {
                    case 0: // 위
                        spawnPos = new Vector3(Random.Range(-7, 7), 9, 0);
                        break;
                    case 1: // 아래
                        spawnPos = new Vector3(Random.Range(-7, 7), -9, 0);
                        break;
                    case 2: // 왼쪽 (화면 바깥)
                        spawnPos = new Vector3(-18, Random.Range(-7, 7), 0); 
                        break;
                    case 3: // 오른쪽 (화면 바깥)
                        spawnPos = new Vector3(18, Random.Range(-7, 7), 0);  
                        break;
                }

                Instantiate(prefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(interval);
            } 
            else {
                int randomPosX = Random.Range(-7, 7);
                Instantiate(prefab, new Vector3(randomPosX, 7, 0), Quaternion.identity);
                yield return new WaitForSeconds(interval);
            }
        }
    }
}