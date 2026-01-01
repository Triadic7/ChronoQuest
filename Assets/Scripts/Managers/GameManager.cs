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
    /// On Game ready.
    /// </summary>
    public event Action OnGameInitialized;

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
        Settings = GetComponent<SettingsManager>();

        // Get UI manager.
        UIManager = GetComponent<UIManager>();

        // Get save manager.
        SaveManager = GetComponent<SaveManager>();

        // Get scene manager.
        GameStageManager = GetComponent<GameStageManager>();

        // Checks if settings are null.
        if (Settings == null)
        {
            Debug.LogError("GameManager requires a SettingsManager component.");
            return;
        }

        // Get menus.
        this.loadMenu = FindAnyObjectByType<LoadMenu>();
        this.talkMenu = FindAnyObjectByType<TalkMenu>();

        if (this.loadMenu == null || this.talkMenu == null) 
        {
            Debug.LogError("Missing a menu.");
            return;
        }

        // Adds event listners.
        this.loadMenu.OnGamePlayButtonHit += StartGame;
        this.talkMenu.OnDialogueFinished += OnDialogueFinished;
        GameStageManager.OnStageFinished += OnStageFinished;
    }

    /// <summary>
    /// Starts game and loads dialogue.
    /// </summary>
    /// <param name="save"></param>
    private void StartGame(SaveModel save)
    {
        Debug.Log($"StartGame called for save: {save.Id}");
        CurrentSave = save;
        ChangeState(GameState.Dialogue);
    }

    /// <summary>
    /// On dialogue finished.
    /// </summary>
    private void OnDialogueFinished()
    {
        if (State == GameState.Dialogue)
        {
            ChangeState(GameState.Gameplay);
        }
        else if (State == GameState.Gameplay)
        {
            AdvanceStageOrEnd();
        }
    }

    /// <summary>
    /// On game stage finished.
    /// </summary>
    private void OnStageFinished()
    {
        ChangeState(GameState.Dialogue);
    }

    /// <summary>
    ///  Changes the game state.
    /// </summary>
    /// <param name="newState">The new state.</param>
    private void ChangeState(GameState newState)
    {
        Debug.Log($"Changing state from {State} to {newState}");
        State = newState;

        switch (State)
        {
            case GameState.MainMenu:
                UIManager.OpenMenu("MainMenu");
                break;

            case GameState.Dialogue:
                NPCModel currentNpc = SaveManager.DataContext.GetNPCFromStage(CurrentSave.StageId);
                OnDialogueStart?.Invoke(currentNpc, CurrentSave.Stage.StageName);
                UIManager.OpenMenu("TalkMenu");
                break;

            case GameState.Gameplay:
                UIManager.CloseMenu("TalkMenu");
                GameStageManager.StartGame(this.CurrentSave.StageId);
                break;

            case GameState.Ending:
                UIManager.OpenMenu("EndMenu");
                break;
        }
    }

    /// <summary>
    /// Checks for further stages, or ends the game.
    /// </summary>
    private void AdvanceStageOrEnd()
    {
        // Checks if stage is the final one.
        if (CurrentSave.Stage.IsFinalStage)
        {
            ChangeState(GameState.Ending);
            return;
        }

        // Changes current stage to the next one.
        CurrentSave.StageId = CurrentSave.Stage.StageId + 1;
        CurrentSave.Stage = SaveManager.DataContext.GetStageById(CurrentSave.StageId);

        // Saves progress after stage.
        SaveManager.SaveProgress(CurrentSave);

        // Change state to next dialogue.
        ChangeState(GameState.Dialogue);
    }


    /// <summary>
    /// On start.
    /// </summary>
    private void Start()
    {
        this.OnGameInitialized?.Invoke();
        LoadMenu();
    }

    /// <summary>
    /// Starts game by showing main menu.
    /// </summary>
    private void LoadMenu()
    {
        UIManager.OpenMenu("MainMenu");
    }
}
