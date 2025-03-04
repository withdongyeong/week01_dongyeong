using UnityEngine;

public class StateManager : MonoBehaviour
{

    public float SpearCount;
    public float ReloadingTime;


    // ΩÃ±€≈Ê ∆–≈œ
    public static StateManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // æ¿ ¿Ãµø«ÿµµ ¿Ø¡ˆ
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
