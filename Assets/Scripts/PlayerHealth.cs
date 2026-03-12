using UnityEngine;

// Tracks the player's health and handles death
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Reduces health by the given amount
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Takes damage from enemy projectiles
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            TakeDamage(1);
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        GameManager.Instance.OnPlayerDeath();
    }
}
