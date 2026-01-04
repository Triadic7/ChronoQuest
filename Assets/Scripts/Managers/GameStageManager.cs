using System;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Class for managing which gameplay stage is being shown.
/// </summary>
public class GameStageManager : MonoBehaviour
{
    /// <summary>
    /// Fires after stage is finished.
    /// </summary>
    public event Action<bool> OnStageFinished;

    /// <summary>
    /// Starts the stage.
    /// </summary>
    public event Action<StageModel> OnStageStarted;

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
    public void StartGame(StageModel stage)
    {
        // Map to minigame instance.
        switch (stage.StageId)
        {
            case 1: 
                currentGame = patternMinigame; 
                break;
            case 2: 
                currentGame = towerDefenseMinigame; 
                break;
            case 3: 
                currentGame = timeMinigame; 
                break;
            case 4: 
                currentGame = spaceMinigame; 
                break;
            default:
                Debug.LogError($"No minigame for stage {stage.StageId}");
                return;
        }

        // Assign the stage data.
        currentGame.SetStage(stage);

        // Subscribe to game end.
        this.currentGame.OnGameEnd += this.EndGame;

        // Initialize the game by starting it.
        this.currentGame.StartGame();

        this.OnStageStarted?.Invoke(stage);

        // Start coroutine for 5 - 10 seconds.

        // Start game objective.
    }

    /// <summary>
    /// Debug to win the game.
    /// </summary>
    public void DebugWinGame()
    {
        this.currentGame.EndGame();
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
    /// Ends a game.
    /// </summary>
    /// <param name="hasWon">If the player has won.</param>
    private void EndGame(bool hasWon)
    {
        // Unsubscribe from game.
        this.currentGame.OnGameEnd -= this.EndGame;

        // Start coroutine for 5 - 10 seconds.

        // Clean up game and fire event.
        this.OnStageFinished?.Invoke(hasWon);
    }
}
