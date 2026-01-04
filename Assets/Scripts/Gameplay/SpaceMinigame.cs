using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The space game where players have to shoot a boss while defending a space station from attacks.
/// </summary>
public class SpaceMinigame : Game
{
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

    // Spawn in boss.

    // Dealing damage to boss increases progress bar.

    // Spawn players as ships that shoot constantly up towards enemies.

    // Enemies move down and if they reach the bottom (space station) get strike.

    // Spawn astroids players can fly into or shoot to fling or destroy.

    // Remove astroids and enemies.

    public override void StartGame()
    {
        // Spawn in boss.
    }

    /// <summary>
    /// Remove any gameObjects from the list and destroy them.
    /// </summary>
    public override void CleanUp()
    {
        
    }
}
