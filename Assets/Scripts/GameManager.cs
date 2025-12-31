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
    /// On Game ready.
    /// </summary>
    public event Action OnGameInitialized;

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

        // Checks if settings are null.
        if (Settings == null)
        {
            Debug.LogError("GameManager requires a SettingsManager component.");
            return;
        }
    }

    /// <summary>
    /// On start.
    /// </summary>
    private void Start()
    {
        this.OnGameInitialized?.Invoke();
        StartGame();
    }

    /// <summary>
    /// Starts game by showing main menu.
    /// </summary>
    private void StartGame()
    {
        UIManager.OpenMenu("MainMenu");
    }
}
