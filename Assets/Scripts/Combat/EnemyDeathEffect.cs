using UnityEngine;
using System.Collections;

public class EnemyDeathEffect : MonoBehaviour
{
    [Header("Disintegration")]
    public float disintegrationTime = 0.4f;

    [Header("Light Particles")]
    public GameObject lightParticlePrefab;

    [Header("Particle Amount")]
    public int minimumParticles = 3;
    public int maximumParticles = 6;

    [Header("Spawn Area")]
    public float spawnRadius = 0.3f;

    [Header("Death Sound")]
    public AudioSource audioSource;
    public AudioClip enemyDeathSound;

    [Range(0f, 1f)]
    public float deathSoundVolume = 0.8f;

    private SpriteRenderer[] spriteRenderers;
    private bool isDying = false;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void StartDeathEffect()
    {
        if (isDying)
            return;

        isDying = true;

        // Play death sound
        if (audioSource != null && enemyDeathSound != null)
        {
            audioSource.PlayOneShot(
                enemyDeathSound,
                deathSoundVolume
            );
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Spawn 3–6 light particles
        SpawnLightParticles();

        float timer = 0f;

        while (timer < disintegrationTime)
        {
            timer += Time.deltaTime;

            float progress = timer / disintegrationTime;

            foreach (SpriteRenderer sprite in spriteRenderers)
            {
                if (sprite == null)
                    continue;

                Color color = sprite.color;

                color.a = Mathf.Lerp(1f, 0f, progress);

                sprite.color = color;
            }

            yield return null;
        }

        // Destroy enemy after disintegration
        Destroy(gameObject);
    }

    private void SpawnLightParticles()
    {
        if (lightParticlePrefab == null)
        {
            Debug.LogError("Light Particle Prefab is NOT assigned!");
            return;
        }

        int amount = Random.Range(
            minimumParticles,
            maximumParticles + 1
        );

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPosition = transform.position;

            spawnPosition.x += Random.Range(
                -spawnRadius,
                spawnRadius
            );

            spawnPosition.y += Random.Range(
                0f,
                spawnRadius
            );

            Instantiate(
                lightParticlePrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }
}