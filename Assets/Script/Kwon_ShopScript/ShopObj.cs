using System.Collections;
using UnityEngine;

public class ShopObj : MonoBehaviour
{
    public GameObject msgSuccess;
    public GameObject msgFailure;
    public enum UpgradeItem
    {
        SpearCount = 0,
        ReroadingTime = 1,
        Luck = 2
    }

    public UpgradeItem characterState;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spear")){
            switch (characterState)
            {
                case UpgradeItem.SpearCount:
                    StateManager.Instance.BuySpear();
                    StartCoroutine(Success());
                    // 추가 업그레이드 후 이후처리 
                    break;
                case UpgradeItem.ReroadingTime:
                    StateManager.Instance.ReroadingUpgrade();
                    //
                    break;
                case UpgradeItem.Luck:
                    StateManager.Instance.LuckLevelUpgrade();
                    //
                    break;

            }
        }
    }
    IEnumerator Success()
    {
        msgSuccess.SetActive(true);
        yield return new WaitForSeconds(1f);
        msgFailure.SetActive(false);
    }
}