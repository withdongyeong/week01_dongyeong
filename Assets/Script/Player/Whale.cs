using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Whale : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth = 50;
    public List<GameObject> parts = new List<GameObject>();

    public bool isExistTail = true;
    public bool isExistBody = true;
    public bool isExistHead = true;

    public bool isTakeDamaged = false;
    public float unattackTimer = 0f; 
    public float unattackTime = 0.75f;

    void Start()
    {
        currentHealth = maxHealth;
        foreach(Transform part in transform)
        {
            parts.Add(part.gameObject);
        }
    }

    void Update()
    {
        if(isTakeDamaged)   // ������ �޾�����
        {
            unattackTimer += Time.deltaTime;    // �����ð� ����
            // Debug.Log("���� ����");

            if(unattackTime < unattackTimer) 
            {
                isTakeDamaged = false;
            }
        }
       
    }

    public void TakeDamage(int attack = 1)
    {
        
        // ���� ü��
        currentHealth = Mathf.Clamp(currentHealth - attack, 0, maxHealth);
        if(currentHealth > 0)
        {
            unattackTimer += Time.deltaTime;

            float floatHealth = (float)currentHealth;
            floatHealth /= maxHealth;

            isExistTail = (floatHealth >= 0.74f);
            isExistBody = (floatHealth >= 0.48f);
            isExistHead = (floatHealth >= 0f);

            if (isExistTail && floatHealth <= 0.74f)
            {
                isExistTail = false;
                DisablePart();
            }
            else if (isExistBody && floatHealth <= 0.48f)
            {
                isExistBody = false;
                DisablePart();
            }
            else if (isExistHead && floatHealth <= 0f)
            {
                isExistHead = false;
                DisablePart();
            }
        }
        else if (!GameManager.Instance.isGameOver)
        {
            Transform chainTransform = transform.parent.Find("Chain");
            chainTransform.gameObject.SetActive(false);

            unattackTimer = 0;
            isTakeDamaged = false;

            GameManager.Instance.GameOver();
        }
    }

    public void DisablePart()
    {
        parts[parts.Count - 1].gameObject.SetActive(false);
        parts.Remove(parts[parts.Count - 1]);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            SharkMove shark = collision.gameObject.GetComponent<SharkMove>();

            if(shark != null && currentHealth > 0) 
            {
                if (!isTakeDamaged)
                {
                    unattackTimer = 0;
                    isTakeDamaged = true;
                    shark.EatWhale();
                    TakeDamage(13);
                }
            }
        }
    }
}