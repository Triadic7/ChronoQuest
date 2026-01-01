using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pause menu logic.
/// </summary>
public class PauseMenu : Menu
{
    /// <summary>
    /// Resume button.
    /// </summary>
    [SerializeField]
    private Button resumeButton;

    /// <summary>
    /// Settings button.
    /// </summary>
    [SerializeField]
    private Button settingsButton;

    /// <summary>
    /// Resume button.
    /// </summary>
    [SerializeField]
    private Button quitButton;

    /// <summary>
    /// On start.
    /// </summary>
    private void Start()
    {
        // Get GameManager.
        GameManager gm = GameManager.Instance;

        // Check for null.
        if (gm == null)
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        // Add event listeners to buttons.
        this.resumeButton.onClick.AddListener(gm.ResumeGame);
        this.settingsButton.onClick.AddListener(() => gm.UIManager.OpenMenu("SettingsMenu"));
        this.quitButton.onClick.AddListener(gm.QuitToMainMenu);
    }
}
