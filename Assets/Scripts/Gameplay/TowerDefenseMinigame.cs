using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseMinigame : Game
{
    // Make enemies ignore players.

    // Spawn enemies on edges of the map and move towards tower.

    // Players can walk into enemies to remove them.

    // Enemies touching the tower will lower progress bar.

    // Defeating enemies will increase progress bar.

    /// <summary>
    /// The list of objects to destroy at the end of the game.
    /// </summary>
    private List<GameObject> gameObjects;

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

    public override void StartGame()
    {
        base.StartGame();

        this.gameObjects = new List<GameObject>();

        // Spawn in tower in middle of screen.
        GameObject tower = Instantiate(this.towerPrefab, this.towerSpawnPoint.position, Quaternion.identity, this.towerSpawnPoint);
        this.gameObjects.Add(tower);

        Tower towerComponent = tower.GetComponent<Tower>();
        towerComponent.OnDamageTaken += this.ReceiveFailStrike;

        SpawnTestEnemies();
    }

    private void SpawnTestEnemies()
    {
        foreach (Transform g in this.enemySpawnPoints)
        {
            // Spawn in tower in middle of screen.
            GameObject spawn = Instantiate(this.enemyPrefab, g.position, Quaternion.identity, g);
            this.gameObjects.Add(spawn);

            EnemyScript spawnComponent = spawn.GetComponent<EnemyScript>();
            // spawnComponent.TakeDamage += this.RegisterSuccess;
        }
    }

    /// <summary>
    /// Remove all enemies.
    /// </summary>
    public override void CleanUp()
    {
        
    }

    public override void GameEnded()
    {
        
    }
}
