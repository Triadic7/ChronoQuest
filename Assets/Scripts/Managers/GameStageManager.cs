using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Class for managing which gameplay stage is being shown.
/// </summary>
public class GameStageManager : MonoBehaviour
{
    /// <summary>
    /// The current game.
    /// </summary>
    public Game CurrentGame { get; private set; }

    /// <summary>
    /// Fires after stage is finished.
    /// </summary>
    public event Action<bool> OnStageFinished;

    /// <summary>
    /// Fires after objective is finished, win or lose.
    /// </summary>
    public event Action OnObjectiveStop;

    /// <summary>
    /// Starts the stage, but doesnt call the objective started.
    /// </summary>
    public event Action<StageModel> OnStageStarted;

    /// <summary>
    /// On game set.
    /// </summary>
    public event Action<Game> OnGameSet;

    /// <summary>
    /// Called when the objective is over.
    /// </summary>
    public event Action<Game> OnObjectiveEnd;

    /// <summary>
    /// On game started, so before objective has been called.
    /// </summary>
    public event Action<Game> OnGameStarted;

    /// <summary>
    /// On game started, so the objective has been called.
    /// </summary>
    public event Action<Game> OnGameObjectiveStarted;

    /// <summary>
    /// Fired when game progress made.
    /// </summary>
    public event Action<int, int> OnGameProgressMade;

    /// <summary>
    /// Fired when fail strike received.
    /// </summary>
    public event Action OnGameFailStrike;

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
    /// Delay for stage.
    /// </summary>
    [SerializeField]
    private float stageDelay = 5f;

    /// <summary>
    /// Stage delay coroutine.
    /// </summary>
    private Coroutine stageDelayRoutine;

    /// <summary>
    /// Starts a game.
    /// </summary>
    public void StartGame(StageModel stage)
    {
        // Map to minigame instance.
        switch (stage.StageID)
        {
            case 1:
                this.CurrentGame = patternMinigame; 
                break;
            case 2:
                this.CurrentGame = towerDefenseMinigame; 
                break;
            case 3:
                this.CurrentGame = timeMinigame; 
                break;
            case 4:
                this.CurrentGame = spaceMinigame; 
                break;
            default:
                Debug.LogError($"No minigame for stage {stage.StageID}");
                return;
        }

        // Assign the stage data.
        this.CurrentGame.SetStage(stage);

        // Fire event.
        this.OnGameSet?.Invoke(this.CurrentGame);

        // Subscribe to game end.
        this.CurrentGame.OnGameEnd += this.EndGame;
        this.CurrentGame.OnGameProgress += this.OnGameProgressMade;
        this.CurrentGame.OnFailStrike += this.OnGameFailStrike;

        // Initialize the game by starting it.
        this.CurrentGame.StartGame();

        this.OnStageStarted?.Invoke(stage);
        this.OnGameStarted?.Invoke(this.CurrentGame);

        // Start coroutine then start game.
        this.stageDelayRoutine = StartCoroutine(this.StageDelay(this.CurrentGame.StartGameObjective));

    }

    /// <summary>
    /// Cleans up current game in case the user goes to main menu.
    /// </summary>
    private void CleanUpCurrentGame()
    {
        if (this.stageDelayRoutine != null)
        {
            StopCoroutine(this.stageDelayRoutine);
            this.stageDelayRoutine = null;
        }

        if (this.CurrentGame != null)
        {
            // Unsubscribe from events.
            this.UnsubscribeFromGame();

            // Call cleanup logic.
            this.CurrentGame.CleanUp();

            // Null out reference.
            this.CurrentGame = null;
        }
    }


    /// <summary>
    /// Debug to win the game.
    /// </summary>
    public void DebugWinGame()
    {
        this.CurrentGame.EndGame(true);
    }

    /// <summary>
    /// Cache mini games.
    /// </summary>
    private void Awake()
    {
        this.patternMinigame = GetComponentInChildren<PatternMinigame>(true);
        this.timeMinigame = GetComponentInChildren<TimeMinigame>(true);
        this.towerDefenseMinigame = GetComponentInChildren<TowerDefenseMinigame>(true);
        this.spaceMinigame = GetComponentInChildren<SpaceMinigame>(true);
    }

    /// <summary>
    /// Cache and add event listeners.
    /// </summary>
    private void Start()
    {
        GameManager gm = GameManager.Instance;

        // Clean up current game.
        if (gm == null)
        {
            Debug.Log("GameManager not found.");
            return;
        }

        gm.OnMainMenu += this.CleanUpCurrentGame;
    }

    /// <summary>
    /// Unsubscribes from the current game if present.
    /// </summary>
    private void UnsubscribeFromGame()
    {
        this.OnObjectiveStop?.Invoke();
        if (this.CurrentGame != null)
        {
            // Unsubscribe from game.
            this.CurrentGame.OnGameEnd -= this.EndGame;
            this.CurrentGame.OnGameProgress -= this.OnGameProgressMade;
            this.CurrentGame.OnFailStrike -= this.OnGameFailStrike;
        }
    }

    /// <summary>
    /// Ends a game.
    /// </summary>
    /// <param name="hasWon">If the player has won.</param>
    private void EndGame(bool hasWon)
    {
        this.OnObjectiveEnd?.Invoke(this.CurrentGame);
        this.UnsubscribeFromGame();

        // Start coroutine then cleanup game.
        StartCoroutine(this.StageEndDelay(hasWon));
    }

    /// <summary>
    /// Coroutine to delay the stage start. Then start the game.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StageDelay(Action function)
    {
        yield return new WaitForSeconds(this.stageDelay);
        this.OnGameObjectiveStarted?.Invoke(this.CurrentGame);
        function?.Invoke();
    }

    /// <summary>
    /// The end of stage delay.
    /// </summary>
    /// <param name="hasWon">If the player has end.</param>
    /// <returns>Ends stage after.</returns>
    private IEnumerator StageEndDelay(bool hasWon)
    {
        yield return new WaitForSeconds(stageDelay);
        if(this.CurrentGame != null)
        {
            this.CurrentGame.CleanUp();
            this.OnStageFinished?.Invoke(hasWon);
        }
    }
}
