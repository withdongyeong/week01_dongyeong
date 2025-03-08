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

    private bool comboActive = false;
    private int currentComboCount = 0;
    public int maxComboCount = 5; // 연속 찌르기 스킬 발동 시 최대 공격 횟수 (인스펙터에서 조정 가능)
    public int maxComboTriggerCount = 3; // 연속 찌르기 스킬을 발동시키기 위한 적중 횟수
    public float comboDuration = 1f; // 콤보 카운트 체크 시간 (인스펙터에서 조정 가능)

    private int comboHitCount = 0; // 현재 적중 횟수
    private Coroutine comboCheckCoroutine;
    private Coroutine comboCoroutine;

    public float comboDelay = 0.2f;
    public float lastComboDelay = 0.1f;

    void Start()
    {
        maxSpearAttackCount += StateManager.Instance.SpearCount;
        spearAttackCount = maxSpearAttackCount;
        harpoonAttackCount = maxHarpoonAttackCount;
    }

    void Update()
    {
        // 콤보 중에는 새로운 공격 입력 불가
        if (comboActive)
        {
            return;
        }

        // 마우스 좌클릭 (Spear - 일반 공격)
        if (Input.GetMouseButtonDown(0) && spearAttackCount > 0)
        {
            SpearAttack();
        }

        // 마우스 우클릭 (Harpoon - 일반 공격)
        if (Input.GetMouseButtonDown(1) && harpoonAttackCount > 0)
        {
            HarpoonAttack();
        }
    }



    private void SpearAttack()
    {
        spearAttackCount--;
        Spear newSpear = FireSpear(0f).GetComponent<Spear>();
        newSpear.SetPlayerAttack(this); // Spear가 적중했을 때 PlayerAttack에 알리도록 설정
    }

    private void HarpoonAttack()
    {
        harpoonAttackCount--;
        FireHarpoon();
    }

    private GameObject FireSpear(float angleOffset)
    {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0f;

        // 마우스 방향을 기준으로 각도 계산
        Vector3 direction = (targetPos - transform.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 각도 보정 추가
        float finalAngle = baseAngle + angleOffset;

        // 최종 회전 적용
        Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);
        GameObject spearObj = Instantiate(spear, transform.position, rotation);
        
        spearObj.GetComponent<Spear>().targetPosition = targetPos;
        return spearObj;
    }


    private void FireHarpoon()
    {
        GameObject harpoonObj = Instantiate(harpoon, transform.position, Quaternion.Euler(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        harpoonObj.GetComponent<Harpoon>().targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnSpearHit()
    {
        comboHitCount++;
        // 콤보 체크 코루틴이 실행 중이 아니면 시작
        if (comboCheckCoroutine == null)
        {
            comboCheckCoroutine = StartCoroutine(ComboCheckTimer());
        }

        // 적중 횟수가 maxComboTriggerCount 이상이면 연속 찌르기 발동
        if (comboHitCount >= maxComboTriggerCount)
        {
            StartComboAttack();
        }
    }


    private IEnumerator ComboCheckTimer()
    {
        yield return new WaitForSeconds(comboDuration);
        Debug.Log("콤보 초기화");
        comboHitCount = 0; // 일정 시간 내 추가 공격이 없으면 카운트 초기화
        comboCheckCoroutine = null;
    }

    private void StartComboAttack()
    {
        if (comboActive) return; // 이미 연속 공격 중이면 실행하지 않음

        comboActive = true;
        comboHitCount = 0; // 적중 횟수 초기화
        if (comboCheckCoroutine != null)
        {
            StopCoroutine(comboCheckCoroutine);
            comboCheckCoroutine = null;
        }

        comboCoroutine = StartCoroutine(ExecuteCombo());
    }

    private IEnumerator ExecuteCombo()
    {
        Debug.Log("연속 찌르기 스킬 발동!!!");

        for (int i = 0; i < maxComboCount; i++)
        {
            yield return new WaitForSeconds(comboDelay);
            Spear newSpear = FireSpear(Random.Range(-30f, 30f)).GetComponent<Spear>();
            newSpear.SetPlayerAttack(this);
        }
        spearAttackCount = maxSpearAttackCount;
        yield return new WaitForSeconds(lastComboDelay);
        ResetCombo();
    }

    private void ResetCombo()
    {
        comboActive = false;
        currentComboCount = 0;
        comboHitCount = 0;
    }


    public void ReloadSpear()
    {
        if (!comboActive)
        {
            spearAttackCount = maxSpearAttackCount;
        }
    }

    public void ReloadHarpoon()
    {
        harpoonAttackCount = maxHarpoonAttackCount;
    }
}