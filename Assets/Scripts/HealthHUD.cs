using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    [Header("Health Bars")]
    public Slider heroHealthBar;
    public Slider enemyHealthBar;

    [Header("Auto Find")]
    public bool automaticallyFindPlayer = true;
    public bool automaticallyFindEnemy = true;

    private Player_Health playerHealth;
    private Enemy_Health currentEnemy;

    private void Awake()
    {
        // Find Player Health
        if (automaticallyFindPlayer)
        {
            playerHealth = FindObjectOfType<Player_Health>();
        }

        // Make sure bars start correctly
        if (heroHealthBar != null)
        {
            heroHealthBar.minValue = 0f;
        }

        if (enemyHealthBar != null)
        {
            enemyHealthBar.minValue = 0f;
            enemyHealthBar.value = 0f;
            enemyHealthBar.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        UpdateHeroBar();
        FindEnemy();
        UpdateEnemyBar();
    }

    private void Update()
    {
        // Keep finding player if necessary
        if (playerHealth == null && automaticallyFindPlayer)
        {
            playerHealth = FindObjectOfType<Player_Health>();
        }

        UpdateHeroBar();

        if (currentEnemy == null ||
            currentEnemy.IsDead)
        {
            FindEnemy();
        }

        UpdateEnemyBar();
    }

    private void UpdateHeroBar()
    {
        if (playerHealth == null)
            return;

        if (heroHealthBar == null)
            return;

        heroHealthBar.minValue = 0f;
        heroHealthBar.maxValue = playerHealth.MaxHealth;
        heroHealthBar.value = playerHealth.CurrentHealth;
    }

    private void FindEnemy()
    {
        if (!automaticallyFindEnemy)
            return;

        Enemy_Health[] enemies =
            FindObjectsOfType<Enemy_Health>();

        if (enemies.Length == 0)
        {
            currentEnemy = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        Enemy_Health closestEnemy = null;

        Vector3 playerPosition = transform.position;

        if (playerHealth != null)
        {
            playerPosition = playerHealth.transform.position;
        }

        foreach (Enemy_Health enemy in enemies)
        {
            if (enemy == null)
                continue;

            if (enemy.IsDead)
                continue;

            float distance =
                Vector3.Distance(
                    playerPosition,
                    enemy.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        currentEnemy = closestEnemy;
    }

    private void UpdateEnemyBar()
    {
        if (enemyHealthBar == null)
            return;

        if (currentEnemy == null ||
            currentEnemy.IsDead)
        {
            enemyHealthBar.gameObject.SetActive(false);
            return;
        }

        enemyHealthBar.gameObject.SetActive(true);

        enemyHealthBar.minValue = 0f;
        enemyHealthBar.maxValue = currentEnemy.MaxHealth;
        enemyHealthBar.value = currentEnemy.CurrentHealth;
    }
}