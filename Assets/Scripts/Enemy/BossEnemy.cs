using System;
using UnityEngine;

/// <summary>
/// Boss enemy for the space minigame.
/// </summary>
public class BossEnemy : MonoBehaviour, IHealth
{
    /// <summary>
    /// Event thats fired whenever the boss takes damage.
    /// </summary>
    public event Action OnTakeDamage;

    /// <summary>
    /// Fires event using the enemy.
    /// </summary>
    public event Action<GameObject> OnSpawnEnemy;

    /// <summary>
    /// The health of the boss.
    /// </summary>
    public int Health { get; set; } = 100;

    /// <summary>
    /// The current health.
    /// </summary>
    public int CurrentHealth { get; set; }

    /// <summary>
    /// How often the boss spawns in enemies.
    /// </summary>
    [SerializeField]
    private float enemySpawnTime;

    /// <summary>
    /// The enemy prefab.
    /// </summary>
    [SerializeField]
    private GameObject enemyPrefab;

    /// <summary>
    /// How far from start boss moves.
    /// </summary>
    [SerializeField] 
    private float moveDistance = 5f;

    /// <summary>
    /// How fast the boss is.
    /// </summary>
    [SerializeField]
    private float moveSpeed = 2f;

    /// <summary>
    /// Where the boss starts at.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// The timer for spawning enemies.
    /// </summary>
    private float spawnTimer;

    /// <summary>
    /// Timer for moving.
    /// </summary>
    private float moveTimer;

    /// <summary>
    /// If the boss can spawn enemies.
    /// </summary>
    private bool canSpawnEnemies;

    /// <summary>
    /// Take damage, and fire event.
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    /// <returns>Returns true if damage killed boss.</returns>
    public void TakeDamage(int damage)
    {
        // Check for death.
        this.CurrentHealth -= damage;
        if (this.CurrentHealth < 0)
        {
            this.CurrentHealth = 0;
        }

        // Fire event.
        this.OnTakeDamage?.Invoke();
    }

    /// <summary>
    /// Start spawning enemies.
    /// </summary>
    public void StartSpawningEnemies()
    {
        this.canSpawnEnemies = true;
    }

    /// <summary>
    /// Stop spawning enemies.
    /// </summary>
    public void StopSpawningEnemies()
    {
        this.canSpawnEnemies = false;
    }

    /// <summary>
    /// Cache start position for movement.
    /// </summary>
    private void Start()
    {
        this.startPosition = this.transform.position;
    }

    /// <summary>
    /// Moves side to side and spawns enemies over time.
    /// </summary>
    private void Update()
    {
        this.MoveSideToSide();
        this.HandleEnemySpawning();
    }

    /// <summary>
    /// Moves the boss side to side.
    /// </summary>
    private void MoveSideToSide()
    {
        if (this.moveDistance <= 0)
        {
            return;
        }

        // Prevent divide by 0.
        if(this.CurrentHealth <= 0)
        {
            return;
        }

        // Calculate speed multiplier based on health.
        float healthRatio = Mathf.Clamp01((float)this.CurrentHealth / this.Health);
        float speedMultiplier = 1f + (1f - healthRatio) * 1f;

        // Increment timer.
        this.moveTimer += Time.deltaTime * this.moveSpeed * speedMultiplier;

        // Move back and forth.
        float offset = Mathf.PingPong(this.moveTimer, this.moveDistance * 2f) - this.moveDistance;
        this.transform.position = this.startPosition + new Vector3(offset, 0f, 0f);
    }

    /// <summary>
    /// Handles enemy spawning.
    /// </summary>
    private void HandleEnemySpawning()
    {
        if (this.enemyPrefab == null || this.enemySpawnTime <= 0)
        {
            return; 
        }

        // Check if boss can spawn enemies.
        if (this.canSpawnEnemies)
        {
            // Spawns enemy over time.
            this.spawnTimer += Time.deltaTime;
            if (this.spawnTimer >= this.enemySpawnTime)
            {
                this.SpawnEnemy();

                // Reset timer.
                this.spawnTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Spawns an enemy in.
    /// </summary>
    private void SpawnEnemy()
    {
        if (this.enemyPrefab == null)
        {
            return;
        }

        // Instantiate enemy at boss's position.
        GameObject enemy = Instantiate(this.enemyPrefab, this.transform.position, Quaternion.identity);

        // Fire event.
        this.OnSpawnEnemy?.Invoke(enemy);
    }

    /// <summary>
    /// Readies the boss for play by getting what enemy the boss will spawn.
    /// </summary>
    /// <param name="health">The boss health.</param>
    /// <param name="enemyPrefab">The enemies the boss will spawn over time.</param>
    public void Initialize(int health, GameObject enemyPrefab)
    {
        this.Health = health;
        this.CurrentHealth = this.Health;
        this.enemyPrefab = enemyPrefab;
    }
}
