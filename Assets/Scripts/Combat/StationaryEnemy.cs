using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [Header("Player Detection")]
    public Transform player;
    public float attackRange = 3f;

    [Header("Attack Timing")]
    public float delayBetweenAttacks = 1.5f;

    [Header("References")]
    public Animator animator;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool wasInRange = false;

    // Prevents startup attack
    private bool startupComplete = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Clear any attack trigger that may already exist
            animator.ResetTrigger("Attack");

            // Force enemy to Idle
            animator.Play("Enemy_A_Idle", 0, 0f);
        }
    }

    private void Start()
    {
        if (player != null)
        {
            // Remember the player's starting range state.
            // If the player starts inside the range,
            // DO NOT attack immediately.
            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            wasInRange = distance <= attackRange;
        }

        attackTimer = delayBetweenAttacks;

        startupComplete = true;
    }

    private void Update()
    {
        if (player == null || animator == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        bool playerInRange = distance <= attackRange;

        // ---------------------------------------------
        // PLAYER OUTSIDE ATTACK RANGE
        // ---------------------------------------------

        if (!playerInRange)
        {
            wasInRange = false;
            isAttacking = false;
            attackTimer = 0f;

            return;
        }

        // ---------------------------------------------
        // PLAYER HAS JUST ENTERED ATTACK RANGE
        // ---------------------------------------------

        if (!wasInRange)
        {
            wasInRange = true;

            attackTimer = 0f;

            TryAttack();

            return;
        }

        // ---------------------------------------------
        // ATTACK TIMER
        // ---------------------------------------------

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        CheckAttackFinished();

        // ---------------------------------------------
        // ATTACK AGAIN
        // ---------------------------------------------

        if (!isAttacking && attackTimer <= 0f)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        // Make absolutely sure there isn't an old trigger
        animator.ResetTrigger("Attack");

        animator.SetTrigger("Attack");
    }

    private void CheckAttackFinished()
    {
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (isAttacking &&
            state.IsName("Enemy_A_Idle"))
        {
            isAttacking = false;

            attackTimer = delayBetweenAttacks;
        }
    }

    private void OnDisable()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
        }

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}