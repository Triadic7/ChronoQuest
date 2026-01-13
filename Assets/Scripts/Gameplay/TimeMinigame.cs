using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects will spawn transparent and become more solid over time. If object becomes fully solid, spawn enemies to knock players around. Each spawn gives a failure.
/// </summary>
public class TimeMinigame : Game
{
    /// <summary>
    /// Spawner for distrubances.
    /// </summary>
    [SerializeField]
    private Spawner disturbanceSpawner;

    /// <summary>
    /// Spawner for enemy.
    /// </summary>
    [SerializeField]
    private Spawner enemySpawner;

    /// <summary>
    /// When the player makes progress, summon enemy if they have made this amount.
    /// E.g. x = 10, every 10% of the objective done, spawn enemy.
    /// </summary>
    [SerializeField]
    private int spawnEnemyAtProgressPercentage;

    /// <summary>
    /// The instances of disturbances.
    /// </summary>
    private List<Disturbance> disturbanceInstances;

    /// <summary>
    /// Keeps count of last enemy spawned at progress amount.
    /// </summary>
    private int lastEnemySpawnProgress;


    // Objects start clear => change to solid over time.

    // If players dont grab objects in time => strike.

    // Spawn enemy every x progress amount.

    // Grabbing objects fills progress bar.

    // Enemies touching players will lose health.

    /// <summary>
    /// Start game .
    /// </summary>
    public override void StartGame()
    {
        base.StartGame();
    }

    /// <summary>
    /// Remove any spawned objects.
    /// </summary>
    public override void CleanUp()
    {
        // Stop spawner.
        this.disturbanceSpawner.StopSpawningAndDestroyAll();

        // Destroy all disturbances in the scene.
        if (this.disturbanceInstances != null)
        {
            for (int i = this.disturbanceInstances.Count - 1; i >= 0; i--)
            {
                if (this.disturbanceInstances[i] != null)
                {
                    Destroy(this.disturbanceInstances[i].gameObject);
                }
            }

            // Clear list.
            this.disturbanceInstances.Clear();
        }
    }

    /// <summary>
    /// Stop spawner.
    /// </summary>
    public override void GameEnded()
    {
        // Stop enemy spawner.
        this.enemySpawner.StopSpawningAndDestroyAll();
    }

    /// <summary>
    /// Starts the game objective.
    /// </summary>
    public override void StartGameObjective()
    {
        base.StartGameObjective();
        this.disturbanceSpawner.StartSpawning();
        this.enemySpawner.StartSpawning();
    }

    /// <summary>
    /// Create new list and add event listeners.
    /// </summary>
    private void Start()
    {
        this.disturbanceInstances = new List<Disturbance>();

        // Cache event listener on spawner when object spawns.
        this.disturbanceSpawner.OnObjectInstanced += (prefab) =>
        {
            Disturbance disturbance = prefab.GetComponent<Disturbance>();

            this.OnDisturbanceSpawn(disturbance);
        };
    }

    /// <summary>
    /// Subscribe to disturbance events and add to list.
    /// </summary>
    /// <param name="disturbance">The disturbance.</param>
    private void OnDisturbanceSpawn(Disturbance disturbance)
    {
        this.disturbanceInstances.Add(disturbance);

        // Subscribe to its destroy event to register success.
        disturbance.OnDestroyDisturbance += this.HandleDisturbanceDestroyed;
    }

    /// <summary>
    /// Handles disturbance destroyed.
    /// </summary>
    /// <param name="disturbance">The disturbance.</param>
    /// <param name="wasCollected">If the disturbance was collected.</param>
    private void HandleDisturbanceDestroyed(Disturbance disturbance, bool wasCollected)
    {
        // Unsubscribe.
        disturbance.OnDestroyDisturbance -= this.HandleDisturbanceDestroyed;

        // Remove from list.
        this.disturbanceInstances.Remove(disturbance);

        // Give penalty or success if the disturbance was collected.
        if (wasCollected)
        {
            // Register success.
            this.RegisterSuccess();
        }
        else
        {
            this.ReceiveFailStrike();
        }

        // Check if enemy should spawn.
        this.TrySpawnEnemy();
    }

    /// <summary>
    /// Tries to spawn enemy.
    /// </summary>
    private void TrySpawnEnemy()
    {
        // Do nothing if objective is not active.
        if (!this.IsObjectiveActive)
        {
            return;
        }

        int currentProgress = this.GetCurrentProgress();

        // Avoid spawning at 0
        if (currentProgress <= 0)
        {
            return;
        }

        // Spawn enemy every X progress based on field for spawning enemies at percentage.
        if (currentProgress % this.spawnEnemyAtProgressPercentage == 0 && currentProgress != this.lastEnemySpawnProgress)
        {
            this.enemySpawner.SpawnOnce();
            this.lastEnemySpawnProgress = currentProgress;
        }
    }


}
