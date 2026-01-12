using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// The space game where players have to shoot a boss while defending a space station from attacks.
/// </summary>
public class SpaceMinigame : Game
{
    /// <summary>
    /// Where the boss spawns.
    /// </summary>
    [SerializeField]
    private Transform bossSpawnPoint;

    /// <summary>
    /// Where the space station spawns.
    /// </summary>
    [SerializeField]
    private Transform stationSpawnPoint;

    /// <summary>
    /// The boss prefab.
    /// </summary>
    [SerializeField]
    private GameObject bossPrefab;

    /// <summary>
    /// Space station prefab.
    /// </summary>
    [SerializeField]
    private GameObject spaceStationPrefab;

    /// <summary>
    /// The boss go when it gets instanced.
    /// </summary>
    private GameObject bossInstance;

    /// <summary>
    /// The station go when it gets instanced.
    /// </summary>
    private GameObject stationInstance;

    /// <summary>
    /// Spawner for asteroids.
    /// </summary>
    [SerializeField]
    private Spawner asteroidSpawner;

    /// <summary>
    /// Starts game by spawning in boss and other game objects.
    /// </summary>
    public override void StartGame()
    {
        base.StartGame();

        // Spawn in boss.
        GameObject boss = Instantiate(this.bossPrefab, this.bossSpawnPoint.position, Quaternion.identity, this.bossSpawnPoint);
        this.bossInstance = boss;

        // Get component and initialize the boss.
        BossEnemy bossEnemy = boss.GetComponent<BossEnemy>();
        bossEnemy.Initialize(this.CurrentStage.ObjectiveProgress);

        // Spawn enemies on objective start.
        this.OnObjectiveStart += bossEnemy.StartSpawningEnemies;

        // Listen for boss take damage.
        bossEnemy.OnTakeDamage += this.RegisterSuccess;

        // Spawn in space station.
        GameObject station = Instantiate(this.spaceStationPrefab, this.stationSpawnPoint.position, Quaternion.identity, this.stationSpawnPoint);
        this.stationInstance = station;

        // Register events with station.
        SpaceStation spaceStation = station.GetComponent<SpaceStation>();
        spaceStation.OnDamageTaken += this.ReceiveFailStrike;
    }

    /// <summary>
    /// Starts spawning asteroids.
    /// </summary>
    public override void StartGameObjective()
    {
        base.StartGameObjective();
        this.asteroidSpawner.StartSpawning();
    }

    /// <summary>
    /// Remove any gameObjects from the list and destroy them.
    /// </summary>
    public override void CleanUp()
    {
        this.asteroidSpawner.StopSpawningAndDestroyAll();
        this.CleanupStation();
        this.CleanUpBoss();
    }

    /// <summary>
    /// Destroys station instance.
    /// </summary>
    private void CleanupStation()
    {
        Destroy(this.stationInstance);
    }

    /// <summary>
    /// Clean up boss and enemies.
    /// </summary>
    public override void GameEnded()
    {
        this.asteroidSpawner.StopSpawningAndDestroyAll();
        this.CleanUpBoss();
    }

    /// <summary>
    /// Unsubscribes from boss and destroys it.
    /// </summary>
    private void CleanUpBoss()
    {
        // If player died, unsubscribe from events and destroy boss.
        if (this.bossInstance != null)
        {
            BossEnemy bossEnemy = this.bossInstance.GetComponent<BossEnemy>();
            this.OnObjectiveStart -= bossEnemy.StartSpawningEnemies;
            bossEnemy.OnTakeDamage -= this.RegisterSuccess;

            Destroy(this.bossInstance);
        }
    }
}
