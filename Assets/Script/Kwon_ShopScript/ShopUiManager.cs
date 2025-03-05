using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUiManager : MonoBehaviour
{
    int _myCoin;
    public Text coinCount;


    private void Start()
    {
        _myCoin = StateManager.Instance.GetCoin();
        coinCount.text = "Craken Leg : " + _myCoin.ToString();
    }
}
