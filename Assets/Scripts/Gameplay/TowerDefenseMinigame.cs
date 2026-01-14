using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseMinigame : Game
{
    [Header("SFX")]
    [SerializeField]
    private AudioClip towerHitSfx;

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
    /// The player one combat componant.
    /// </summary>
    [SerializeField]
    private PlayerCombat playerOneCombat;

    /// <summary>
    /// Player two combat componant.
    /// </summary>
    [SerializeField]
    private PlayerCombat playerTwoCombat;

    /// <summary>
    /// The enemy spawner.
    /// </summary>
    [SerializeField]
    private Spawner enemySpawner;

    /// <summary>
    /// The tower instance.
    /// </summary>
    private GameObject towerInstance;

    /// <summary>
    /// Event for subscribing and unsubscribing to spawn.
    /// </summary>
    private Action<GameObject> onEnemySpawned;

    /// <summary>
    /// Starts game and instances tower and subscribes to events.
    /// </summary>
    public override void StartGame()
    {
        base.StartGame();

        // Spawn in tower in middle of screen.
        GameObject tower = Instantiate(this.towerPrefab, this.towerSpawnPoint.position, Quaternion.identity, this.towerSpawnPoint);
        this.towerInstance = tower;

        // Add event to tower component.
        Tower towerComponent = tower.GetComponent<Tower>();
        towerComponent.OnDamageTaken += this.OnTowerDamaged;

        // Register success when player destroys enemy.
        this.playerOneCombat.OnDestroyEnemy += this.RegisterSuccess;
        this.playerTwoCombat.OnDestroyEnemy += this.RegisterSuccess;

        // Allows players to harm enemies.
        this.playerOneCombat.EnableHurtEnemies();
        this.playerTwoCombat.EnableHurtEnemies();

        // Disable player getting hurt.
        this.playerOneCombat.GetComponent<Player>().DisableCanBeHurt();
        this.playerTwoCombat.GetComponent<Player>().DisableCanBeHurt();

        // Set on enemy spawned to chase tower.
        this.onEnemySpawned = (go) =>
        {
            ChaseMovement chase = go.GetComponent<ChaseMovement>();

            // Make this target the tower.
            chase.SetTargetFunction(() => this.towerInstance.transform);
        };

        // Add event to enemy spawner.
        this.enemySpawner.OnObjectInstanced += this.onEnemySpawned;
    }

    /// <summary>
    /// Starts the game objective and spawns in enemies.
    /// </summary>
    public override void StartGameObjective()
    {
        base.StartGameObjective();

        this.enemySpawner.StartSpawning();
    }

    /// <summary>
    /// Handles logic to be executed when the tower takes damage, including playing a hit sound effect and registering a
    /// failed strike.
    /// </summary>
    private void OnTowerDamaged()
    {
        if (this.towerHitSfx != null)
        {
            SoundManager.Instance?.Play(this.towerHitSfx);
        }

        this.ReceiveFailStrike();
    }

    /// <summary>
    /// Destroy tower instance.
    /// </summary>
    public override void CleanUp()
    {
        this.enemySpawner.StopSpawningAndDestroyAll();
    }

    /// <summary>
    /// On game ended.
    /// </summary>
    public override void GameEnded()
    {
        // Unsubscribe from event.
        this.playerOneCombat.OnDestroyEnemy -= this.RegisterSuccess;
        this.playerTwoCombat.OnDestroyEnemy -= this.RegisterSuccess;

        // Removes players option to harm enemies.
        this.playerOneCombat.DisableHurtEnemies();
        this.playerTwoCombat.DisableHurtEnemies();

        // Unsubscribe from events and destroy tower instance.
        Tower towerComponent = towerInstance.GetComponent<Tower>();
        towerComponent.OnDamageTaken -= this.OnTowerDamaged;

        // Destroy tower instance.
        Destroy(towerInstance);

        // Clear events.
        if (this.onEnemySpawned != null)
        {
            this.enemySpawner.OnObjectInstanced -= this.onEnemySpawned;
            this.onEnemySpawned = null;
        }

        this.CleanUp();
    }
}