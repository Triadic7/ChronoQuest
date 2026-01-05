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
    /// Fired after OnGameEnd, to cleanup game.
    /// </summary>
    public event Action OnGameCleanup;

    /// <summary>
    /// When the games objective starts.
    /// </summary>
    public event Action OnObjectiveStart;

    /// <summary>
    /// When progress has been made.
    /// </summary>
    public event Action<int, int> OnGameProgress;

    /// <summary>
    /// When fail strike recieved.
    /// </summary>
    public event Action OnFailStrike;

    /// <summary>
    /// The count of how many points to win the game.
    /// </summary>
    public int PointsToWin { get; protected set; }

    /// <summary>
    /// How many times the player can fail until they lose the stage.
    /// </summary>
    public int FailureChances { get; protected set; } = 3;

    /// <summary>
    /// The current stage data.
    /// </summary>
    public StageModel CurrentStage { get; private set; }

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
        this.OnObjectiveStart?.Invoke();
    }

    /// <summary>
    /// This should be where you remove non enemy things, such as tower defense tower, or pressure plates.
    /// </summary>
    public abstract void CleanUp();

    /// <summary>
    /// Game ended, this is where you remove enemies.
    /// </summary>
    public abstract void GameEnded();

    /// <summary>
    /// Ends game. This should be where you remove enemies.
    /// </summary>
    public void EndGame()
    {
        this.GameEnded();
        this.OnGameEnd?.Invoke(this.wonGame);
    }

    /// <summary>
    /// Receive a failure strike.
    /// </summary>
    protected void ReceiveFailStrike()
    {
        this.OnFailStrike?.Invoke();
        this.currentFailureCount++;
        this.CheckForFailure();
    }

    /// <summary>
    /// Get progress and check for success.
    /// </summary>
    protected void RegisterSuccess()
    {
        this.currentSuccessCount++;
        this.OnGameProgress?.Invoke(this.currentSuccessCount, this.CurrentStage.ObjectiveProgress);
        this.CheckForSuccess();
    }

    /// <summary>
    /// Sets the inital values for the game.
    /// </summary>
    private void InitializeGame()
    {
        this.wonGame = false;
        this.currentFailureCount = 0;
        this.currentSuccessCount = 0;
        this.PointsToWin = this.CurrentStage.ObjectiveProgress;
    }

    /// <summary>
    /// Checks if player has lost upon receiving a failure.
    /// </summary>
    /// <returns></returns>
    private void CheckForFailure()
    {
        Debug.Log($"Current fails: {this.currentFailureCount}/{this.FailureChances}");
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
        Debug.Log($"Current success: {this.currentSuccessCount}/{this.PointsToWin}");
        if (this.currentSuccessCount >= this.PointsToWin)
        {
            this.wonGame = true;
            this.EndGame();
        }
    }
}