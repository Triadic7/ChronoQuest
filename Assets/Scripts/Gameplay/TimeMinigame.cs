using System;
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

    /// <summary>
    /// Event for subscribing and unsubscribing to spawn.
    /// </summary>
    private Action<GameObject> onEnemySpawned;

    /// <summary>
    /// Start game by finding all players.
    /// Enables ability for players to get hurt.
    /// On enemy spawn, add events for chasing one of the players.
    /// </summary>
    public override void StartGame()
    {
        base.StartGame();

        // Find all players and make them get hurt.
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        foreach (Player player in players) 
        {
            player.EnableCanBeHurt();
        }

        // Set on enemy spawned to chase player.
        this.onEnemySpawned = (go) =>
        {
            ChaseMovement chase = go.GetComponent<ChaseMovement>();

            // Find random player.
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

            // Pick a random player.
            Player targetPlayer = players[UnityEngine.Random.Range(0, players.Length)];

            // Make this target the player.
            chase.SetTargetFunction(() => targetPlayer.transform);
        };

        // Add event to enemy spawner.
        this.enemySpawner.OnObjectInstanced += this.onEnemySpawned;
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

        // Clear events.
        if (this.onEnemySpawned != null)
        {
            this.enemySpawner.OnObjectInstanced -= this.onEnemySpawned;
            this.onEnemySpawned = null;
        }
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

        // Avoid spawning at 0.
        if (currentProgress <= 0)
        {
            return;
        }


        // Convert the percentage to actual progress units.
        int unitsPerEnemy = Mathf.CeilToInt(this.PointsToWin * (this.spawnEnemyAtProgressPercentage / 100f));

        // No 0 division.
        if (unitsPerEnemy <= 0)
        {
            unitsPerEnemy = 1;
        }

        // Spawn enemy if enough units of progress have passed.
        if (currentProgress >= this.lastEnemySpawnProgress + unitsPerEnemy)
        {
            this.enemySpawner.SpawnOnce();
            this.lastEnemySpawnProgress += unitsPerEnemy;
        }
    }


}
