using System;
using UnityEngine;

/// <summary>
/// Boss enemy for the space minigame.
/// </summary>
public class BossEnemy : Enemy, IHealth
{
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
    /// Readies the boss for play by getting what enemy the boss will spawn.
    /// </summary>
    /// <param name="health">The boss health.</param>
    public void Initialize(int health)
    {
        this.Health = health;
        this.CurrentHealth = this.Health;
        GetComponentInChildren<HealthBar>().Refresh();
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
