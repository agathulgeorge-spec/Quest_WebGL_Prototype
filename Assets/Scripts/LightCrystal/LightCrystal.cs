using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightCrystal : MonoBehaviour
{
    public Color dimColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    public Color activeColor = Color.white;

    public float activeLightIntensity = 2f;
    public float activeLightRadius = 3f;
    public float activationSpeed = 2f;

    private SpriteRenderer sr;
    private Light2D light2D;
    private bool activated;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();

        if (sr != null)
            sr.color = dimColor;

        if (light2D != null)
        {
            light2D.intensity = 0f;
            light2D.pointLightOuterRadius = activeLightRadius;
        }
    }

    public void Activate()
    {
        if (activated) return;

        activated = true;
        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        float t = 0f;
        Color startColor = sr.color;

        while (t < 1f)
        {
            t += Time.deltaTime * activationSpeed;

            if (sr != null)
                sr.color = Color.Lerp(startColor, activeColor, t);

            if (light2D != null)
                light2D.intensity = Mathf.Lerp(0f, activeLightIntensity, t);

            yield return null;
        }
    }
}