using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 50; // 최대 체력
    private float currentHealth;


    public float shakeDuration = 0.3f; // 흔들리는 지속 시간
    public float shakeMagnitude = 0.2f; // 흔들림 강도

    private Vector3 originalPosition;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "IntegrateScene")
        originalPosition = Camera.main.transform.position; 
        currentHealth = maxHealth; // 시작할 때 최대 체력 설정
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss")) // 보스와 충돌하면
        {
            TakeDamage(10); // 데미지 받기 (10)
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("플레이어 체력: " + currentHealth);
        StartCoroutine(ShakeCamera());

        //SoundManager.instance.PlaySFX("Clash");


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망!");
        GameManager.Instance.GameOver();
        Destroy(gameObject); // 플레이어 삭제
    }

    public void UpdateCurrentHP(int value)
    {
        currentHealth = value;
    }


    IEnumerator ShakeCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            Camera.main.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalPosition; // 원래 위치로 복귀
    }
    
}
