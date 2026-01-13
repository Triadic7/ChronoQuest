using System;
using UnityEngine;

/// <summary>
/// Boss enemy for the space minigame.
/// </summary>
public class BossEnemy : Enemy, IHealth
{
    /// <summary>
    /// How far from start boss moves.
    /// </summary>
    [SerializeField]
    private float moveDistance = 5f;

    /// <summary>
    /// Timer for moving.
    /// </summary>
    private float moveTimer;

    /// <summary>
    /// The orginal scale.
    /// </summary>
    private Vector3 originalScale;

    /// <summary>
    /// Enemy spawner for boss.
    /// </summary>
    [SerializeField]
    private Spawner enemySpawner;

    /// <summary>
    /// Start spawning enemies.
    /// </summary>
    public void StartSpawningEnemies()
    {
        this.enemySpawner.StartSpawning();
    }

    /// <summary>
    /// Stop spawning enemies.
    /// </summary>
    public void StopSpawningEnemies()
    {
        this.enemySpawner.StopSpawning();
    }

    /// <summary>
    /// Cache start position for movement.
    /// </summary>
    private void Start()
    {
        this.startPosition = this.transform.position;
        this.originalScale = this.transform.localScale;
    }

    /// <summary>
    /// Moves side to side and spawns enemies over time.
    /// </summary>
    public override void FixedUpdate()
    {
        this.MoveSideToSide();
    }

    /// <summary>
    /// Moves the boss side to side.
    /// </summary>
    private void MoveSideToSide()
    {
        if (this.moveDistance <= 0 || this.CurrentHealth <= 0)
        {
            return;
        }

        // Calculate speed multiplier based on health.
        float healthRatio = Mathf.Clamp01((float)this.CurrentHealth / this.Health);
        float speedMultiplier = 1f + (1f - healthRatio) * 1f;

        // Increment timer.
        this.moveTimer += Time.deltaTime * this.MovementSpeed * speedMultiplier;

        // Move back and forth.
        float xOffset = Mathf.Sin(this.moveTimer) * this.moveDistance;
        Vector3 newPos = this.startPosition + new Vector3(xOffset, 0f, 0f);

        // Determine direction.
        float direction = newPos.x - this.transform.position.x;

        // Flips sprite.
        if (direction != 0f)
        {
            float sign = Mathf.Sign(direction);
            this.transform.localScale = new Vector3(Mathf.Abs(this.originalScale.x) * sign, this.originalScale.y, this.originalScale.z);
        }

        this.transform.position = newPos;
    }

    /// <summary>
    /// Readies the boss for play by getting what enemy the boss will spawn.
    /// </summary>
    /// <param name="health">The boss health.</param>
    /// <param name="enemyPrefab">The enemies the boss will spawn over time.</param>
    public void Initialize(int health)
    {
        this.Health = health;
        this.CurrentHealth = this.Health;
    }

    /// <summary>
    /// On boss killed, destroy all spawned objects.
    /// </summary>
    private void OnDestroy()
    {
        if (this.enemySpawner != null)
        {
            this.enemySpawner.StopSpawningAndDestroyAll();
        }
    }
}
