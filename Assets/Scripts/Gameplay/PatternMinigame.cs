using UnityEngine;

/// <summary>
/// Pattern minigame that tasks players to match the pattern given.
/// </summary>
public class PatternMinigame : Game
{
    // On game start, display pressure plates players can walk on.

    // Store x count of plates for pattern.

    // Play pattern.

    // On correct pattern played => fill progress bar.

    // On incorrect pattern played, give one strike.

    // Win or lose => EndGame();

    public override void StartGame()
    {
        Debug.Log("Starting pattern game!");
        // Display plates.
    }

    /// <summary>
    /// Remove plates and enemies.
    /// </summary>
    public override void CleanUp()
    {
        
    }

    private void Awake()
    {
        // subscribe to plates.
    }
}
