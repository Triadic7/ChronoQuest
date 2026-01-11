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
    /// Where the astroids spawns.
    /// </summary>
    [SerializeField]
    private Transform asteroidSpawnPointA;

    /// <summary>
    /// Where the astroids spawns.
    /// </summary>
    [SerializeField]
    private Transform asteroidSpawnPointB;

    /// <summary>
    /// The boss prefab.
    /// </summary>
    [SerializeField]
    private GameObject bossPrefab;

    /// <summary>
    /// Astroid prefab.
    /// </summary>
    [SerializeField]
    private GameObject asteroidPrefab;

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

    /// <summary>
    /// Time between asteroid spawns.
    /// </summary>
    [SerializeField]
    private float asteroidSpawnInterval = 2f;

    /// <summary>
    /// Timer for spawning asteroids.
    /// </summary>
    private float asteroidSpawnTimer;

    /// <summary>
    /// Whether asteroids are currently spawning.
    /// </summary>
    private bool spawnAsteroids;

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
        this.CleanupEnemiesAndBoss();
        this.StopSpawningAsteroids();
    }

    /// <summary>
    /// Spawns astroids over time.
    /// </summary>
    private void Update()
    {
        if (!this.spawnAsteroids)
        {
            return;
        }

        // Increase timer.
        this.asteroidSpawnTimer += Time.deltaTime;

        // Spawn astroid if timer hit.
        if (this.asteroidSpawnTimer >= this.asteroidSpawnInterval)
        {
            this.SpawnAstroid();
            this.asteroidSpawnTimer = 0f;
        }
    }

    /// <summary>
    /// Cleans up enemies and boss by destroying.
    /// </summary>
    private void CleanupEnemiesAndBoss()
    {
        if(this.gameObjects.Count > 0)
        {
            foreach (GameObject go in this.gameObjects)
            {
                if (go != null)
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
    }

    /// <summary>
    /// Spawns astroid and adds it to the list of gameobjects.
    /// </summary>
    private void SpawnAstroid()
    {
        // Spawns astroids on a random point between the spawn points.
        float t = Random.value;
        Vector3 spawnPos = Vector3.Lerp(this.asteroidSpawnPointA.position, this.asteroidSpawnPointB.position, t);

        GameObject astroid = Instantiate(this.asteroidPrefab, spawnPos, Quaternion.identity);
        this.gameObjects.Add(astroid);
    }

    /// <summary>
    /// Starts spawning astroids.
    /// </summary>
    private void StartSpawningAsteroids()
    {
        this.spawnAsteroids = true;
        this.asteroidSpawnTimer = 0f;
    }

    /// <summary>
    /// Stops spawning astroids.
    /// </summary>
    private void StopSpawningAsteroids()
    {
        this.spawnAsteroids = false;
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

    /// <summary>
    /// Starts spawning asteroids.
    /// </summary>
    public override void StartGameObjective()
    {
        base.StartGameObjective();

        this.StartSpawningAsteroids();
    }
}
