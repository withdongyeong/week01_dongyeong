using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Bbb10311031_GameManager : MonoBehaviour
{

    public GameObject startUI;
    public GameObject playUI;
    public GameObject overUI;
    public GameObject clearUI;

    public GameObject enemySpawner;
    public GameObject obstacleSpawner;
    public GameObject cloudeSpawner;

    public Text gameTime;
    public Text gameLevel;

    public bool isGameOver;

    public float hardTime = 0;
    public float playTime = 0;

    public int Level = 1;

    // Start is called before the first frame update
    void Start()
    {
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        playTime+=Time.deltaTime;
        hardTime+=Time.deltaTime;

        if (hardTime > 30) {
            hardTime = 0;
            Level += 1;
            gameLevel.text = "Level " + Level.ToString();
            enemySpawner.GetComponent<Bbb10311031_EnemySpawn>().spawnInterval -= 1f;
        }
        gameTime.text = ((int)playTime).ToString();
        if (playTime > 180) {
            GameClear();
            return;
        }
        if (isGameOver) return;
        if (playTime > 7) {
            GamePlaying();
        }

    }


    public void GameStart() {
        startUI.SetActive(true);
        enemySpawner.SetActive(false);
    }

    public void GamePlaying() {
        startUI.SetActive(false);
        playUI.SetActive(true);
        enemySpawner.SetActive(true);
    }

    public void GameOver() {
        playUI.SetActive(false);
        overUI.SetActive(true);
        isGameOver = true;
    }

    public void GameClear() {
        playUI.SetActive(false);
        clearUI.SetActive(true);
    }

    public void GameReplay() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
