using UnityEngine;

public class Water : MonoBehaviour
{
    [Header("Water Properties")]
    public float buoyancyForce = 5f;
    public float waterDrag = 2f;
    public float splashThreshold = 2f; // Minimum velocity to create splash
    
    [Header("Visual Settings")]
    public Color waterColor = new Color(0.2f, 0.6f, 1f, 0.7f);
    public Material waterMaterial;
    
    [Header("Audio Settings")]
    public AudioClip splashSound;
    public AudioClip bubbleSound;
    
    // Components
    private Collider2D waterCollider;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    // State tracking
    private bool heroInWater = false;
    
    void Start()
    {
        InitializeWater();
        SetupVisuals();
    }
    
    void InitializeWater()
    {
        // Get or add required components
        waterCollider = GetComponent<Collider2D>();
        if (waterCollider == null)
        {
            waterCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Set as trigger for detection
        waterCollider.isTrigger = true;
        
        // Set water layer
        gameObject.layer = LayerMask.NameToLayer("Water");
        if (gameObject.layer == 0) // If Water layer doesn't exist
        {
            Debug.LogWarning("Water layer not found. Please create a 'Water' layer in Project Settings > Tags and Layers");
            gameObject.layer = 6; // Default to layer 6
        }
        
        // Add tag for easy identification
        gameObject.tag = "Water";
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
    }
    
    void SetupVisuals()
    {
        // Get or add SpriteRenderer for visual representation
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Create a simple colored sprite if no material is assigned
        if (waterMaterial == null)
        {
            spriteRenderer.color = waterColor;
            // Create a simple white texture that can be tinted
            CreateDefaultWaterSprite();
        }
        else
        {
            spriteRenderer.material = waterMaterial;
        }
        
        // Set sorting order to be behind most objects but in front of background
        spriteRenderer.sortingOrder = -1;
    }
    
    void CreateDefaultWaterSprite()
    {
        // Create a simple 1x1 white texture for the sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        // Create sprite from texture
        Sprite waterSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100);
        spriteRenderer.sprite = waterSprite;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hero entered water
        if (other.CompareTag("Player") || other.GetComponent<HeroController>() != null)
        {
            heroInWater = true;
            
            HeroController hero = other.GetComponent<HeroController>();
            if (hero != null)
            {
                OnHeroEnterWater(hero, other.GetComponent<Rigidbody2D>());
            }
            
            Debug.Log($"{other.name} entered water!");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        // Check if hero exited water
        if (other.CompareTag("Player") || other.GetComponent<HeroController>() != null)
        {
            heroInWater = false;
            
            HeroController hero = other.GetComponent<HeroController>();
            if (hero != null)
            {
                OnHeroExitWater(hero, other.GetComponent<Rigidbody2D>());
            }
            
            Debug.Log($"{other.name} exited water!");
        }
    }
    
    void OnHeroEnterWater(HeroController hero, Rigidbody2D heroRb)
    {
        if (heroRb != null)
        {
            // Check velocity for splash effect
            float velocity = heroRb.linearVelocity.magnitude;
            if (velocity > splashThreshold)
            {
                CreateSplashEffect(heroRb.transform.position);
                PlaySplashSound();
            }
            
            // Apply water drag
            heroRb.linearDamping = waterDrag;
            
            Debug.Log("Hero entered water - applying water physics");
        }
    }
    
    void OnHeroExitWater(HeroController hero, Rigidbody2D heroRb)
    {
        if (heroRb != null)
        {
            // Remove water drag
            heroRb.linearDamping = 0f;
            
            Debug.Log("Hero exited water - removing water physics");
        }
    }
    
    void CreateSplashEffect(Vector3 splashPosition)
    {
        // Create a simple particle effect for splash
        // This is a basic implementation - can be enhanced with Unity's Particle System
        
        GameObject splashEffect = new GameObject("SplashEffect");
        splashEffect.transform.position = splashPosition;
        
        // Add a simple visual effect (could be replaced with particle system)
        SpriteRenderer splashRenderer = splashEffect.AddComponent<SpriteRenderer>();
        splashRenderer.color = new Color(1f, 1f, 1f, 0.8f);
        splashRenderer.sortingOrder = 10;
        
        // Destroy effect after a short time
        Destroy(splashEffect, 1f);
        
        Debug.Log("Created splash effect at: " + splashPosition);
    }
    
    void PlaySplashSound()
    {
        if (audioSource != null && splashSound != null)
        {
            audioSource.clip = splashSound;
            audioSource.Play();
        }
    }
    
    void PlayBubbleSound()
    {
        if (audioSource != null && bubbleSound != null)
        {
            audioSource.clip = bubbleSound;
            audioSource.Play();
        }
    }
    
    // Public methods for other systems
    public bool IsHeroInWater()
    {
        return heroInWater;
    }
    
    public Vector2 GetWaterSurfaceNormal(Vector2 point)
    {
        // For simple rectangular water, surface normal is always up
        // This could be enhanced for more complex water shapes
        return Vector2.up;
    }
    
    public float GetWaterLevel()
    {
        // Return the top Y position of the water surface
        if (waterCollider != null)
        {
            return waterCollider.bounds.max.y;
        }
        return transform.position.y;
    }
    
    // Gizmos for debugging
    void OnDrawGizmos()
    {
        // Draw water bounds
        Gizmos.color = new Color(waterColor.r, waterColor.g, waterColor.b, 0.3f);
        
        if (waterCollider != null)
        {
            Gizmos.DrawCube(waterCollider.bounds.center, waterCollider.bounds.size);
        }
        else
        {
            // Draw a default water area if no collider is set up yet
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
        
        // Draw water surface line
        Gizmos.color = Color.cyan;
        Vector3 surfaceCenter = waterCollider != null ? 
            new Vector3(waterCollider.bounds.center.x, waterCollider.bounds.max.y, 0) : 
            transform.position;
        float surfaceWidth = waterCollider != null ? waterCollider.bounds.size.x : 1f;
        
        Gizmos.DrawLine(
            surfaceCenter - Vector3.right * surfaceWidth / 2,
            surfaceCenter + Vector3.right * surfaceWidth / 2
        );
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw more detailed information when selected
        Gizmos.color = Color.blue;
        
        if (waterCollider != null)
        {
            // Draw water surface normal arrows
            Vector3 surfaceCenter = new Vector3(waterCollider.bounds.center.x, waterCollider.bounds.max.y, 0);
            float surfaceWidth = waterCollider.bounds.size.x;
            int arrowCount = 5;
            
            for (int i = 0; i < arrowCount; i++)
            {
                float t = (float)i / (arrowCount - 1);
                Vector3 arrowPos = surfaceCenter + Vector3.right * (t - 0.5f) * surfaceWidth;
                Gizmos.DrawLine(arrowPos, arrowPos + Vector3.up * 0.5f);
                
                // Arrow head
                Gizmos.DrawLine(arrowPos + Vector3.up * 0.5f, arrowPos + Vector3.up * 0.3f + Vector3.left * 0.1f);
                Gizmos.DrawLine(arrowPos + Vector3.up * 0.5f, arrowPos + Vector3.up * 0.3f + Vector3.right * 0.1f);
            }
        }
    }
}