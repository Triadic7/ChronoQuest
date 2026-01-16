using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
    /// The sound effects manager.
    /// </summary>
    public SFXManager SFXManager { get; private set; }

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
    /// On game stage location selected.
    /// </summary>
    public event Action<LocationModel> OnLocationSelected;

    /// <summary>
    /// On game start, event that's fired for coop.
    /// </summary>
    public event Action<bool> OnGameStartMultiplayer;

    /// <summary>
    /// On game stage start.
    /// </summary>
    public event Action OnStageEnd;

    /// <summary>
    /// On game stage start.
    /// </summary>
    public event Action OnMainMenu;


    /// <summary>
    /// The current save.
    /// </summary>
    public SaveModel CurrentSave { get; private set; }

    /// <summary>
    /// The game state.
    /// </summary>
    private enum GameState
    {
        MainMenu,
        Dialogue,
        Gameplay,
        OutroDialogue,
        Ending
    }

    /// <summary>
    /// Gets or sets the game state.
    /// </summary>
    private GameState State { get; set; }

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

        // Fire main menu event.
        this.OnMainMenu?.Invoke();

        // Opens main menu.
        this.UIManager.OpenMenu("MainMenu");
    }

    /// <summary>
    /// Starts game and loads dialogue.
    /// </summary>
    /// <param name="save"></param>
    public void StartGame(SaveModel save)
    {
        Debug.Log($"StartGame called for save: {save.SaveID}");
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
        this.DialogueManager.OnDialogueEnded += OnDialogueFinished;

        // Get sfx manager.
        this.SFXManager = GetComponent<SFXManager>();

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

        // Checks and subscribes to events.
        if (this.loadMenu != null)
        {
            this.loadMenu.OnGamePlayButtonHit -= this.StartGame;
            this.loadMenu.OnGamePlayButtonHit += this.StartGame;
        }
        else
        {
            Debug.LogWarning("LoadMenu not found at startup.");
        }

        if (inputManager != null)
        {
            inputManager.OnPause += this.PauseGame;
        }
        else
        {
            Debug.LogWarning("InputManager not found at startup.");
        }

        if (ui != null)
        {
            ui.OnStageTimeElapsed += this.AddTimeToCurrentSave;
        }
        else
        {
            Debug.LogWarning("PlayerGameplayUI not found at startup.");
        }

        // Adds event listners.
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
        Debug.Log($"Added {time} seconds to save {CurrentSave.SaveID}, total: {CurrentSave.PlayTime}");
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
        this.UIManager.OpenMenu("PauseMenu");
        this.IsPaused = true;
        Time.timeScale = 0f;
        this.OnGamePaused?.Invoke();
    }

    /// <summary>
    /// On dialogue finished.
    /// </summary>
    private void OnDialogueFinished()
    {
        // Get the current dialogue.
        DialogueModel dialogue = this.DialogueManager.CurrentDialogue;

        // If it's an outro, finish as usual.
        if (dialogue != null && dialogue.IsOutro)
        {
            OnOutroDialogueFinished();
            return;
        }

        // Otherwise start gameplay if this is an intro dialogue.
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
        // Record stage result.
        CurrentSave.AddStageResult(CurrentSave.CurrentStageId, hasWon);
        SaveManager.SaveProgress(CurrentSave);

        // Show outro for the stage we just finished.
        ChangeState(GameState.OutroDialogue, hasWon);
    }

    /// <summary>
    ///  Changes the game state.
    /// </summary>
    /// <param name="newState">The new state.</param>
    /// <param name="stageWon">If the stage has been won.</param>
    private void ChangeState(GameState newState, bool stageWon = true)
    {
        this.State = newState;
        Debug.Log($"[GameManager] Changing to {newState}.");
        switch (this.State)
        {
            case GameState.MainMenu:
                this.UIManager.OpenMenu("MainMenu");
                break;

            case GameState.Dialogue:
                // Open talk menu.
                this.UIManager.OpenMenu("TalkMenu");

                // Gets the current npc from the stage the player is on.
                NPCModel currentNpc = this.SaveManager.DataContext.GetNPCFromStage(CurrentSave.CurrentStageId);

                // Start dialogue through DialogueManager.
                this.DialogueManager.StartDialogue(currentNpc, this.CurrentSave.Stage);

                // Display location in background.
                if (this.CurrentSave.Stage.Location == null)
                {
                    LocationTable table = this.SaveManager.DataContext.GetLocationByID(this.CurrentSave.Stage.LocationID);
                    this.CurrentSave.Stage.Location = new LocationModel(table, AssetLoader.Resolver);
                }

                // Fire event for selecting the current stage.
                this.OnLocationSelected?.Invoke(this.CurrentSave.Stage.Location);
                break;

            case GameState.Gameplay:
                Debug.Log($"Starting gameplay on stage {this.CurrentSave.Stage.StageID}");
                this.UIManager.CloseMenu("TalkMenu");
                this.OnStageStart?.Invoke();

                // Spawn either single or coop players.
                this.OnGameStartMultiplayer?.Invoke(this.CurrentSave.IsCoop);

                // Start game.
                this.GameStageManager.StartGame(this.CurrentSave.Stage);
                break;

            case GameState.OutroDialogue:
                this.UIManager.OpenMenu("TalkMenu");

                // Restore default music during outro dialogue.
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.PlayDefaultMusic();
                }

                // Select dialogue based on win/loss.
                DialogueModel outroDialogue = stageWon
                    ? this.SaveManager.DataContext.GetOutroDialogueForStage(CurrentSave.CurrentStageId, true)
                    : this.SaveManager.DataContext.GetOutroDialogueForStage(CurrentSave.CurrentStageId, false);

                if (outroDialogue == null)
                {
                    Debug.LogWarning($"No outro found for stage {CurrentSave.CurrentStageId}. Skipping outro.");
                    OnOutroDialogueFinished();
                    return;
                }

                // Start the outro dialogue.
                this.DialogueManager.StartDialogue(outroDialogue);
                break;

            case GameState.Ending:
                this.UIManager.OpenMenu("EndMenu");
                break;
        }
    }

    /// <summary>
    /// On outro dialogue finished, load next dialogue.
    /// </summary>
    private void OnOutroDialogueFinished()
    {
        // Fire stage end event.
        this.OnStageEnd?.Invoke();

        int nextStageId = this.CurrentSave.CurrentStageId + 1;
        StageModel nextStage = this.SaveManager.DataContext.GetStageById(nextStageId);

        if (nextStage == null)
        {
            // No more stages, end game.
            this.ChangeState(GameState.Ending);
            return;
        }

        // Update current save for next stage.
        this.CurrentSave.CurrentStageId = nextStageId;
        this.CurrentSave.Stage = nextStage;
        this.SaveManager.SaveProgress(CurrentSave);

        // Start next stage dialogue.
        this.ChangeState(GameState.Dialogue);
    }

    /// <summary>
    /// On start.
    /// </summary>
    private IEnumerator Start()
    {
        // Wait one frame for stuff to load.
        yield return null;
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
