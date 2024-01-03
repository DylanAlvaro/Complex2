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
/// <summary>
/// Unused script for player damage, useful for users
/// if they want to expand upon this game for AI or hazards
/// which functionality exists for but isn't in the
/// game currently. 
/// </summary>
/// <param name="damage"></param>
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
