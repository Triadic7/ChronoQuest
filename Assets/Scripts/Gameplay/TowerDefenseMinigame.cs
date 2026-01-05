using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseMinigame : Game
{
    [SerializeField]
    private Transform towerSpawnPoint;

    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private Transform[] enemySpawnPoints;

    private List<GameObject> spawnedObjects;

    [SerializeField]
    private float gameTimer;

    [SerializeField]
    private float enemySpawnTime;

    /// <summary>
    /// The timer for spawning enemies.
    /// </summary>
    private float spawnTimer;

    /// <summary>
    /// If the boss can spawn enemies.
    /// </summary>
    private bool canSpawnEnemies;

    // Make enemies ignore players.

    // Spawn enemies on edges of the map and move towards tower.

    // Players can walk into enemies to remove them.

    // Enemies touching the tower will lower progress bar.

    public override void StartGame()
    {
        // Spawn in tower in middle of screen.
        base.StartGame(); // Start the game.

        GameObject towerGO = Instantiate(towerPrefab, towerSpawnPoint.position, Quaternion.identity);
        this.spawnedObjects.Add(towerGO);

        Tower tower = towerGO.GetComponent<Tower>();
        tower.OnDamageTaken += this.ReceiveFailStrike;

        // Spawn enemies periodically.
        //SpawnEnemy();
    }

    private void Start()
    {
        InvokeRepeating("RegisterSuccess", 0f, 1f);

    }

    /// <summary>
    /// Remove all enemies.
    /// </summary>
    public override void CleanUp()
    {
        CancelInvoke("RegisterSuccess");
        DeleteEnemies();
    }

    public override void GameEnded()
    {
        DeleteTower();
    }

    private void Awake()
    {
        this.spawnedObjects = new List<GameObject>();
    }

    // Deletes all enemies from the game.
    private void DeleteEnemies()
    {
        foreach (GameObject go in this.spawnedObjects)
        {
            if (go.GetComponent<EnemyScript>())
            {
                Destroy(go);
            }
        }
    }

    // Deletes the tower from the game.
    private void DeleteTower()
    {
        foreach (GameObject go in this.spawnedObjects)
        {
            if (go.GetComponent<Tower>())
            {
                Destroy(go);
            }
        }
    }

    private void SpawnEnemy()
    {
        // Choose random spawn point.

        if (enemyPrefab != null)
        {
            int index = Random.Range(0, enemySpawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefab, enemySpawnPoints[index].position, Quaternion.identity);
            this.spawnedObjects.Add(enemy);
        }
    }

    /// <summary>
    /// Moves side to side and spawns enemies over time.
    /// </summary>
    private void Update()
    {
        this.HandleEnemySpawning();
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
}
