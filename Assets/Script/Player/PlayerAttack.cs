using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject spear;
    public GameObject harpoon;
    
    private int maxSpearAttackCount = 1;
    private int maxHarpoonAttackCount = 1;
    
    private int spearAttackCount;
    private int harpoonAttackCount;

    void Start()
    {
        maxSpearAttackCount += StateManager.Instance.SpearCount;
        spearAttackCount = maxSpearAttackCount;
        harpoonAttackCount = maxHarpoonAttackCount;
    }

    void Update()
    {
        // 마우스 좌클릭 : 창 공격
        if (Input.GetMouseButtonDown(0) && spearAttackCount > 0)
        {
            SpearAttack();
        }

        // 마우스 우클릭 : 작살 공격
        if (Input.GetMouseButtonDown(1) && harpoonAttackCount > 0)
        {
            HarpoonAttack();
        }
    }

    private void SpearAttack()
    {
        spearAttackCount--;
        FireSpear();
    }

    private void HarpoonAttack()
    {
        harpoonAttackCount--;
        FireHarpoon();
    }

    private void FireSpear()
    {
        Debug.Log("찌르개살 공격");
        GameObject spearObj = Instantiate(spear, transform.position, Quaternion.Euler(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        spearObj.GetComponent<Spear>().targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FireHarpoon()
    {
        Debug.Log("작살 공격");
        GameObject harpoonObj = Instantiate(harpoon, transform.position, Quaternion.Euler(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        harpoonObj.GetComponent<Harpoon>().targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void ReloadSpear()
    {
        spearAttackCount = maxSpearAttackCount;
    }

    public void ReloadHarpoon()
    {
        harpoonAttackCount = maxHarpoonAttackCount;
    }
}
