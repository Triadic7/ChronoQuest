using System;
using UnityEngine;

/// <summary>
/// Class for managing which gameplay stage is being shown.
/// </summary>
public class GameStageManager : MonoBehaviour
{
    /// <summary>
    /// Fires after stage is finished.
    /// </summary>
    public event Action OnStageFinished;

    /// <summary>
    /// The pattern minigame.
    /// </summary>
    private PatternMinigame patternMinigame;

    /// <summary>
    /// The current game.
    /// </summary>
    private Game currentGame;

    /// <summary>
    /// Starts a game.
    /// </summary>
    public void StartGame(int gameId)
    {
        if(gameId == 1)
        {
            this.currentGame = this.patternMinigame;
        }
        this.currentGame.StartGame();
    }

    /// <summary>
    /// Cache mini games.
    /// </summary>
    private void Awake()
    {
        this.patternMinigame = GetComponentInChildren<PatternMinigame>();
    }

    /// <summary>
    /// Loads a game for the player to play.
    /// </summary>
    /// <param name="game">The game to play.</param>
    private void LoadGame(Game game)
    {
        this.currentGame = game;
    }

    /// <summary>
    /// Ends a game.
    /// </summary>
    private void EndGame()
    {
        this.currentGame.EndGame();
        this.OnStageFinished?.Invoke();
    }
}
