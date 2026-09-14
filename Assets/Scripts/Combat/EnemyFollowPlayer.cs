using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Facing")]
    public bool flipX = true;

    [Header("Combat Turn")]
    public float turnDelay = 0.5f;

    private SpriteRenderer spriteRenderer;

    private bool facingRight;
    private float turnTimer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Set this according to the enemy's starting direction.
            facingRight = !spriteRenderer.flipX;
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null || spriteRenderer == null)
            return;

        FacePlayer();
    }

    private void FacePlayer()
    {
        bool playerIsRight = player.position.x > transform.position.x;

        // Player is already in front of the enemy.
        if (playerIsRight == facingRight)
        {
            turnTimer = 0f;
            return;
        }

        // Player has moved behind the enemy.
        turnTimer += Time.deltaTime;

        // Give the player time to attack from behind.
        if (turnTimer >= turnDelay)
        {
            facingRight = playerIsRight;

            if (facingRight)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            turnTimer = 0f;
        }
    }
}