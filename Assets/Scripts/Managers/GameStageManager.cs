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
    /// The time game.
    /// </summary>
    private TimeMinigame timeMinigame;

    /// <summary>
    /// The tower defense minigame.
    /// </summary>
    private TowerDefenseMinigame towerDefenseMinigame;

    /// <summary>
    /// The space minigame.
    /// </summary>
    private SpaceMinigame spaceMinigame;

    /// <summary>
    /// The current game.
    /// </summary>
    private Game currentGame;

    /// <summary>
    /// Starts a game.
    /// </summary>
    public void StartGame(int gameId)
    {
        if (gameId == 1)
        {
            this.currentGame = this.patternMinigame;
        }
        else if (gameId == 2) 
        {
            this.currentGame = this.timeMinigame;
        }
        else if (gameId == 3)
        {
            this.currentGame = this.towerDefenseMinigame;
        }
        else if (gameId == 4)
        {
            this.currentGame = this.spaceMinigame;
        }
        this.currentGame.StartGame();
    }

    /// <summary>
    /// Debug to win the game.
    /// </summary>
    public void DebugWinGame()
    {
        this.EndGame();
    }

    /// <summary>
    /// Cache mini games.
    /// </summary>
    private void Awake()
    {
        this.patternMinigame = GetComponentInChildren<PatternMinigame>();
        this.timeMinigame = GetComponentInChildren<TimeMinigame>();
        this.towerDefenseMinigame = GetComponentInChildren<TowerDefenseMinigame>();
        this.spaceMinigame = GetComponentInChildren<SpaceMinigame>();
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
