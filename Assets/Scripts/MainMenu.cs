using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IMenu
{
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
    /// Hides main menu.
    /// </summary>
    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Nothing, main menu has no back button.
    /// </summary>
    public void OnBack() { }

    /// <summary>
    /// Shows main menu.
    /// </summary>
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

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
    /// On start.
    /// </summary>
    private void Start()
    {
        UIManager uIManager = GameManager.Instance.UIManager;

        // Adds events for menu buttons.
        if(uIManager != null)
        {
            settingsButton.onClick.AddListener(() => uIManager.OpenMenu("SettingsMenu"));
            loadButton.onClick.AddListener(() => uIManager.OpenMenu("LoadMenu"));
        }
        else
        {
            Debug.LogError("No UIManager found on GameManager.");
        }
    }
}
