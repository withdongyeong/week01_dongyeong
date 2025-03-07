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
    public GameObject overUI;
    public Button RestartBtn;
    public GameObject clearUI;
    public GameObject bossUI;


    public int testNum = 1;

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
        UpdateGameStartUI();        
    }

    public void UpdateGameStartUI()
    {
        startUI.SetActive(true);
    }

    public void UpdateGamePlayingUI()
    {
        startUI.SetActive(false);
    }

    public void UpdateGameClearUI()
    {
        bossUI.SetActive(false);
        clearUI.SetActive(true);
    }

    public void UpdateGameOverUI()
    {
        overUI.SetActive(true);
    }

    public void UpdateBossStart()
    {
        bossUI.SetActive(true);
    }

    public void UpdateGoShopUI()
    {
        foreach (Transform ui in transform)
            ui.gameObject.SetActive(false);
    }

    // ����� ��ư
    public void GameReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "IntegrateScene")
        {
            Debug.LogWarning("���� ��");
        }
        else if(scene.name == "SceneShop")
        {
            Debug.LogWarning("���� ��");
        }
    }
}