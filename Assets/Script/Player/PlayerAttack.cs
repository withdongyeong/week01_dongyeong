using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Bbb10311031_PlayerAttack : MonoBehaviour
{
    public GameObject bullet;
    private int maxAttackCount = 1;
    public int attackCount;

    public CameraController cameraController;
    void Start() 
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        maxAttackCount += StateManager.Instance.SpearCount();
        attackCount = maxAttackCount;
    }
    void Update()
    {

        // 마우스 클릭 시 이동 시작
        if (Input.GetMouseButtonDown(0) && attackCount > 0)
        {
            AttackCountDown();
            GameObject bulletObj = Instantiate(bullet, transform.position, Quaternion.identity);
            bulletObj.GetComponent<Spear>().targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(cameraController != null)
                StartCoroutine(cameraController.ShakeCamera());
        }
    }

    public void AttackCountUp()
    {
        attackCount += 1;
    }

    public void AttackCountDown()
    {
        if (attackCount != 0)
        {
            attackCount -= 1;
        }
    }
}
