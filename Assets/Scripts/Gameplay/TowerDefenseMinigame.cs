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

            Debug.Log($"Spawned enemy at position: {g.position}");

            Enemy spawnComponent = spawn.GetComponent<Enemy>();
            // spawnComponent.TakeDamage += this.RegisterSuccess;
        }
    }

    // Spawn wave of enemies

    public override void CleanUp()
    {

    }

    public override void GameEnded()
    {

    }
}