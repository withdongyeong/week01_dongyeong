using UnityEngine;

public class ShopObj : MonoBehaviour
{
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
                    break;
                case UpgradeItem.ReroadingTime:
                    break;
                case UpgradeItem.Luck:
                    break;
                    
            }
        }
    }
}