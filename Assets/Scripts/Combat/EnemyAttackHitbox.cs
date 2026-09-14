using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    [Header("Attack")]
    public float damage = 10f;

    [Header("Hitbox")]
    public Collider2D attackCollider;

    [Header("Attack Sound")]
    public AudioSource audioSource;
    public AudioClip enemyAttackSound;

    [Range(0f, 1f)]
    public float attackSoundVolume = 0.8f;

    private bool canDamage = false;
    private bool alreadyDamaged = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        canDamage = false;
        alreadyDamaged = false;

        if (attackCollider == null)
        {
            attackCollider =
                GetComponentInChildren<Collider2D>();
        }

        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();
    }

    public void EnableDamage()
    {
        // Start the damage window
        canDamage = true;
        alreadyDamaged = false;

        // Play attack sound only when the attack
        // damage window is actually enabled
        if (audioSource != null &&
            enemyAttackSound != null)
        {
            audioSource.PlayOneShot(
                enemyAttackSound,
                attackSoundVolume
            );
        }
    }

    public void DisableDamage()
    {
        canDamage = false;
        alreadyDamaged = false;
    }

    private void Update()
    {
        if (!canDamage)
            return;

        if (alreadyDamaged)
            return;

        if (attackCollider == null)
            return;

        Collider2D[] hits =
            Physics2D.OverlapBoxAll(
                attackCollider.bounds.center,
                attackCollider.bounds.size,
                0f
            );

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            Player_Health health =
                hit.GetComponent<Player_Health>();

            if (health == null)
            {
                health =
                    hit.GetComponentInParent<Player_Health>();
            }

            if (health != null)
            {
                health.TakeDamage(damage);

                alreadyDamaged = true;

                return;
            }
        }
    }

    private void OnDisable()
    {
        canDamage = false;
        alreadyDamaged = false;
    }
}