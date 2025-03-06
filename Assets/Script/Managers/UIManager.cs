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
    public Button RestartBtn;
    public GameObject clearUI;
    public Text gameTime;
    public GameObject bossUI;
    public Text shaksfinUI;


    public int testNum = 1;

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            FindUI();
        }
    }

    void Start()
    {
        UpdateGameStartUI();        
    }

    void Update()
    {
        UpdateShaksfinUI();
    }

    void FindUI()
    {
        startUI = transform.GetChild(0).gameObject;
        playUI = transform.GetChild(1).gameObject;
        overUI = transform.GetChild(2).gameObject;
        clearUI = transform.GetChild(3).gameObject;

        // 재시작 버튼
        RestartBtn = overUI.transform.GetChild(1).GetComponent<Button>();
        RestartBtn.onClick.AddListener(() => GameManager.Instance.GoShopScene());
        if (RestartBtn != null)
            Debug.Log("재시작 이벤트 등록 되어있음");


        gameTime = transform.GetChild(4).GetComponent<Text>();

        bossUI = transform.GetChild(5).gameObject;
        shaksfinUI = transform.GetChild(6).GetComponent<Text>();

        //IsReadyUI = true;
    }

    public void UpdateGameStartUI()
    {
        startUI.SetActive(true);
        gameTime.gameObject.SetActive(true);
        shaksfinUI.gameObject.SetActive(true);
    }

    public void UpdateGamePlayingUI()
    {
        startUI.SetActive(false);
        playUI.SetActive(true);
        shaksfinUI.gameObject.SetActive(true);
    }

    public void UpdateGameClearUI()
    {
        playUI.SetActive(false);
        bossUI.SetActive(false);
        clearUI.SetActive(true);
    }

    public void UpdateGameOverUI()
    {
        playUI.SetActive(false);
        overUI.SetActive(true);
    }

    public void UpdateBossStart()
    {
        bossUI.SetActive(true);
        playUI.SetActive(true);
    }

    public void UpdateShaksfinUI()
    {
        Debug.Log($"샥스핀 ui: {StateManager.Instance.MyCoin}");
        shaksfinUI.text = $"Shaksfin: {StateManager.Instance.MyCoin}";
    }

    public void UpdateGoShopUI()
    {
        foreach (Transform ui in transform)
            ui.gameObject.SetActive(false);
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "IntegrateScene")
        {
            FindUI();

            Debug.LogWarning("게임 씬");
        }
        else if(scene.name == "SceneShop")
        {
            Debug.LogWarning("상점 씬");
        }
    }
}