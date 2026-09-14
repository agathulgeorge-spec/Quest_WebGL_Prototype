using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightManager : MonoBehaviour
{
    [Header("Area Lights")]
    public Light2D[] areaLights;
    public float targetIntensity = 1.5f;
    public float fadeDuration = 3f;

    [Header("Player Light")]
    public Light2D playerLight;
    public float playerLightOffIntensity = 0f;

    private bool activated = false;

    void Start()
    {
        foreach (Light2D l in areaLights)
        {
            if (l != null)
            {
                l.gameObject.SetActive(true);
                l.intensity = 0f;
            }
        }
    }

    public void ActivateLights()
    {
        if (activated) return;

        activated = true;
        StartCoroutine(FadeAreaLightsAndPlayerLight());
    }

    IEnumerator FadeAreaLightsAndPlayerLight()
    {
        float timer = 0f;
        float startPlayerIntensity = playerLight != null ? playerLight.intensity : 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            foreach (Light2D l in areaLights)
            {
                if (l != null)
                    l.intensity = Mathf.Lerp(0f, targetIntensity, t);
            }

            if (playerLight != null)
                playerLight.intensity = Mathf.Lerp(startPlayerIntensity, playerLightOffIntensity, t);

            yield return null;
        }

        foreach (Light2D l in areaLights)
        {
            if (l != null)
                l.intensity = targetIntensity;
        }

        if (playerLight != null)
            playerLight.intensity = playerLightOffIntensity;
    }
}