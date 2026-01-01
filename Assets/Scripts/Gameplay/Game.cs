using UnityEngine;

/// <summary>
/// Parent game class.
/// </summary>
public abstract class Game : MonoBehaviour
{
    /// <summary>
    /// Starts game.
    /// </summary>
    public abstract void StartGame();

    /// <summary>
    /// Ends game.
    /// </summary>
    public abstract void EndGame();
}