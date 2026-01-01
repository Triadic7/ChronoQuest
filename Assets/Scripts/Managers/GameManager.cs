using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    /// On Game ready.
    /// </summary>
    public event Action OnGameInitialized;

    /// <summary>
    /// On game paused.
    /// </summary>
    public event Action OnGamePaused;

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
    /// The talk menu.
    /// </summary>
    private TalkMenu talkMenu;

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

        // Checks if settings are null.
        if (this.Settings == null)
        {
            Debug.LogError("GameManager requires a SettingsManager component.");
            return;
        }

        // Get menus.
        this.loadMenu = FindAnyObjectByType<LoadMenu>();
        this.talkMenu = FindAnyObjectByType<TalkMenu>();

        InputManager inputManager = FindAnyObjectByType<InputManager>();

        if (this.loadMenu == null || this.talkMenu == null || inputManager == null) 
        {
            Debug.LogError("Missing a menu or input manager.");
            return;
        }

        // Adds event listners.
        this.loadMenu.OnGamePlayButtonHit += StartGame;
        this.talkMenu.OnDialogueFinished += OnDialogueFinished;
        inputManager.OnPause += PauseGame;
        this.GameStageManager.OnStageFinished += OnStageFinished;
    }

    /// <summary>
    /// Starts game and loads dialogue.
    /// </summary>
    /// <param name="save"></param>
    private void StartGame(SaveModel save)
    {
        Debug.Log($"StartGame called for save: {save.Id}");
        this.CurrentSave = save;
        this.ChangeState(GameState.Dialogue);
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
        else if (this.State == GameState.Gameplay)
        {
            this.AdvanceStageOrEnd();
        }
    }

    /// <summary>
    /// On game stage finished.
    /// </summary>
    private void OnStageFinished()
    {
        this.ChangeState(GameState.Dialogue);
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
                NPCModel currentNpc = SaveManager.DataContext.GetNPCFromStage(CurrentSave.StageId);
                this.OnDialogueStart?.Invoke(currentNpc, CurrentSave.Stage.StageName);
                this.UIManager.OpenMenu("TalkMenu");
                break;

            case GameState.Gameplay:
                this.UIManager.CloseMenu("TalkMenu");
                this.GameStageManager.StartGame(this.CurrentSave.StageId);
                break;

            case GameState.Ending:
                this.UIManager.OpenMenu("EndMenu");
                break;
        }
    }

    /// <summary>
    /// Checks for further stages, or ends the game.
    /// </summary>
    private void AdvanceStageOrEnd()
    {
        // Checks if stage is the final one.
        if (this.CurrentSave.Stage.IsFinalStage)
        {
            this.ChangeState(GameState.Ending);
            return;
        }

        // Changes current stage to the next one.
        this.CurrentSave.StageId = CurrentSave.Stage.StageId + 1;
        this.CurrentSave.Stage = SaveManager.DataContext.GetStageById(CurrentSave.StageId);

        // Saves progress after stage.
        this.SaveManager.SaveProgress(CurrentSave);

        // Change state to next dialogue.
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
