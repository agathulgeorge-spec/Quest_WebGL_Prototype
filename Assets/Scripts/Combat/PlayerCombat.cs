using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Sword")]
    public bool hasSword = false;

    [Header("Combo")]
    public float comboResetTime = 1f;

    [Header("Heavy Attack")]
    public float heavyHoldTime = 0.5f;

    private int attackIndex = 0;

    private float comboTimer = 0f;
    private float holdTimer = 0f;

    private bool heavyAttackTriggered = false;


    void Update()
    {
        // Player cannot attack until sword is collected
        if (!hasSword)
            return;

        HandleAttackInput();
        UpdateComboTimer();
    }


    void HandleAttackInput()
    {
        // F was pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            holdTimer = 0f;
            heavyAttackTriggered = false;
        }


        // F is being held
        if (Input.GetKey(KeyCode.F))
        {
            holdTimer += Time.deltaTime;

            // Heavy attack
            if (holdTimer >= heavyHoldTime && !heavyAttackTriggered)
            {
                PerformHeavyAttack();
            }
        }


        // F was released
        if (Input.GetKeyUp(KeyCode.F))
        {
            // If it wasn't a heavy attack,
            // treat it as a light attack
            if (!heavyAttackTriggered)
            {
                PerformLightAttack();
            }
        }
    }


    void PerformLightAttack()
    {
        attackIndex++;


        // First attack
        if (attackIndex == 1)
        {
            animator.SetTrigger("Attack1");
        }


        // Second attack
        else if (attackIndex == 2)
        {
            animator.SetTrigger("Attack2");
        }


        // Third attack
        else if (attackIndex == 3)
        {
            animator.SetTrigger("Attack3");

            // Combo is complete
            // Next press starts from Attack1
            attackIndex = 0;

            comboTimer = 0f;

            return;
        }


        // Start/reset combo timer
        comboTimer = comboResetTime;
    }


    void PerformHeavyAttack()
    {
        heavyAttackTriggered = true;

        // Heavy attack uses Attack3 animation
        animator.SetTrigger("Attack3");

        // Heavy attack resets combo
        attackIndex = 0;

        comboTimer = 0f;
    }


    void UpdateComboTimer()
    {
        if (attackIndex > 0)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0f)
            {
                attackIndex = 0;
            }
        }
    }
}