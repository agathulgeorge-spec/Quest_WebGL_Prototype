using UnityEngine;

public class PlayerCombatLock : MonoBehaviour
{
    [Header("Target Detection")]
    public float detectionRange = 3f;

    [Header("Attack Distance")]
    public float attackDistance = 1.4f;

    [Header("Lock Movement")]
    public float lockSpeed = 12f;

    [Header("Input")]
    public KeyCode attackKey = KeyCode.F;

    [Header("References")]
    public Transform attackPoint;

    private Transform currentEnemy;
    private SpriteRenderer spriteRenderer;

    private bool isLocking;
    private float targetX;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            FindEnemyAndAttack();
        }

        if (isLocking)
        {
            SmoothLockMovement();
        }
    }

    private void FindEnemyAndAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            detectionRange
        );

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            Enemy_Health enemy = hit.GetComponent<Enemy_Health>();

            if (enemy == null)
            {
                enemy = hit.GetComponentInParent<Enemy_Health>();
            }

            if (enemy != null)
            {
                float distance = Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }

        if (closestEnemy == null)
        {
            Debug.Log("No enemy nearby.");
            return;
        }

        currentEnemy = closestEnemy;

        CalculateAttackPosition();

        FaceEnemy();

        isLocking = true;

        Debug.Log("Smooth attack lock started.");
    }

    private void CalculateAttackPosition()
    {
        float enemyX = currentEnemy.position.x;

        float direction;

        if (transform.position.x < enemyX)
        {
            // Player is LEFT of enemy
            direction = -1f;
        }
        else
        {
            // Player is RIGHT of enemy
            direction = 1f;
        }

        targetX = enemyX + (direction * attackDistance);
    }

    private void SmoothLockMovement()
    {
        Vector3 currentPosition = transform.position;

        float newX = Mathf.Lerp(
            currentPosition.x,
            targetX,
            lockSpeed * Time.deltaTime
        );

        currentPosition.x = newX;

        // Keep player's height exactly the same
        currentPosition.y = transform.position.y;

        transform.position = currentPosition;

        // Stop when close enough
        if (Mathf.Abs(transform.position.x - targetX) < 0.02f)
        {
            currentPosition.x = targetX;
            transform.position = currentPosition;

            isLocking = false;
        }
    }

    private void FaceEnemy()
    {
        if (currentEnemy == null || spriteRenderer == null)
            return;

        bool enemyIsRight =
            currentEnemy.position.x > transform.position.x;

        spriteRenderer.flipX = !enemyIsRight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}