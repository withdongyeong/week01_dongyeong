using System.Collections.Generic;
using UnityEngine;
public class Whale : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth = 50;
    public List<GameObject> parts = new List<GameObject>();

    public bool isExistTail = true;
    public bool isExistBody = true;
    public bool isExistHead = true;

    void Start()
    {
        currentHealth = maxHealth;
        foreach(Transform part in transform)
        {
            parts.Add(part.gameObject);
        }
    }

    void TakeDamage(int attack = 1)
    {
        if (currentHealth > 0)
        {
            float floatHealth = (float)currentHealth;
            floatHealth /= maxHealth;

            isExistTail = (floatHealth >= 0.8f);
            isExistBody = (floatHealth >= 0.4f);
            isExistHead = (floatHealth >= 0f);

            if ( (!isExistTail || !isExistBody || !isExistHead) && parts.Count > 0 )
            {
                parts[parts.Count - 1].gameObject.SetActive(false);
                parts.Remove(parts[parts.Count - 1]);
            }

            currentHealth -= attack;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(5);
        }
    }
}