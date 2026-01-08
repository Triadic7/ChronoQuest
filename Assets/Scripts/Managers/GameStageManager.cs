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
    /// Fires after stage is finished.
    /// </summary>
    public event Action<bool> OnStageFinished;

    /// <summary>
    /// Fires after objective is finished, win or lose.
    /// </summary>
    public event Action OnObjectiveStop;

    /// <summary>
    /// Starts the stage.
    /// </summary>
    public event Action<StageModel> OnStageStarted;

    /// <summary>
    /// On stage set.
    /// </summary>
    public event Action<Game> OnGameSet;

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
    /// The current game.
    /// </summary>
    private Game currentGame;

    /// <summary>
    /// Delay for stage.
    /// </summary>
    [SerializeField]
    private float stageDelay = 5f;

    /// <summary>
    /// Starts a game.
    /// </summary>
    public void StartGame(StageModel stage)
    {
        // Map to minigame instance.
        switch (stage.StageId)
        {
            case 1:
                this.currentGame = patternMinigame; 
                break;
            case 2:
                this.currentGame = towerDefenseMinigame; 
                break;
            case 3:
                this.currentGame = timeMinigame; 
                break;
            case 4:
                this.currentGame = spaceMinigame; 
                break;
            default:
                Debug.LogError($"No minigame for stage {stage.StageId}");
                return;
        }

        // Assign the stage data.
        this.currentGame.SetStage(stage);

        // Fire event.
        this.OnGameSet?.Invoke(this.currentGame);

        // Subscribe to game end.
        this.currentGame.OnGameEnd += this.EndGame;
        this.currentGame.OnGameProgress += this.OnGameProgressMade;
        this.currentGame.OnFailStrike += this.OnGameFailStrike;

        // Initialize the game by starting it.
        this.currentGame.StartGame();

        this.OnStageStarted?.Invoke(stage);

        // Start coroutine then start game.
        StartCoroutine(this.StageDelay(this.currentGame.StartGameObjective));

    }

    /// <summary>
    /// Cleans up current game in case the user goes to main menu.
    /// </summary>
    public void CleanUpCurrentGame()
    {
        if (this.currentGame != null)
        {
            // Unsubscribe from events.
            this.currentGame.OnGameEnd -= this.EndGame;
            this.currentGame.OnGameProgress -= this.OnGameProgressMade;
            this.currentGame.OnFailStrike -= this.OnGameFailStrike;

            this.OnObjectiveStop?.Invoke();

            // Call cleanup logic.
            this.currentGame.CleanUp();

            // Null out reference.
            this.currentGame = null;
        }
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
    /// Ends a game.
    /// </summary>
    /// <param name="hasWon">If the player has won.</param>
    private void EndGame(bool hasWon)
    {
        // Unsubscribe from game.
        this.currentGame.OnGameEnd -= this.EndGame;
        this.currentGame.OnGameProgress -= this.OnGameProgressMade;
        this.currentGame.OnFailStrike -= this.OnGameFailStrike;

        // Start coroutine then cleanup game.
        StartCoroutine(this.StageEndDelay(hasWon));
    }

    /// <summary>
    /// Coroutine to delay the stage.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StageDelay(Action function)
    {
        yield return new WaitForSeconds(this.stageDelay);
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
        this.currentGame.CleanUp();
        this.OnStageFinished?.Invoke(hasWon);
    }
}
