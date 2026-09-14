using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HeroController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 13f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Ground Stick")]
    public float groundStickForce = -2f;

    [Header("Orb Jump")]
    public bool canOrbJump;
    public float orbJumpForce = 13f;

    [Header("Beacon Control")]
    public BeaconController currentBeacon;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;


    // =========================
    // ATTACK SYSTEM
    // =========================

    private bool isAttacking = false;

    // 1 = attack 1
    // 2 = attack 2
    // 3 = attack 3
    private int currentAttack = 0;

    private int queuedAttack = 0;

    private bool attackFinished = false;


    // =========================
    // ROLL SYSTEM
    // =========================

    [Header("Roll")]
    public float rollSpeed = 10f;
    public float rollDuration = 0.5f;

    private bool isRolling = false;
    private float rollTimer = 0f;
    private float rollDirection = 1f;


    // =========================
    // ATTACK DIRECTION LOCK
    // =========================

    private bool attackFacingRight;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }


    private void Update()
    {
        // =====================================================
        // MOVEMENT INPUT
        // =====================================================

        // PC A / D
        moveInput = Input.GetAxisRaw("Horizontal");

        // MOBILE LEFT / RIGHT
        if (MobileInput.leftHeld)
            moveInput = -1f;

        if (MobileInput.rightHeld)
            moveInput = 1f;


        // =====================================================
        // ATTACK / ROLL
        // =====================================================

        CheckAttackFinished();
        HandleAttackInput();
        HandleRollInput();
        UpdateRoll();


        // =====================================================
        // JUMP
        // =====================================================

        if (!isAttacking &&
            !isRolling &&
            (Input.GetButtonDown("Jump") ||
             MobileInput.jumpPressed))
        {
            jumpPressed = true;

            MobileInput.ResetJump();
        }


        // =====================================================
        // BEACON
        // =====================================================

        HandleBeaconInput();


        // =====================================================
        // ANIMATION
        // =====================================================

        UpdateAnimation();
        FlipSprite();
    }


    private void FixedUpdate()
    {
        CheckGround();
        Move();
        StickToGround();
        Jump();
    }


    // =========================================================
    // ATTACK INPUT
    // =========================================================

    private void HandleAttackInput()
    {
        bool attackInput =
            Input.GetKeyDown(KeyCode.F) ||
            MobileInput.attackPressed;


        if (!attackInput)
            return;


        MobileInput.ResetAttack();


        if (animator == null)
            return;


        // START ATTACK 1
        if (!isAttacking)
        {
            StartAttack(1);
            return;
        }


        // QUEUE ATTACK 2
        if (currentAttack == 1)
        {
            queuedAttack = 2;
            return;
        }


        // QUEUE ATTACK 3
        if (currentAttack == 2)
        {
            queuedAttack = 3;
            return;
        }


        // ATTACK 3 -> ATTACK 1
        if (currentAttack == 3)
        {
            queuedAttack = 1;
            return;
        }
    }


    // =========================================================
    // ROLL INPUT
    // =========================================================

    private void HandleRollInput()
    {
        bool rollInput =
            Input.GetKeyDown(KeyCode.LeftControl) ||
            MobileInput.rollPressed;


        if (!rollInput)
            return;


        MobileInput.ResetRoll();


        if (isAttacking ||
            isRolling ||
            !isGrounded)
            return;


        StartRoll();
    }


    // =========================================================
    // START ROLL
    // =========================================================

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = 0f;
        jumpPressed = false;


        rollDirection =
            spriteRenderer.flipX ? -1f : 1f;


        if (animator != null)
        {
            animator.Play(
                "Base Layer.Roll",
                0,
                0f
            );
        }
    }


    private void UpdateRoll()
    {
        if (!isRolling)
            return;


        rollTimer += Time.deltaTime;


        if (rollTimer >= rollDuration)
        {
            EndRoll();
        }
    }


    private void EndRoll()
    {
        isRolling = false;
        rollTimer = 0f;
    }


    // =========================================================
    // START ATTACK
    // =========================================================

    private void StartAttack(int attackNumber)
    {
        isAttacking = true;
        currentAttack = attackNumber;
        attackFinished = false;


        // LOCK FACING
        attackFacingRight =
            !spriteRenderer.flipX;


        // STOP HORIZONTAL MOVEMENT
        rb.linearVelocity =
            new Vector2(
                0f,
                rb.linearVelocity.y
            );


        string stateName = "";


        if (attackNumber == 1)
        {
            stateName =
                "Base Layer.attack 1";
        }
        else if (attackNumber == 2)
        {
            stateName =
                "Base Layer.attack 2";
        }
        else if (attackNumber == 3)
        {
            stateName =
                "Base Layer.attack 3";
        }


        animator.Play(
            stateName,
            0,
            0f
        );
    }


    // =========================================================
    // CHECK ATTACK COMPLETION
    // =========================================================

    private void CheckAttackFinished()
    {
        if (!isAttacking)
            return;


        if (animator == null)
            return;


        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);


        bool correctState = false;


        if (currentAttack == 1)
        {
            correctState =
                state.IsName("attack 1");
        }
        else if (currentAttack == 2)
        {
            correctState =
                state.IsName("attack 2");
        }
        else if (currentAttack == 3)
        {
            correctState =
                state.IsName("attack 3");
        }


        if (!correctState)
            return;


        if (state.normalizedTime >= 1f &&
            !attackFinished)
        {
            attackFinished = true;


            if (queuedAttack != 0)
            {
                int nextAttack =
                    queuedAttack;


                queuedAttack = 0;


                StartAttack(nextAttack);


                return;
            }


            EndAttack();
        }
    }


    // =========================================================
    // END ATTACK
    // =========================================================

    private void EndAttack()
    {
        isAttacking = false;
        currentAttack = 0;
        queuedAttack = 0;
        attackFinished = false;


        animator.Play(
            "Base Layer.Idle",
            0,
            0f
        );
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void Move()
    {
        // ATTACK
        if (isAttacking)
        {
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );

            return;
        }


        // ROLL
        if (isRolling)
        {
            rb.linearVelocity =
                new Vector2(
                    rollDirection * rollSpeed,
                    rb.linearVelocity.y
                );

            return;
        }


        // NORMAL MOVEMENT
        rb.linearVelocity =
            new Vector2(
                moveInput * moveSpeed,
                rb.linearVelocity.y
            );
    }


    // =========================================================
    // GROUND CHECK
    // =========================================================

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }


        isGrounded =
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
    }


    // =========================================================
    // GROUND STICK
    // =========================================================

    private void StickToGround()
    {
        if (isGrounded &&
            rb.linearVelocity.y <= 0f)
        {
            rb.linearVelocity =
                new Vector2(
                    rb.linearVelocity.x,
                    groundStickForce
                );
        }
    }


    // =========================================================
    // JUMP
    // =========================================================

    private void Jump()
    {
        if (isRolling)
        {
            jumpPressed = false;
            return;
        }


        if (!jumpPressed)
            return;


        if (isGrounded)
        {
            rb.linearVelocity =
                new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );
        }
        else if (canOrbJump)
        {
            rb.linearVelocity =
                new Vector2(
                    rb.linearVelocity.x,
                    orbJumpForce
                );


            canOrbJump = false;
        }


        jumpPressed = false;
    }


    // =========================================================
    // ANIMATION
    // =========================================================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;


        if (isAttacking ||
            isRolling)
            return;


        float speed =
            Mathf.Abs(moveInput);


        animator.SetBool(
            "isRunning",
            speed > 0.1f &&
            isGrounded
        );


        animator.SetBool(
            "isGrounded",
            isGrounded
        );


        animator.SetFloat(
            "yVelocity",
            rb.linearVelocity.y
        );
    }


    // =========================================================
    // SPRITE DIRECTION
    // =========================================================

    private void FlipSprite()
    {
        if (spriteRenderer == null)
            return;


        // ATTACK
        if (isAttacking)
        {
            spriteRenderer.flipX =
                !attackFacingRight;

            return;
        }


        // ROLL
        if (isRolling)
        {
            spriteRenderer.flipX =
                rollDirection < 0f;

            return;
        }


        // NORMAL MOVEMENT
        if (moveInput > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }


    // =========================================================
    // BEACON
    // =========================================================

    private void HandleBeaconInput()
    {
        if (currentBeacon == null)
            return;


        // =====================================================
        // E
        // PC E OR MOBILE E
        // =====================================================

        bool ePressed =
            Input.GetKey(KeyCode.E) ||
            MobileInput.eHeld;


        // =====================================================
        // G
        // =====================================================

        bool gPressed =
            Input.GetKey(KeyCode.G);


        // =====================================================
        // BEACON DIRECTION
        // =====================================================

        Vector2 directionInput =
            Vector2.zero;


        // -----------------------------------------------------
        // PC I / J / K / L
        // -----------------------------------------------------

        // I = UP
        if (Input.GetKey(KeyCode.I))
            directionInput.y += 1f;


        // K = DOWN
        if (Input.GetKey(KeyCode.K))
            directionInput.y -= 1f;


        // J = LEFT
        if (Input.GetKey(KeyCode.J))
            directionInput.x -= 1f;


        // L = RIGHT
        if (Input.GetKey(KeyCode.L))
            directionInput.x += 1f;


        // -----------------------------------------------------
        // MOBILE JOYSTICK
        // -----------------------------------------------------

        // Only allow the joystick to control
        // the beacon while E is being held.
        if (MobileInput.eHeld)
        {
            if (MobileInput.beaconDirection.magnitude > 0.1f)
            {
                directionInput =
                    MobileInput.beaconDirection;
            }
        }


        // =====================================================
        // SEND INPUT TO BEACON
        // =====================================================

        currentBeacon.HandleHeroInput(
            ePressed,
            gPressed,
            directionInput
        );
    }


    // =========================================================
    // BEACON COLLISION
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        BeaconController beacon =
            other.GetComponent<BeaconController>();


        if (beacon != null)
        {
            currentBeacon = beacon;


            currentBeacon.SetHeroControl(
                true,
                transform
            );
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        BeaconController beacon =
            other.GetComponent<BeaconController>();


        if (beacon != null &&
            beacon == currentBeacon)
        {
            currentBeacon.SetHeroControl(
                false,
                transform
            );


            currentBeacon = null;
        }
    }


    // =========================================================
    // ORB JUMP
    // =========================================================

    public void DoOrbJump(
        float customOrbJumpForce)
    {
        rb.linearVelocity =
            new Vector2(
                rb.linearVelocity.x,
                customOrbJumpForce
            );


        canOrbJump = false;
    }


    public void SetCanOrbJump(bool value)
    {
        canOrbJump = value;
    }
}