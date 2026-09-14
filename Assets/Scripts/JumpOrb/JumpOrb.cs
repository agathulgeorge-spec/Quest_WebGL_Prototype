using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class JumpOrb : MonoBehaviour
{
    [Header("Orb Settings")]
    public float orbJumpForce = 13f;

    [Header("Activation")]
    [Tooltip("If enabled, this specific orb will always remain active.")]
    public bool alwaysActive = false;

    [Header("Visual")]
    public SpriteRenderer orbRenderer;
    public Color unlitColor = Color.gray;
    public Color litColor = Color.white;

    [Header("Activation Sound")]
    public AudioSource audioSource;
    public AudioClip activationSound;

    [Range(0f, 1f)]
    public float activationSoundVolume = 0.8f;

    [Tooltip("Orb must remain OFF for this long before the activation sound can play again.")]
    public float activationSoundResetTime = 1f;

    [Header("Jump Sound")]
    public AudioClip jumpSound;

    [Range(0f, 1f)]
    public float jumpSoundVolume = 0.8f;

    private bool isLit;
    private bool playerInside;
    private HeroController hero;

    // Tracks when the orb was last turned OFF
    private float lastTurnOffTime = -Mathf.Infinity;

    private void Awake()
    {
        if (orbRenderer == null)
            orbRenderer = GetComponent<SpriteRenderer>();

        SetLit(alwaysActive);
    }

    private void Update()
    {
        // Keep the selected Always Active orb active
        if (alwaysActive && !isLit)
        {
            SetLit(true);
        }

        if (!isLit) return;
        if (!playerInside) return;
        if (hero == null) return;

        if (Input.GetButtonDown("Jump"))
        {
            // Play jump sound
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(
                    jumpSound,
                    jumpSoundVolume
                );
            }

            hero.DoOrbJump(orbJumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HeroController foundHero = other.GetComponent<HeroController>();

        if (foundHero != null)
        {
            hero = foundHero;
            playerInside = true;

            hero.SetCanOrbJump(isLit);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        HeroController foundHero = other.GetComponent<HeroController>();

        if (foundHero != null && foundHero == hero)
        {
            playerInside = false;
            hero.SetCanOrbJump(false);
            hero = null;
        }
    }

    // Called by BeaconController when the light ray hits this orb
    public void SetLitByBeacon(BeaconController beacon)
    {
        SetLit(true);
    }

    // Called by BeaconController when the light ray stops hitting this orb
    public void RemoveBeacon(BeaconController beacon)
    {
        // Do not turn off the selected Always Active orb
        if (alwaysActive)
        {
            SetLit(true);
            return;
        }

        SetLit(false);
    }

    public void SetLit(bool value)
    {
        // Always Active overrides attempts to turn it off
        if (alwaysActive)
        {
            value = true;
        }

        bool wasLit = isLit;

        isLit = value;

        if (orbRenderer != null)
        {
            orbRenderer.color = isLit ? litColor : unlitColor;
        }

        // Orb just turned OFF
        if (!isLit && wasLit)
        {
            lastTurnOffTime = Time.time;
        }

        // Orb just turned ON
        if (isLit && !wasLit)
        {
            // Only play activation sound if:
            // 1. It has been OFF for at least 1 second
            // 2. OR this is the first activation
            if (Time.time - lastTurnOffTime >= activationSoundResetTime)
            {
                if (audioSource != null && activationSound != null)
                {
                    audioSource.PlayOneShot(
                        activationSound,
                        activationSoundVolume
                    );
                }
            }
        }

        if (hero != null)
        {
            hero.SetCanOrbJump(isLit && playerInside);
        }
    }

    public bool IsLit()
    {
        return isLit;
    }
}