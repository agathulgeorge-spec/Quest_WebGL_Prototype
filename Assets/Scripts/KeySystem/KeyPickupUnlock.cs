using UnityEngine;
using System.Collections;

public class KeyPickupUnlock : MonoBehaviour
{
    // =========================================
    // GLOBAL KEY UNLOCK
    // =========================================

    public static bool KeyUnlocked = false;

    [Header("Normal Door")]
    public GameObject lockedBlock;

    [Header("Effects")]
    public ParticleSystem burstEffect;

    [Header("Crystal Activation")]
    public LightCrystal[] crystalsToActivate;

    [Header("Area Lights")]
    public LightManager lightManager;

    [Header("Final Door (Optional)")]
    public DoorUnlockManager doorManager;

    [Tooltip("1 = Key1, 2 = Key2, 3 = Key3")]
    public int keyID = 1;

    [Header("Pickup Sound")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    [Range(0f, 1f)]
    public float pickupSoundVolume = 0.8f;

    private SpriteRenderer sr;
    private bool collected;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // KEY HAS BEEN UNLOCKED
            KeyUnlocked = true;

            // Play pickup sound
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(
                    pickupSound,
                    pickupSoundVolume
                );
            }

            StartCoroutine(CollectRoutine());
        }
    }

    private IEnumerator CollectRoutine()
    {
        float t = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.5f;

        while (t < 0.2f)
        {
            t += Time.deltaTime;

            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    endScale,
                    t / 0.2f
                );

            if (sr != null)
            {
                sr.color =
                    Color.Lerp(
                        Color.white,
                        new Color(2f, 2f, 2f, 1f),
                        t / 0.2f
                    );
            }

            yield return null;
        }

        // Play burst effect
        if (burstEffect != null)
            Instantiate(
                burstEffect,
                transform.position,
                Quaternion.identity
            );

        // Open normal locked block
        if (lockedBlock != null)
            lockedBlock.SetActive(false);

        // Activate crystals
        foreach (LightCrystal crystal in crystalsToActivate)
        {
            if (crystal != null)
                crystal.Activate();
        }

        // Activate area lights
        if (lightManager != null)
            lightManager.ActivateLights();

        // Notify DoorUnlockManager
        if (doorManager != null)
        {
            switch (keyID)
            {
                case 2:
                    doorManager.CollectKey2();
                    break;

                case 3:
                    doorManager.CollectKey3();
                    break;
            }
        }

        Destroy(gameObject);
    }
}