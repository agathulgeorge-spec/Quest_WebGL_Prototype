using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public bool showDebugInfo = true;
    public KeyCode toggleDebugKey = KeyCode.F1;
    
    [Header("Layer Configuration")]
    public LayerMask groundLayer = 1;
    public LayerMask waterLayer = 1 << 6;
    public LayerMask obstacleLayer = 1 << 7;
    public LayerMask playerLayer = 1 << 8;
    
    [Header("Scene Objects")]
    public HeroController hero;
    public BeaconController beacon;
    public GameObject[] waterObjects;
    public GameObject[] obstacles;
    
    // Game state
    private bool gameInitialized = false;
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        HandleDebugInput();
        
        if (showDebugInfo)
        {
            DisplayDebugInfo();
        }
    }
    
    void InitializeGame()
    {
        Debug.Log("=== Cave Game - Initializing ===");
        
        // Find hero if not assigned
        if (hero == null)
        {
            hero = FindFirstObjectByType<HeroController>();
        }
        
        // Find beacon if not assigned  
        if (beacon == null)
        {
            beacon = FindFirstObjectByType<BeaconController>();
        }
        
        // Find water objects
        if (waterObjects == null || waterObjects.Length == 0)
        {
            Water[] waters = FindObjectsByType<Water>(FindObjectsSortMode.None);
            waterObjects = new GameObject[waters.Length];
            for (int i = 0; i < waters.Length; i++)
            {
                waterObjects[i] = waters[i].gameObject;
            }
        }
        
        // Find obstacles
        if (obstacles == null || obstacles.Length == 0)
        {
            LightAbsorbingObstacle[] obs = FindObjectsByType<LightAbsorbingObstacle>(FindObjectsSortMode.None);
            obstacles = new GameObject[obs.Length];
            for (int i = 0; i < obs.Length; i++)
            {
                obstacles[i] = obs[i].gameObject;
            }
        }
        
        // Validate critical components
        ValidateGameSetup();
        
        gameInitialized = true;
        Debug.Log("=== Cave Game - Initialization Complete ===");
    }
    
    void ValidateGameSetup()
    {
        // Check hero setup
        if (hero == null)
        {
            Debug.LogError("No Hero found in scene! Please add a HeroController component to the player character.");
        }
        else
        {
            Debug.Log($"✓ Hero found: {hero.name}");
            
            // Validate hero components
            if (hero.GetComponent<Rigidbody2D>() == null)
                Debug.LogError("Hero missing Rigidbody2D component!");
            if (hero.GetComponent<Collider2D>() == null)
                Debug.LogError("Hero missing Collider2D component!");
        }
        
        // Check beacon setup
        if (beacon == null)
        {
            Debug.LogError("No Beacon found! Please add a BeaconController component to the hero or create a separate beacon object.");
        }
        else
        {
            Debug.Log($"✓ Beacon found: {beacon.name}");
        }
        
        // Check layers
        ValidateLayers();
        
        // Check water objects
        Debug.Log($"✓ Found {waterObjects.Length} water objects");
        foreach (GameObject water in waterObjects)
        {
            if (water.GetComponent<Water>() == null)
                Debug.LogWarning($"Water object {water.name} missing Water component!");
        }
        
        // Check obstacles
        Debug.Log($"✓ Found {obstacles.Length} obstacle objects");
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle.GetComponent<LightAbsorbingObstacle>() == null)
                Debug.LogWarning($"Obstacle object {obstacle.name} missing LightAbsorbingObstacle component!");
        }
    }
    
    void ValidateLayers()
    {
        // Check if required layers exist
        string[] requiredLayers = { "Ground", "Water", "Obstacle", "Player" };
        bool allLayersValid = true;
        
        foreach (string layerName in requiredLayers)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                Debug.LogWarning($"Layer '{layerName}' not found. Please create it in Project Settings > Tags and Layers");
                allLayersValid = false;
            }
            else
            {
                Debug.Log($"✓ Layer '{layerName}' found at index {layer}");
            }
        }
        
        if (!allLayersValid)
        {
            Debug.LogWarning("Some required layers are missing. The game will use default layers but may not work optimally.");
        }
    }
    
    void HandleDebugInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        if (keyboard.f1Key.wasPressedThisFrame)
        {
            showDebugInfo = !showDebugInfo;
            Debug.Log($"Debug info display: {(showDebugInfo ? "ON" : "OFF")}");
        }
    }
    
    void DisplayDebugInfo()
    {
        // This would typically be displayed on screen using Unity's UI system
        // For now, we'll log debug information periodically
        
        if (Time.frameCount % 60 == 0) // Every 60 frames (~1 second at 60 FPS)
        {
            LogGameState();
        }
    }
    
    void LogGameState()
    {
        if (!gameInitialized) return;
        
        string debugInfo = "\n=== Cave Game Debug Info ===";
        
        // Hero state
        if (hero != null)
        {
            Vector3 heroPos = hero.transform.position;
            debugInfo += $"\nHero Position: ({heroPos.x:F2}, {heroPos.y:F2})";
            
            if (hero.GetComponent<Rigidbody2D>() != null)
            {
                Vector2 velocity = hero.GetComponent<Rigidbody2D>().linearVelocity;
                debugInfo += $"\nHero Velocity: ({velocity.x:F2}, {velocity.y:F2})";
            }
        }
        
        // Beacon state
        if (beacon != null)
        {
            debugInfo += $"\nBeacon Active: {beacon.IsBeaconActive()}";
            debugInfo += $"\nBeacon Directional: {beacon.IsDirectionalMode()}";
            if (beacon.IsDirectionalMode())
            {
                Vector2 direction = beacon.GetBeaconDirection();
                debugInfo += $"\nBeacon Direction: ({direction.x:F2}, {direction.y:F2})";
            }
        }
        
        debugInfo += "\n===========================";
        Debug.Log(debugInfo);
    }
    
    // Public methods for other systems
    public HeroController GetHero()
    {
        return hero;
    }
    
    public BeaconController GetBeacon()
    {
        return beacon;
    }
    
    public GameObject[] GetWaterObjects()
    {
        return waterObjects;
    }
    
    public GameObject[] GetObstacles()
    {
        return obstacles;
    }
    
    public bool IsGameInitialized()
    {
        return gameInitialized;
    }
    
    // Utility methods
    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}