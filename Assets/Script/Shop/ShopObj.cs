using System.Collections;
using UnityEngine;

public enum UpgradeItem
{
    SpearCount = 0,
    ReroadingTime = 1,
    Luck = 2
}

public class ShopObj : MonoBehaviour
{
    public GameObject msgSuccess;
    public GameObject msgFailure;

    public UpgradeItem characterState;

    IEnumerator Success()
    {
        msgSuccess.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        msgSuccess.SetActive(false);
    }
    IEnumerator Failure()
    {
        msgFailure.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        msgFailure.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spear"))
        {
            switch (characterState)
            {
                case UpgradeItem.SpearCount:
                    if (StateManager.Instance.BuySpear())
                    {
                        StartCoroutine(Success());
                    }
                    else
                    {
                        StartCoroutine(Failure());
                    }
                    break;
                case UpgradeItem.ReroadingTime:
                    if (StateManager.Instance.ReroadingUpgrade())
                    {
                        StartCoroutine(Success());
                    }
                    else
                    {
                        StartCoroutine(Failure());
                    }
                    break;
                case UpgradeItem.Luck:
                    if (StateManager.Instance.LuckLevelUpgrade())
                    {
                        StartCoroutine(Success());
                    }
                    else
                    {
                        StartCoroutine(Failure());
                    }
                    break;
            }
        }
    }
}