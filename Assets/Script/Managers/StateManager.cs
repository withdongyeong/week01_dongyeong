using UnityEngine;

public class StateManager : MonoBehaviour
{
    static float _relodaingUpgradeValue = 0.2f;
    static float _reloadingTime = 1;
    static float _luck = 2f;
    static int spearCoin = 3;
    static int powerUpCoin = 2;
    static int luckCoin = 2;
    int _spearCount;
    int _reloadUpgradeCount = 1;
    int _myCoin = 10;
    float _luckLevel;
    public static StateManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
            _spearCount++;
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
        if (UseCoin(luckCoin)) {
            _luckLevel += _luck;
            return true;
        }
        return false;    }
    public int SpearCount() { return _spearCount; }
    public float ReloadingTime()
    {
        return _reloadingTime + (_relodaingUpgradeValue * _reloadUpgradeCount); // Spear.cs , isReturn 일때만 속도가 증가하도록 변경해야함 
    }
    public float GetLuckLevel()
    {
        return _luckLevel;
    }
    public int GetCoin(){ return _myCoin; }
    public void Addcoin(int coin)
    {
        _myCoin += coin;
    }
    public bool UseCoin(int coin)
    {
        if (_myCoin >= coin)
        {
            _myCoin -= coin;
            ShopUiManager shopUiManager = GameObject.Find("ShopUIManager").GetComponent<ShopUiManager>();
            if (shopUiManager != null) shopUiManager.UpdatePurchase();
            return true;
        }
            //업그레이드 실패 메시지
            return false;
    }
}
