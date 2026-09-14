using UnityEngine;

public class Player_Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 0.5f;

    private float currentHealth;
    private bool isDead = false;

    // Automatically remembers the player's starting position
    private Vector3 startingPosition;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        // Remember the position where the player starts the scene
        startingPosition = transform.position;

        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            gameObject.name +
            " Health: " +
            currentHealth +
            " / " +
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            gameObject.name +
            " Died - Respawning..."
        );

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        // Return to the position where the player
        // originally started the scene
        transform.position = startingPosition;

        // Restore health
        currentHealth = maxHealth;

        // Allow player to move and take damage again
        isDead = false;

        Debug.Log(
            gameObject.name +
            " Respawned at starting position."
        );
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }
}