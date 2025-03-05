using UnityEngine;

public class StateManager : MonoBehaviour
{
    static float _relodaingUpgradeValue = 0.2f;
    static float _reloadingTime = 1;
    static float _luck = 2f;
    int _spearCount;
    int _reloadUpgradeCount = 1;
    int _myCoin;
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
    public void BuySpear()
    {
        _spearCount++;        
    }
    public void ReroadingUpgrade()
    {
        _reloadUpgradeCount++;
    }
    public int SpearCount()
    {
        return _spearCount;
    }
    public float ReloadingTime()
    {
        return _reloadingTime + (_relodaingUpgradeValue * _reloadUpgradeCount); // Spear.cs , isReturn 일때만 속도가 증가하도록 변경해야함 
    }
    public void Addcoin(int coin)
    {
        _myCoin += coin;
    }
    public bool UseCoin(int coin)
    {
        if (_myCoin >= coin)
        {
            _myCoin -= coin;
            //업그레이드 성공 매시지
            return true;
        }
            //업그레이드 실패 메시지
            return false;
    }
    public float GetLuckLevel()
    {
        return _luckLevel;
    }
    public void LuckLevelUpgrade()
    {
        _luckLevel += _luck;
    }
}
