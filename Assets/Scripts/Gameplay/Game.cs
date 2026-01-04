using System;
using UnityEngine;

/// <summary>
/// Parent game class.
/// </summary>
public abstract class Game : MonoBehaviour
{
    /// <summary>
    /// On game end, fires event using WonGame.
    /// </summary>
    public event Action<bool> OnGameEnd;

    /// <summary>
    /// The count of how many points to win the game.
    /// </summary>
    public int PointsToWin { get; private set; }

    /// <summary>
    /// How many times the player can fail until they lose the stage.
    /// </summary>
    public int FailureChances { get; private set; }

    /// <summary>
    /// The current stage data.
    /// </summary>
    protected StageModel CurrentStage { get; private set; }

    /// <summary>
    /// Whether the game has been won or not.
    /// </summary>
    private bool wonGame;

    /// <summary>
    /// How many fails the player currently has.
    /// </summary>
    private int currentFailureCount;

    /// <summary>
    /// How many points the player has.
    /// </summary>
    private int currentSuccessCount;

    /// <summary>
    /// Sets the stage.
    /// </summary>
    /// <param name="stage">The stage data.</param>
    public void SetStage(StageModel stage)
    {
        this.CurrentStage = stage;
    }

    /// <summary>
    /// Starts game.
    /// </summary>
    public virtual void StartGame()
    {
        this.InitializeGame();
    }

    /// <summary>
    /// Starts the objective in the game.
    /// </summary>
    public virtual void StartGameObjective()
    {

    }

    /// <summary>
    /// This should be where you remove non enemy things, such as tower defense tower, or pressure plates.
    /// </summary>
    public abstract void CleanUp();

    /// <summary>
    /// Ends game. This should be where you remove enemies.
    /// </summary>
    public void EndGame()
    {
        this.OnGameEnd?.Invoke(this.wonGame);
        this.ResetVariables();
    }

    /// <summary>
    /// Receive a failure strike.
    /// </summary>
    protected void ReceiveFailStrike()
    {
        this.currentFailureCount++;
        this.CheckForFailure();
    }

    /// <summary>
    /// Get progress and check for success.
    /// </summary>
    protected void RegisterSuccess()
    {
        this.currentSuccessCount++;
        this.CheckForSuccess();
    }

    /// <summary>
    /// Resets failure and progress bars.
    /// </summary>
    protected virtual void ResetVariables()
    {
        this.InitializeGame();

        // Clean up rest of game.
        this.CleanUp();
    }

    /// <summary>
    /// Sets the inital values for the game.
    /// </summary>
    private void InitializeGame()
    {
        this.wonGame = false;
        this.currentFailureCount = 0;
        this.currentSuccessCount = 0;
    }

    /// <summary>
    /// Checks if player has lost upon receiving a failure.
    /// </summary>
    /// <returns></returns>
    private void CheckForFailure()
    {
        if(this.currentFailureCount >= this.FailureChances)
        {
            this.wonGame = false;
            this.EndGame();
        }
    }

    /// <summary>
    /// Checks if player has won upon getting progress.
    /// </summary>
    private void CheckForSuccess()
    {
        if (this.currentSuccessCount >= this.PointsToWin)
        {
            this.wonGame = true;
            this.EndGame();
        }
    }
}