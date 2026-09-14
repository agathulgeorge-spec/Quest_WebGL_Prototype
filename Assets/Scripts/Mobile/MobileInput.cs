using UnityEngine;

public class MobileInput : MonoBehaviour
{
    // =========================
    // MOVEMENT
    // =========================

    public static bool leftHeld;
    public static bool rightHeld;


    // =========================
    // E / INTERACT / BEACON
    // =========================

    public static bool eHeld;


    // =========================
    // BEACON JOYSTICK
    // =========================

    public static Vector2 beaconDirection;

    public static void SetBeaconDirection(Vector2 direction)
    {
        beaconDirection = direction;
    }

    public static void ResetBeaconDirection()
    {
        beaconDirection = Vector2.zero;
    }


    // =========================
    // ACTIONS
    // =========================

    public static bool jumpPressed;
    public static bool attackPressed;
    public static bool rollPressed;


    // =========================
    // LEFT
    // =========================

    public static void SetLeft(bool value)
    {
        leftHeld = value;
    }


    // =========================
    // RIGHT
    // =========================

    public static void SetRight(bool value)
    {
        rightHeld = value;
    }


    // =========================
    // E / INTERACT
    // =========================

    public static void SetE(bool value)
    {
        eHeld = value;
    }


    // =========================
    // JUMP
    // =========================

    public static void Jump()
    {
        jumpPressed = true;
    }

    public static void ResetJump()
    {
        jumpPressed = false;
    }


    // =========================
    // ATTACK
    // =========================

    public static void Attack()
    {
        attackPressed = true;
    }

    public static void ResetAttack()
    {
        attackPressed = false;
    }


    // =========================
    // ROLL
    // =========================

    public static void Roll()
    {
        rollPressed = true;
    }

    public static void ResetRoll()
    {
        rollPressed = false;
    }
}