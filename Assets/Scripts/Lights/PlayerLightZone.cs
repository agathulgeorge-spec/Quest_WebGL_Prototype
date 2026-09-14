using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerLightZone : MonoBehaviour
{
    public Light2D playerLight;
    public float targetIntensity = 1.5f;
    public float fadeDuration = 1f;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            used = true;
            StartCoroutine(FadePlayerLightOn());
        }
    }

    IEnumerator FadePlayerLightOn()
    {
        float timer = 0f;
        float startIntensity = playerLight.intensity;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            playerLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null;
        }

        playerLight.intensity = targetIntensity;
    }
}