using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    /// <summary>
    /// The resume button for last save.
    /// </summary>
    [SerializeField]
    private Button resumeButton;

    /// <summary>
    /// Settings button.
    /// </summary>
    [SerializeField] 
    private Button settingsButton;

    /// <summary>
    /// Load button.
    /// </summary>
    [SerializeField] 
    private Button loadButton;

    /// <summary>
    /// Quits application.
    /// </summary>
    public void QuitGame()
    {
        // If running in the editor.
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If running as a standalone build.
        Application.Quit();
        #endif
    }

    /// <summary>
    /// Refreshes last played game.
    /// </summary>
    public override void Open()
    {
        base.Open();

        // Refresh resume button.
        this.RefreshResumeButton();
    }

    /// <summary>
    /// On start.
    /// </summary>
    private void Start()
    {
        UIManager uIManager = GameManager.Instance.UIManager;

        // Adds events for menu buttons.
        if(uIManager != null)
        {
            this.settingsButton.onClick.AddListener(() => uIManager.OpenMenu("SettingsMenu"));
            this.loadButton.onClick.AddListener(() => uIManager.OpenMenu("LoadMenu"));
        }
        else
        {
            Debug.LogError("No UIManager found on GameManager.");
        }
    }

    /// <summary>
    /// Refreshes the Resume button based on the last played save.
    /// </summary>
    private void RefreshResumeButton()
    {
        // Get last save.
        SaveModel lastSave = GameManager.Instance.SaveManager.DataContext.GetLastPlayedSave();

        // Clear events on resume button.
        this.resumeButton.onClick.RemoveAllListeners();

        // If last save isnt null, show resume button. Else hide it.
        if (lastSave != null)
        {
            this.resumeButton.interactable = true;
            this.resumeButton.onClick.AddListener(() => GameManager.Instance.StartGame(lastSave));
        }
        else
        {
            this.resumeButton.interactable = false;
        }
    }
}
