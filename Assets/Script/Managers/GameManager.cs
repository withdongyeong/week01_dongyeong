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
    public float hardTime = 0;
    public float playTime = 0;
    public int Level = 1;

    [Header("소환")]
    public GameObject enemySpawner;
    public GameObject obstacleSpawner;
    public GameObject cloudeSpawner;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        GameStart();
    }

    void Update()
    {
        if (isGameOver) return;

        UpdateTimer();

        if (hardTime > 30)
            StageLevelUp();

        if (playTime > 7) // 7초 초과부터 게임 시작
        {
            GamePlaying();
        }
        else if (playTime > 180)
        {
            if(UIManager.Instance.IsReadyUI)
                UIManager.Instance.UpdateGameClearUI();
            return;
        }
    }

    public void UpdateTimer()
    {
        playTime += Time.deltaTime;
        hardTime += Time.deltaTime;
        UIManager.Instance.UpdateTimeText((int)playTime);
    }

    public void StageLevelUp()
    {
        hardTime = 0;
        Level += 1;
        UIManager.Instance.UpdateLevelText(Level);
        //enemySpawner.GetComponent<EnemySpawn>().spawnInterval -= 1f;
    }

    public void GameStart()
    {
        enemySpawner.SetActive(false);
    }

    public void GamePlaying()
    {
        UIManager.Instance.UpdateGamePlayingUI();
        enemySpawner.SetActive(true);
    }

    public void GameOver()
    {
        UIManager.Instance.UpdateGameOverUI();
        isGameOver = true;
    }
}