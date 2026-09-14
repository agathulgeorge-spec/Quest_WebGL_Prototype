using UnityEngine;

public class LightAbsorbingObstacle : MonoBehaviour
{
    [Header("Obstacle Properties")]
    public bool isDestructible = false;
    public int hitPointsRequired = 3;
    public float destructionDelay = 0.1f;
    
    [Header("Visual Settings")]
    public Color obstacleColor = Color.black;
    public Material obstacleMaterial;
    public bool showAbsorptionEffect = true;
    
    [Header("Audio Settings")]
    public AudioClip absorptionSound;
    public AudioClip destructionSound;
    
    [Header("Particle Effects")]
    public GameObject absorptionEffectPrefab;
    public GameObject destructionEffectPrefab;
    
    // Components
    private Collider2D obstacleCollider;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    // State tracking
    private int currentHitPoints;
    private bool isDestroyed = false;
    
    void Start()
    {
        InitializeObstacle();
        SetupVisuals();
        currentHitPoints = hitPointsRequired;
    }
    
    void InitializeObstacle()
    {
        // Get or add required components
        obstacleCollider = GetComponent<Collider2D>();
        if (obstacleCollider == null)
        {
            obstacleCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Set as solid collider (not trigger) to block movement and light
        obstacleCollider.isTrigger = false;
        
        // Set obstacle layer
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
        if (gameObject.layer == 0) // If Obstacle layer doesn't exist
        {
            Debug.LogWarning("Obstacle layer not found. Please create an 'Obstacle' layer in Project Settings > Tags and Layers");
            gameObject.layer = 7; // Default to layer 7
        }
        
        // Add tag for easy identification
        gameObject.tag = "Obstacle";
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f;
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
        if (obstacleMaterial == null)
        {
            spriteRenderer.color = obstacleColor;
            CreateDefaultObstacleSprite();
        }
        else
        {
            spriteRenderer.material = obstacleMaterial;
        }
        
        // Set sorting order to be visible above background
        spriteRenderer.sortingOrder = 0;
    }
    
    void CreateDefaultObstacleSprite()
    {
        // Create a simple 1x1 texture for the sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        // Create sprite from texture
        Sprite obstacleSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100);
        spriteRenderer.sprite = obstacleSprite;
    }
    
    public void OnLightRayHit(Vector2 hitPoint, Vector2 rayDirection)
    {
        if (isDestroyed) return;
        
        // Light ray has been absorbed by this obstacle
        Debug.Log($"Light ray absorbed by obstacle: {name} at point: {hitPoint}");
        
        // Play absorption effect
        if (showAbsorptionEffect)
        {
            CreateAbsorptionEffect(hitPoint, rayDirection);
        }
        
        // Play sound
        PlayAbsorptionSound();
        
        // LIGHT RAYS CAN DESTROY OBSTACLES!
        if (isDestructible)
        {
            currentHitPoints--;
            Debug.Log($"Light ray damaged obstacle {name}! HP: {currentHitPoints}/{hitPointsRequired}");
            
            if (currentHitPoints <= 0)
            {
                Debug.Log($"Obstacle {name} destroyed by light ray!");
                StartCoroutine(DestroyObstacle());
            }
            else
            {
                // Visual feedback for light damage
                StartCoroutine(FlashDamage());
            }
        }
        
        // Log the absorption for debugging
        LogLightAbsorption(hitPoint, rayDirection);
    }
    
    void CreateAbsorptionEffect(Vector2 hitPoint, Vector2 rayDirection)
    {
        if (absorptionEffectPrefab != null)
        {
            // Instantiate prefab effect
            GameObject effect = Instantiate(absorptionEffectPrefab, hitPoint, Quaternion.identity);
            Destroy(effect, 2f);
        }
        else
        {
            // Create simple particle effect
            CreateSimpleAbsorptionEffect(hitPoint);
        }
    }
    
    void CreateSimpleAbsorptionEffect(Vector2 hitPoint)
    {
        // Create a simple visual effect for light absorption
        GameObject effectObject = new GameObject("AbsorptionEffect");
        effectObject.transform.position = hitPoint;
        
        SpriteRenderer effectRenderer = effectObject.AddComponent<SpriteRenderer>();
        effectRenderer.color = new Color(1f, 1f, 0f, 0.8f); // Yellow flash
        effectRenderer.sortingOrder = 15;
        
        // Create simple expanding circle effect
        CreateExpandingCircleTexture(effectRenderer);
        
        // Animate the effect
        StartCoroutine(AnimateAbsorptionEffect(effectObject, effectRenderer));
    }
    
    void CreateExpandingCircleTexture(SpriteRenderer renderer)
    {
        int size = 16;
        Texture2D texture = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = distance < (size / 4f) ? 1f : 0f;
                texture.SetPixel(x, y, new Color(1f, 1f, 0f, alpha));
            }
        }
        
        texture.Apply();
        
        Sprite effectSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        renderer.sprite = effectSprite;
    }
    
    System.Collections.IEnumerator AnimateAbsorptionEffect(GameObject effectObject, SpriteRenderer effectRenderer)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.1f;
        Vector3 endScale = Vector3.one * 0.5f;
        Color startColor = effectRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        effectObject.transform.localScale = startScale;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Scale animation
            effectObject.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            // Fade out animation
            effectRenderer.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }
        
        Destroy(effectObject);
    }
    
    void PlayAbsorptionSound()
    {
        if (audioSource != null && absorptionSound != null)
        {
            audioSource.clip = absorptionSound;
            audioSource.pitch = Random.Range(0.8f, 1.2f); // Add slight pitch variation
            audioSource.Play();
        }
    }
    
    void LogLightAbsorption(Vector2 hitPoint, Vector2 rayDirection)
    {
        Debug.Log($"[Light Absorption] Obstacle: {name}, Hit Point: {hitPoint}, Ray Direction: {rayDirection}");
    }
    
    // Called when hero attacks this obstacle with sword
    public void OnSwordHit(HeroController attacker)
    {
        if (isDestroyed) return;
        
        Debug.Log($"Obstacle {name} hit by sword!");
        
        if (isDestructible)
        {
            currentHitPoints--;
            Debug.Log($"Obstacle {name} took damage. HP: {currentHitPoints}/{hitPointsRequired}");
            
            if (currentHitPoints <= 0)
            {
                StartCoroutine(DestroyObstacle());
            }
            else
            {
                // Visual feedback for damage
                StartCoroutine(FlashDamage());
            }
        }
        else
        {
            Debug.Log($"Obstacle {name} is indestructible!");
            // Could add spark effect or sound here
        }
    }
    
    System.Collections.IEnumerator DestroyObstacle()
    {
        isDestroyed = true;
        
        // Play destruction sound
        if (audioSource != null && destructionSound != null)
        {
            audioSource.clip = destructionSound;
            audioSource.Play();
        }
        
        // Create destruction effect
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // Visual destruction animation
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        Color originalColor = spriteRenderer.color;
        
        while (elapsed < destructionDelay)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / destructionDelay;
            
            // Scale down
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            
            // Fade out
            Color newColor = Color.Lerp(originalColor, Color.clear, t);
            spriteRenderer.color = newColor;
            
            yield return null;
        }
        
        Debug.Log($"Obstacle {name} destroyed!");
        
        // Destroy the game object
        Destroy(gameObject);
    }
    
    System.Collections.IEnumerator FlashDamage()
    {
        Color originalColor = spriteRenderer.color;
        Color flashColor = Color.red;
        
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    // Public methods for other systems
    public bool IsDestructible()
    {
        return isDestructible;
    }
    
    public bool IsDestroyed()
    {
        return isDestroyed;
    }
    
    public int GetRemainingHitPoints()
    {
        return currentHitPoints;
    }
    
    public Vector2 GetSurfaceNormal(Vector2 hitPoint)
    {
        // Calculate surface normal based on hit point
        // For simple rectangular obstacles, find the closest face
        Vector2 center = obstacleCollider.bounds.center;
        Vector2 size = obstacleCollider.bounds.size;
        
        Vector2 localHit = hitPoint - center;
        
        // Determine which face was hit based on the hit point
        float absX = Mathf.Abs(localHit.x / (size.x / 2));
        float absY = Mathf.Abs(localHit.y / (size.y / 2));
        
        if (absX > absY)
        {
            // Hit left or right face
            return localHit.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            // Hit top or bottom face
            return localHit.y > 0 ? Vector2.up : Vector2.down;
        }
    }
    
    // Gizmos for debugging
    void OnDrawGizmos()
    {
        // Draw obstacle bounds
        Gizmos.color = isDestructible ? Color.red : Color.black;
        
        if (obstacleCollider != null)
        {
            Gizmos.DrawWireCube(obstacleCollider.bounds.center, obstacleCollider.bounds.size);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw more detailed information when selected
        Gizmos.color = isDestructible ? Color.yellow : Color.gray;
        
        if (obstacleCollider != null)
        {
            Gizmos.DrawCube(obstacleCollider.bounds.center, obstacleCollider.bounds.size);
            
            // Draw hit points indicator
            if (isDestructible)
            {
                Vector3 textPos = obstacleCollider.bounds.center + Vector3.up * (obstacleCollider.bounds.size.y / 2 + 0.5f);
                // Note: Gizmos.DrawGUI is not available, so we use debug text in console instead
                Debug.Log($"Obstacle {name} - HP: {currentHitPoints}/{hitPointsRequired}");
            }
        }
    }
}