using System.Collections.Generic;
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
    /// Astroid prefab.
    /// </summary>
    [SerializeField]
    private GameObject astroidPrefab;

    /// <summary>
    /// Enemy prefab.
    /// </summary>
    [SerializeField]
    private GameObject enemyPrefab;

    /// <summary>
    /// Space station prefab.
    /// </summary>
    [SerializeField]
    private GameObject spaceStationPrefab;

    /// <summary>
    /// The list of objects to destroy at the end of the game.
    /// </summary>
    private List<GameObject> gameObjects;

    // Dealing damage to boss increases progress bar.

    // Spawn astroids players can fly into or shoot to fling or destroy.

    /// <summary>
    /// Starts game by spawning in boss and other game objects.
    /// </summary>
    public override void StartGame()
    {
        base.StartGame();

        this.gameObjects = new List<GameObject>();

        // Spawn in boss.
        GameObject boss = Instantiate(this.bossPrefab, this.bossSpawnPoint.position, Quaternion.identity, this.bossSpawnPoint);
        this.gameObjects.Add(boss);

        // Get component and initialize the boss.
        BossEnemy bossEnemy = boss.GetComponent<BossEnemy>();
        bossEnemy.Initialize(this.CurrentStage.ObjectiveProgress, enemyPrefab);

        // Add event listeners to boss.
        bossEnemy.OnSpawnEnemy += (enemy) =>
        {
            // Adds enemy spawned to list of objects to clean up.
            this.gameObjects.Add(enemy);
        };

        // Spawn enemies on objective start.
        this.OnObjectiveStart += bossEnemy.StartSpawningEnemies;

        // Listen for boss take damage.
        bossEnemy.OnTakeDamage += this.RegisterSuccess;

        // Spawn in space station.
        GameObject station = Instantiate(this.spaceStationPrefab, this.stationSpawnPoint.position, Quaternion.identity, this.stationSpawnPoint);
        this.gameObjects.Add(station);

        // Register events with station.
        SpaceStation spaceStation = station.GetComponent<SpaceStation>();
        spaceStation.OnDamageTaken += this.ReceiveFailStrike;
    }

    /// <summary>
    /// Remove any gameObjects from the list and destroy them.
    /// </summary>
    public override void CleanUp()
    {
        this.CleanupStation();
    }

    /// <summary>
    /// Cleans up enemies and boss by destroying.
    /// </summary>
    private void CleanupEnemiesAndBoss()
    {
        foreach (GameObject go in this.gameObjects)
        {
            if(go != null)
            {
                BossEnemy bossEnemy = go.GetComponent<BossEnemy>();
                if (bossEnemy)
                {
                    // Spawn enemies on objective start.
                    this.OnObjectiveStart -= bossEnemy.StartSpawningEnemies;
                    bossEnemy.StopSpawningEnemies();
                }
                Destroy(go);
            }
        }
    }

    /// <summary>
    /// Destroys station.
    /// </summary>
    private void CleanupStation()
    {
        foreach (GameObject go in gameObjects)
        {
            if (go == null)
            {
                continue;
            }

            if (go.GetComponent<SpaceStation>())
            {
                Destroy(go);
            }
        }
    }

    /// <summary>
    /// Clean up boss and enemies.
    /// </summary>
    public override void GameEnded()
    {
        this.CleanupEnemiesAndBoss();
    }
}
