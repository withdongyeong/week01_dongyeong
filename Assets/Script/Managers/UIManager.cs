using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static UIManager _instance;
    public static UIManager Instance { get { return _instance; } private set { } }

    //public bool IsReadyUI { get; private set; }

    [Header("UI")]
    public GameObject startUI;
    public GameObject playUI;
    public GameObject overUI;
    public GameObject clearUI;
    public Text gameTime;
    public Text gameLevel;

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        FindUI();
    }

    void Start()
    {
        UpdateGameStartUI();
    }

    void FindUI()
    {
        startUI = transform.GetChild(0).gameObject;
        playUI = transform.GetChild(1).gameObject;
        overUI = transform.GetChild(2).gameObject;
        clearUI = transform.GetChild(3).gameObject;

        gameTime = transform.GetChild(4).GetComponent<Text>();
        gameLevel = transform.GetChild(5).GetComponent<Text>();

        //IsReadyUI = true;
    }

    public void UpdateGameStartUI()
    {
        startUI.SetActive(true);
    }

    public void UpdateGamePlayingUI()
    {
        startUI.SetActive(false);
        playUI.SetActive(true);
    }

    public void UpdateGameClearUI()
    {
        playUI.SetActive(false);
        clearUI.SetActive(true);
    }

    public void UpdateGameOverUI()
    {
        playUI.SetActive(false);
        overUI.SetActive(true);
    }

    public void UpdateGoShopUI()
    {
        foreach (Transform ui in transform)
            ui.gameObject.SetActive(false);
    }

    public void UpdateLevelText(int level)
    {
        gameLevel.text = $"Level {level}";
    }

    public void UpdateTimeText(int playTime)
    {
        gameTime.text = playTime.ToString();
    }

    // 재시작 버튼
    public void GameReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}