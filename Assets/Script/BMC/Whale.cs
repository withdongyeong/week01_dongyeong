using System.Collections.Generic;
using UnityEngine;
public class Whale : MonoBehaviour
{
    public int health = 3;
    public List<GameObject> parts = new List<GameObject>();
    
    void Start()
    {
        foreach(Transform part in transform)
        {
            parts.Add(part.gameObject);
        }
    }

    void TakeDamage(int attack = 1)
    {
        if (health > 0)
        {
            health -= attack;
            parts[parts.Count - 1].gameObject.SetActive(false);
            parts.Remove(parts[parts.Count - 1]);
            health--;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage();
        }
    }
}