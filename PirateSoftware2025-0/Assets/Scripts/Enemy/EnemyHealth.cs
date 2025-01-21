using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public GameObject deathParticlesPrefab; // Prefab de part�culas de muerte

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Instanciar el prefab de part�culas de muerte
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }

        // Aqu� puedes a�adir cualquier l�gica adicional que necesites cuando el enemigo muere
        Destroy(gameObject); // Destruye el enemigo inmediatamente
    }
}
