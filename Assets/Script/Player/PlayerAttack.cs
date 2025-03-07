using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bullet;
    
    private int maxSpearAttackCount = 1;
    private int maxHarpoonAttackCount = 1;
    
    private int spearAttackCount;
    private int harpoonAttackCount;

    public CameraController cameraController;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        maxSpearAttackCount += StateManager.Instance.SpearCount;
        spearAttackCount = maxSpearAttackCount;
        harpoonAttackCount = maxHarpoonAttackCount;
    }

    void Update()
    {
        // 마우스 좌클릭 : 찌르개살 공격
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
        FireProjectile();
    }

    private void HarpoonAttack()
    {
        harpoonAttackCount--;
        FireProjectile();
    }

    private void FireProjectile()
    {
        GameObject bulletObj = Instantiate(bullet, transform.position, Quaternion.Euler(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        bulletObj.GetComponent<Harpoon>().targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (cameraController != null)
            StartCoroutine(cameraController.ShakeCamera());
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
