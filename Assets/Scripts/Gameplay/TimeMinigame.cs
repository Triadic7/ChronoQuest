using UnityEngine;

/// <summary>
/// Objects will spawn transparent and become more solid over time. If object becomes fully solid, spawn enemies to knock players around. Each spawn gives a failure.
/// </summary>
public class TimeMinigame : Game
{
    // Objects start green => change to red over time.

    // If players dont grab objects in time => spawn enemies from red objects.

    // Grabbing green objects fills progress abr.

    // Enemies touching players will lower progress bar.

    public override void StartGame()
    {
        // Spawn in objects over time.
    }

    /// <summary>
    /// Remove any spawned objects.
    /// </summary>
    public override void CleanUp()
    {
        
    }

    public override void GameEnded()
    {
        
    }
}
