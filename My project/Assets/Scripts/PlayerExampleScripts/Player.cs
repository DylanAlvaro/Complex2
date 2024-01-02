using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float maxHealth = 100f;
    private float currentHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("damage taken" + damage);

        if (currentHealth <= 0f)
            Die();
            
    }

    private void Die()
    {
        Debug.Log("player dead");
    }
}
