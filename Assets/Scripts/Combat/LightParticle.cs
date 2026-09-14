using UnityEngine;

public class LightParticle : MonoBehaviour
{
    [Header("Very Tiny Movement")]
    public float minUpwardVelocity = 0.005f;
    public float maxUpwardVelocity = 0.015f;

    public float minHorizontalVelocity = 0.005f;
    public float maxHorizontalVelocity = 0.02f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (rb == null)
            return;

        float upward = Random.Range(
            minUpwardVelocity,
            maxUpwardVelocity
        );

        float horizontal = Random.Range(
            minHorizontalVelocity,
            maxHorizontalVelocity
        );

        if (Random.value < 0.5f)
            horizontal *= -1f;

        rb.linearVelocity = new Vector2(
            horizontal,
            upward
        );
    }
}