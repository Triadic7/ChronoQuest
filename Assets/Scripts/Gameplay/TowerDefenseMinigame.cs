using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseMinigame : Game
{
    /// <summary>
    /// Where the tower spawns
    /// </summary>
    [SerializeField]
    private List<Transform> enemySpawnPoints;

    /// <summary>
    /// Where the tower spawns
    /// </summary>
    [SerializeField]
    private Transform towerSpawnPoint;

    /// <summary>
    /// The tower prefab.
    /// </summary>
    [SerializeField]
    private GameObject towerPrefab;

    /// <summary>
    /// The enemy prefab.
    /// </summary>
    [SerializeField]
    private GameObject enemyPrefab;

    /// <summary>
    /// Time between enemy spawns.
    /// </summary>
    [SerializeField]
    private float enemySpawnInterval = 1f;

    [SerializeField]
    public PlayerCombat playerOneCombat;

    [SerializeField]
    public PlayerCombat playerTwoCombat;

    /// <summary>
    /// Timer for spawning enemies.
    /// </summary>
    private float enemySpawnTimer;

    /// <summary>
    /// Whether the game is currently in progress or not.
    /// </summary>
    private bool gameInProgress { get; set; } = false;

    public override void StartGame()
    {
        this.CurrentStage.ObjectiveProgress = 50;

        base.StartGame();

        this.gameObjects = new List<GameObject>();

        // Spawn in tower in middle of screen.
        GameObject tower = Instantiate(this.towerPrefab, this.towerSpawnPoint.position, Quaternion.identity, this.towerSpawnPoint);
        this.gameObjects.Add(tower);

        Tower towerComponent = tower.GetComponent<Tower>();
        towerComponent.OnDamageTaken += this.ReceiveFailStrike;

        this.playerOneCombat.OnDestroyEnemy += this.RegisterSuccess;
        this.playerTwoCombat.OnDestroyEnemy += this.RegisterSuccess;

        this.OnObjectiveStart += this.BeginGame;
    }

    private void BeginGame()
    {
        SpawnWaveOfEnemies();

        this.gameInProgress = true;
    }

    private void SpawnWaveOfEnemies()
    {
        foreach (Transform g in this.enemySpawnPoints)
        {
            // Spawn in tower in middle of screen.
            GameObject spawn = Instantiate(this.enemyPrefab, g.position, Quaternion.identity, g);
            this.gameObjects.Add(spawn);

            Debug.Log($"Spawned enemy at position: {g.position}");

            Enemy spawnComponent = spawn.GetComponent<Enemy>();
        }
    }

    /// <summary>
    /// Spawns enemies over time.
    /// </summary>
    private void Update()
    {
        if (!this.gameInProgress)
        {
            return;
        }

        // Increase timer.
        this.enemySpawnTimer += Time.deltaTime;

        // Spawn enemy if timer hit.
        if (this.enemySpawnTimer >= this.enemySpawnInterval)
        {
            this.SpawnEnemy();
            this.enemySpawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // Choose random spawn point.
        int index = Random.Range(0, this.enemySpawnPoints.Count);
        Transform spawnPoint = this.enemySpawnPoints[index];

        if (index == this.enemySpawnPoints.Count - 1)
        {
            Debug.Log("Spawning wave of enemies");
            SpawnWaveOfEnemies();
        }
        else
        {
            // Spawn in enemy at spawn point.
            GameObject spawn = Instantiate(this.enemyPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            this.gameObjects.Add(spawn);

            Debug.Log($"Spawned enemy at position: {spawnPoint.position}");
            Enemy spawnComponent = spawn.GetComponent<Enemy>();
        }
    }

    public override void CleanUp()
    {
        foreach (GameObject obj in this.gameObjects)
        {
            if (obj)
            {
                if (obj.tag != "Tower" && obj != null)
                {
                    Destroy(obj);
                }
            }
        }
    }

    public override void GameEnded()
    {
        this.gameInProgress = false;
        this.CleanUp();
    }
}