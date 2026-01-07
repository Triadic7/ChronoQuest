using System;
using UnityEngine;

/// <summary>
/// The class for managing the game for things like settings and game events.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Gets the settings manager.
    /// </summary>
    public SettingsManager Settings { get; private set; }

    /// <summary>
    /// Gets the UI manager.
    /// </summary>
    public UIManager UIManager { get; private set; }

    /// <summary>
    /// Gets the save manager.
    /// </summary>
    public SaveManager SaveManager { get; private set; }

    /// <summary>
    /// Gets the game scene manager.
    /// </summary>
    public GameStageManager GameStageManager { get; private set; }

    /// <summary>
    /// Gets the dialogue manager.
    /// </summary>
    public DialogueManager DialogueManager { get; private set; }

    /// <summary>
    /// On Game ready.
    /// </summary>
    public event Action OnGameInitialized;

    /// <summary>
    /// On game paused.
    /// </summary>
    public event Action OnGamePaused;

    /// <summary>
    /// On game resumed.
    /// </summary>
    public event Action OnGameResumed;

    /// <summary>
    /// On game stage start.
    /// </summary>
    public event Action OnStageStart;

    /// <summary>
    /// On game start, event that's fired for coop.
    /// </summary>
    public event Action<bool> OnGameStartMultiplayer;

    /// <summary>
    /// On game stage start.
    /// </summary>
    public event Action OnStageEnd;

    /// <summary>
    /// On dialogue start.
    /// </summary>
    public event Action<NPCModel, string> OnDialogueStart;

    /// <summary>
    /// The game state.
    /// </summary>
    private enum GameState
    {
        MainMenu,
        Dialogue,
        Gameplay,
        Ending
    }

    /// <summary>
    /// Gets or sets the game state.
    /// </summary>
    private GameState State { get; set; }

    /// <summary>
    /// The current save.
    /// </summary>
    private SaveModel CurrentSave { get; set; }

    /// <summary>
    /// The load menu.
    /// </summary>
    private LoadMenu loadMenu;

    /// <summary>
    /// If the game is paused.
    /// </summary>
    public bool IsPaused { get; private set; }

    /// <summary>
    /// Resumes game.
    /// </summary>
    public void ResumeGame()
    {
        // Return if game not paused.
        if (!this.IsPaused)
        {
            return;
        }

        this.IsPaused = false;
        Time.timeScale = 1f;
        this.OnGameResumed?.Invoke();
        this.UIManager.CloseMenu("PauseMenu");
    }

    /// <summary>
    /// Quits to main menu.
    /// </summary>
    public void QuitToMainMenu()
    {
        // Checks if game is paused and resumes time scale.
        if (this.IsPaused)
        {
            this.IsPaused = false;
            Time.timeScale = 1f;
        }

        // Opens main menu.
        this.UIManager.OpenMenu("MainMenu");
    }

    /// <summary>
    /// Starts game and loads dialogue.
    /// </summary>
    /// <param name="save"></param>
    public void StartGame(SaveModel save)
    {
        Debug.Log($"StartGame called for save: {save.Id}");
        this.CurrentSave = save;
        this.ChangeState(GameState.Dialogue);
    }

    /// <summary>
    /// Fires once before start.
    /// </summary>
    private void Awake()
    {
        // If instance isn't null, destroy and return.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Set instance to this.
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get settings manager.
        this.Settings = GetComponent<SettingsManager>();

        // Get UI manager.
        this.UIManager = GetComponent<UIManager>();

        // Get save manager.
        this.SaveManager = GetComponent<SaveManager>();

        // Get scene manager.
        this.GameStageManager = GetComponent<GameStageManager>();

        // Get dialogue manager.
        this.DialogueManager = GetComponent<DialogueManager>();

        // Checks if settings are null.
        if (this.Settings == null)
        {
            Debug.LogError("GameManager requires a SettingsManager component.");
            return;
        }

        // Get menus.
        this.loadMenu = FindAnyObjectByType<LoadMenu>();

        PlayerGameplayUI ui = FindAnyObjectByType<PlayerGameplayUI>();

        InputManager inputManager = FindAnyObjectByType<InputManager>();

        if (this.loadMenu == null || inputManager == null || ui == null) 
        {
            Debug.LogError("Missing a dependency.");
            return;
        }

        // Adds event listners.
        this.loadMenu.OnGamePlayButtonHit += this.StartGame;
        inputManager.OnPause += this.PauseGame;
        ui.OnStageTimeElapsed += this.AddTimeToCurrentSave;
        this.GameStageManager.OnStageFinished += this.OnStageFinished;
    }

    /// <summary>
    /// Adds the time to a current save.
    /// </summary>
    /// <param name="time">The time to add.</param>
    private void AddTimeToCurrentSave(float time)
    {
        if (this.CurrentSave == null)
        {
            return;
        }

        this.CurrentSave.PlayTime += time;
        this.SaveManager.SaveProgress(CurrentSave);
        Debug.Log($"Added {time} seconds to save {CurrentSave.Id}, total: {CurrentSave.PlayTime}");
    }

    /// <summary>
    /// Toggles pause on the game.
    /// </summary>
    private void PauseGame()
    {
        // Only pause during gameplay.
        if (this.State != GameState.Gameplay)
        {
            return;
        }

        // Toggles the pause if already paused.
        if (this.IsPaused)
        {
            this.ResumeGame();
        }
        else
        {
            this.DoPause();
        }
    }

    /// <summary>
    /// Sends event to pause game.
    /// </summary>
    private void DoPause()
    {
        this.IsPaused = true;
        Time.timeScale = 0f;
        this.OnGamePaused?.Invoke();
        this.UIManager.OpenMenu("PauseMenu");
    }

    /// <summary>
    /// On dialogue finished.
    /// </summary>
    private void OnDialogueFinished()
    {
        if (this.State == GameState.Dialogue)
        {
            this.ChangeState(GameState.Gameplay);
        }
    }

    /// <summary>
    /// On game stage finished.
    /// </summary>
    /// <param name="hasWon">If the player has won.</param>
    private void OnStageFinished(bool hasWon)
    {
        this.OnStageEnd?.Invoke();
        this.AdvanceStageOrEnd(hasWon);
    }

    /// <summary>
    ///  Changes the game state.
    /// </summary>
    /// <param name="newState">The new state.</param>
    private void ChangeState(GameState newState)
    {
        Debug.Log($"Changing state from {State} to {newState}");
        this.State = newState;

        switch (this.State)
        {
            case GameState.MainMenu:
                this.UIManager.OpenMenu("MainMenu");
                break;

            case GameState.Dialogue:
                // Subscribe to dialogue finished.
                this.DialogueManager.OnDialogueEnded -= OnDialogueFinished;
                this.DialogueManager.OnDialogueEnded += OnDialogueFinished;

                // Open talk menu.
                this.UIManager.OpenMenu("TalkMenu");

                // Gets the current npc from the stage the player is on.
                NPCModel currentNpc = this.SaveManager.DataContext.GetNPCFromStage(CurrentSave.StageId);

                // Start dialogue through DialogueManager.
                this.DialogueManager.StartDialogue(currentNpc, this.CurrentSave.Stage);
                break;

            case GameState.Gameplay:
                this.UIManager.CloseMenu("TalkMenu");
                this.OnStageStart?.Invoke();
                this.OnGameStartMultiplayer?.Invoke(this.CurrentSave.IsCoop);
                this.GameStageManager.StartGame(this.CurrentSave.Stage);
                break;

            case GameState.Ending:
                this.UIManager.OpenMenu("EndMenu");
                break;
        }
    }

    /// <summary>
    /// Checks for further stages, or ends the game.
    /// </summary>
    /// <param name="hasWon">If the players have won.</param>
    private void AdvanceStageOrEnd(bool hasWon)
    {
        // Store to check if stage exists.
        int nextStageId = this.CurrentSave.StageId + 1;

        // Add win to save.

        // Check if the next stage exists.
        StageModel nextStage = this.SaveManager.DataContext.GetStageById(nextStageId);

        // If no next stage, end game.
        if (nextStage == null)
        {
            this.ChangeState(GameState.Ending);
            return;
        }

        // Otherwise advance normally.
        this.CurrentSave.StageId = nextStageId;
        this.CurrentSave.Stage = nextStage;

        // Save progress.
        this.SaveManager.SaveProgress(CurrentSave);

        // Go to next dialogue.
        this.ChangeState(GameState.Dialogue);
    }

    /// <summary>
    /// On start.
    /// </summary>
    private void Start()
    {
        this.OnGameInitialized?.Invoke();
        this.LoadMenu();
    }

    /// <summary>
    /// Starts game by showing main menu.
    /// </summary>
    private void LoadMenu()
    {
        this.UIManager.OpenMenu("MainMenu");
    }
}
