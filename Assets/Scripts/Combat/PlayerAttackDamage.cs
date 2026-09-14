using UnityEngine;

public class PlayerAttackDamage : MonoBehaviour
{
    [Header("Attack Damage")]
    public float damage = 25f;

    [Header("Attack Range")]
    public float attackRange = 1.5f;

    [Header("Attack Position")]
    public Transform attackPoint;

    [Header("Attack Point Position")]
    public float attackPointDistance = 0.8f;

    [Header("Attack Sound")]
    public AudioSource audioSource;
    public AudioClip swordAttackSound;

    [Range(0f, 1f)]
    public float swordAttackVolume = 0.8f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateAttackPoint();
    }

    private void UpdateAttackPoint()
    {
        if (attackPoint == null)
            return;

        float direction = 1f;

        // Your character's normal orientation
        // is controlled by flipX.
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            direction = -1f;
        }

        Vector3 localPosition = attackPoint.localPosition;

        localPosition.x = attackPointDistance * direction;

        attackPoint.localPosition = localPosition;
    }

    public void DealDamage()
    {
        // Play sword attack sound
        if (audioSource != null && swordAttackSound != null)
        {
            audioSource.PlayOneShot(
                swordAttackSound,
                swordAttackVolume
            );
        }

        Debug.Log("PLAYER ATTACK DAMAGE TRIGGERED");

        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is NOT assigned!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange
        );

        Debug.Log("Objects detected: " + hits.Length);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Hit: " + hit.gameObject.name);

            Enemy_Health enemyHealth =
                hit.GetComponent<Enemy_Health>();

            if (enemyHealth == null)
            {
                enemyHealth =
                    hit.GetComponentInParent<Enemy_Health>();
            }

            if (enemyHealth != null)
            {
                Debug.Log(
                    "ENEMY HIT! Dealing " +
                    damage +
                    " damage."
                );

                enemyHealth.TakeDamage(damage);
                return;
            }
        }

        Debug.Log("No enemy found in attack range.");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}