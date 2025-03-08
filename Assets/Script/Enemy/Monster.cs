using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public int maxHealth = 100;    // 몬스터의 최대 체력
    private int currentHealth;     // 현재 체력
    private bool isDead = false;   // 몬스터의 생존 여부

    public GameObject bloodParticlePrefab;
    private SharkMove sharkMove;   // 🦈 상어 움직임 컨트롤러
    private TrailRenderer trailRenderer; // 🛑 Trail Renderer 추가

    void Start()
    {
        currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
        sharkMove = GetComponent<SharkMove>(); // 🦈 상어의 움직임 컨트롤러 가져오기
        
        // 🛑 하위 오브젝트에서 Trail Renderer 찾기
        Transform fin = transform.Find("Fin/GameObject"); // 경로 지정
        if (fin != null)
        {
            trailRenderer = fin.GetComponent<TrailRenderer>();
        }
    }

    // 데미지를 입는 함수
    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽은 상태면 데미지 무효

        currentHealth -= damage;
        // Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 체력이 0 이하가 되었을 때 호출되는 함수
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // 🛑 Trail Renderer 끄기
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        // 🛑 상어가 죽었음을 작살에게 알림 (즉시 돌아오도록)
        Harpoon harpoon = GetComponentInChildren<Harpoon>();
        if (harpoon != null)
        {
            harpoon.ForceReturn();
        }

        if (bloodParticlePrefab != null)
        {
            GameObject bloodEffect = Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity, transform);
            ParticleSystem particle = bloodEffect.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Play();
                Destroy(bloodEffect, particle.main.duration);
            }
        }

        if (sharkMove != null)
        {
            sharkMove.SetDeadState();
        }

        GetComponent<Animator>().SetTrigger("Die");
        StartCoroutine(DestroyAfterAnimation());
    }

    // 애니메이션이 끝난 후 자동으로 몬스터 삭제
    private IEnumerator DestroyAfterAnimation()
    {
        Animator animator = GetComponent<Animator>();

        // 현재 재생 중인 애니메이션의 길이를 가져와 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject); // 애니메이션이 끝나면 몬스터 삭제
    }
}
