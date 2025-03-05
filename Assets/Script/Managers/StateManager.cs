using UnityEngine;

public class StateManager : MonoBehaviour
{
    static StateManager _instance;
    public static StateManager Instance { get { return _instance; } private set { } }

    static float _relodaingUpgradeValue = 0.2f;
    static float _reloadingTime = 1;
    static float _luck = 2f;
    static int spearCoin = 3;
    static int powerUpCoin = 2;
    static int luckCoin = 2;
    [field: SerializeField] public int SpearCount { get; set; }
    int _reloadUpgradeCount = 1;
    [field: SerializeField] public int MyCoin { get; private set; } = 10;
    [field: SerializeField] public float LuckLevel { get; private set; }


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동해도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool BuySpear()
    {
        if (UseCoin(spearCoin)) 
        { 
            SpearCount++;

            return true;
        }
        return false;
    }
    public bool ReroadingUpgrade()
    {
        if (UseCoin(powerUpCoin)) 
        { 
            _reloadUpgradeCount++;
            return true;
        }
        return false;
    }
    public bool LuckLevelUpgrade()
    {
        if (UseCoin(luckCoin)) 
        {
            LuckLevel += _luck;
            return true;
        }
        return false;    
    }
    public float ReloadingTime()
    {
        return _reloadingTime + (_relodaingUpgradeValue * _reloadUpgradeCount); // Spear.cs , isReturn 일때만 속도가 증가하도록 변경해야함 
    }

    public void Addcoin(int coin)
    {
        MyCoin += coin;
    }
    public bool UseCoin(int coin)
    {
        if (MyCoin >= coin)
        {
            MyCoin -= coin;
            ShopUiManager shopUiManager = GameObject.Find("ShopUIManager").GetComponent<ShopUiManager>();
            if (shopUiManager != null) shopUiManager.UpdatePurchase();
            return true;
        }
            //업그레이드 실패 메시지
            return false;
    }
}
