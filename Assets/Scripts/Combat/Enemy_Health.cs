using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    // Public values used by Health HUD
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
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
            " Died"
        );

        EnemyDeathEffect deathEffect =
            GetComponent<EnemyDeathEffect>();

        if (deathEffect != null)
        {
            deathEffect.StartDeathEffect();
            return;
        }

        // Safety fallback
        Destroy(gameObject);
    }

    // Optional healing
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

    // Optional health reset
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }
}