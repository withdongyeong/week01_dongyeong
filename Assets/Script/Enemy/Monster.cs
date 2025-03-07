using UnityEngine;

public class Monster : MonoBehaviour
{
    public int maxHealth = 100;    // 몬스터의 최대 체력
    private int currentHealth;     // 현재 체력

    void Start()
    {
        currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
    }

    // 데미지를 입는 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 체력이 0 이하가 되었을 때 호출되는 함수
    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        // 필요시 죽는 애니메이션, 사운드, 이펙트 등을 여기에 추가할 수 있음
        Destroy(gameObject);
    }
}
