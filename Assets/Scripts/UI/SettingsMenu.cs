using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

/// <summary>
/// Class that handles the settings menu UI.
/// </summary>
public class SettingsMenu : Menu
{
    #region Keybind Text
    [SerializeField]
    private TMP_Text playerOneUpText;

    [SerializeField]
    private TMP_Text playerOneDownText;

    [SerializeField]
    private TMP_Text playerOneLeftText;

    [SerializeField]
    private TMP_Text playerOneRightText;

    [SerializeField]
    private TMP_Text playerTwoUpText;

    [SerializeField]
    private TMP_Text playerTwoDownText;

    [SerializeField]
    private TMP_Text playerTwoLeftText;

    [SerializeField]
    private TMP_Text playerTwoRightText;
    #endregion

    /// <summary>
    /// The music volume slider.
    /// </summary>
    [Header("Audio")]
    [SerializeField]
    private Slider musicVolumeSlider;
    /// <summary>
    /// The sound effects slider.
    /// </summary>
    [SerializeField] 
    private Slider sfxVolumeSlider;

    #region Keybind Buttons
    [SerializeField]
    private Button playerOneUpButton;

    [SerializeField]
    private Button playerOneDownButton;

    [SerializeField]
    private Button playerOneLeftButton;

    [SerializeField]
    private Button playerOneRightButton;

    [SerializeField]
    private Button playerTwoUpButton;

    [SerializeField]
    private Button playerTwoDownButton;

    [SerializeField]
    private Button playerTwoLeftButton;

    [SerializeField]
    private Button playerTwoRightButton;
    #endregion

    /// <summary>
    /// Settings manager for getting keybinds.
    /// </summary>
    private SettingsManager settingsManager;

    /// <summary>
    /// If waiting for key to be pressed.
    /// </summary>
    private bool waitingForKey = false;

    /// <summary>
    /// The current text to update.
    /// </summary>
    private TMP_Text currentTextToUpdate;

    /// <summary>
    /// The current player the keybinding is changing.
    /// </summary>
    private string currentPlayer;

    /// <summary>
    /// The current action like left right up down.
    /// </summary>
    private string currentAction;

    /// <summary>
    /// Original colors of the texts before rebinding.
    /// </summary>
    private Dictionary<TMP_Text, Color> originalColors = new Dictionary<TMP_Text, Color>();

    /// <summary>
    /// Helps prevent recursive updates when changing the music slider.
    /// </summary>
    private bool updatingMusicSlider;

    /// <summary>
    /// The sound effects manager.
    /// </summary>
    private SFXManager sfxManager;

    /// <summary>
    /// On open display settings.
    /// </summary>
    public override void Open()
    {
        base.Open();

        // Initialize music volume slider.
        if (musicVolumeSlider != null)
        {
            updatingMusicSlider = true;
            musicVolumeSlider.value = MusicManager.Instance != null ? MusicManager.Instance.Volume : musicVolumeSlider.value;
            updatingMusicSlider = false;

            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeSliderChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderChanged);
        }

        if(this.sfxManager == null)
        {
            this.sfxManager = GameManager.Instance.SFXManager;
        }

        // Initialize SFX volume slider.
        if (this.sfxVolumeSlider != null)
        {
            this.sfxVolumeSlider.value = sfxManager.Volume;
            this.sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeSliderChanged);
            this.sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeSliderChanged);
        }

        // Subscribe to music track changes.
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.OnTrackChanged -= OnMusicTrackChanged;
            MusicManager.Instance.OnTrackChanged += OnMusicTrackChanged;
        }

    }

    /// <summary>
    /// Resets keybinds to default.
    /// </summary>
    public void ResetKeybinds()
    {
        this.settingsManager.ResetKeybinds();
        this.DisplayKeybinds();
    }

    /// <summary>
    /// On sfx slider changed.
    /// </summary>
    /// <param name="value">The volume.</param>
    private void OnSFXVolumeSliderChanged(float value)
    {
        sfxManager.SetVolume(value);
    }

    /// <summary>
    /// Initializes the rebinding process for a specific key.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="action">The action.</param>
    /// <param name="textLabel">The text label.</param>
    private void StartRebindKey(string player, string action, TMP_Text textLabel)
    {
        // Only one key at a time.
        if (this.waitingForKey)
        {
            return;
        }

        // Sets waiting and current player.
        this.waitingForKey = true;
        this.currentPlayer = player;
        this.currentAction = action;
        this.currentTextToUpdate = textLabel;

        // Store original color if not already stored.
        if (!this.originalColors.ContainsKey(textLabel))
        {
            this.originalColors[textLabel] = textLabel.color;
        }

        // Displays text to prompt user input.
        textLabel.text = "Press a key...";
        textLabel.color = Color.yellow;
    }

    /// <summary>
    /// Checks if key is already bound.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Returns true if key is already bound.</returns>
    private bool IsKeyAlreadyBound(Key key)
    {
        var p1 = this.settingsManager.PlayerOneKeybinds;
        var p2 = this.settingsManager.PlayerTwoKeybinds;

        // Only check other keys, skip the one we're rebinding.
        if (this.currentPlayer == "PlayerOne")
        {
            if (this.currentAction != "up" && key == p1.up)
            {
                return true;
            }
            if (this.currentAction != "down" && key == p1.down)
            {
                return true;
            }
            if (this.currentAction != "left" && key == p1.left)
            {
                return true;
            }
            if (this.currentAction != "right" && key == p1.right)
            {
                return true;
            }
        }
        // PlayerTwo.
        else
        {
            if (this.currentAction != "up" && key == p2.up)
            {
                return true;
            }
            if (this.currentAction != "down" && key == p2.down)
            {
                return true;
            }
            if (this.currentAction != "left" && key == p2.left)
            {
                return true;
            }
            if (this.currentAction != "right" && key == p2.right)
            {
                return true;
            }
        }

        // Also check other player's keys.
        if (key == (this.currentPlayer == "PlayerOne" ? p2.up : p1.up))
        {
            return true;
        }
        if (key == (this.currentPlayer == "PlayerOne" ? p2.down : p1.down))
        {
            return true;
        }
        if (key == (this.currentPlayer == "PlayerOne" ? p2.left : p1.left))
        {
            return true;
        }
        if (key == (this.currentPlayer == "PlayerOne" ? p2.right : p1.right))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Checks if key is allowed.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Returns true if key can be used.</returns>
    private bool IsAllowedKey(Key key)
    {
        // Letters.
        if (key >= Key.A && key <= Key.Z)
        {
            return true;
        }

        // Numbers.
        if (key >= Key.Digit0 && key <= Key.Digit9)
        {
            return true;
        }

        // Arrow keys.
        if (key == Key.UpArrow || key == Key.DownArrow || key == Key.LeftArrow || key == Key.RightArrow)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Caches settings manager to save keybinds to.
    /// </summary>
    private void Start()
    {
        this.settingsManager = GameManager.Instance.Settings;
        this.sfxManager = GameManager.Instance.SFXManager;
        this.DisplayKeybinds();

        // Add listeners for player 1.
        this.playerOneUpButton.onClick.AddListener(() => this.StartRebindKey("PlayerOne", "up", this.playerOneUpText));
        this.playerOneDownButton.onClick.AddListener(() => this.StartRebindKey("PlayerOne", "down", this.playerOneDownText));
        this.playerOneLeftButton.onClick.AddListener(() => this.StartRebindKey("PlayerOne", "left", this.playerOneLeftText));
        this.playerOneRightButton.onClick.AddListener(() => this.StartRebindKey("PlayerOne", "right", this.playerOneRightText));

        // Add listeners for player 2.
        this.playerTwoUpButton.onClick.AddListener(() => this.StartRebindKey("PlayerTwo", "up", this.playerTwoUpText));
        this.playerTwoDownButton.onClick.AddListener(() => this.StartRebindKey("PlayerTwo", "down", this.playerTwoDownText));
        this.playerTwoLeftButton.onClick.AddListener(() => this.StartRebindKey("PlayerTwo", "left", this.playerTwoLeftText));
        this.playerTwoRightButton.onClick.AddListener(() => this.StartRebindKey("PlayerTwo", "right", this.playerTwoRightText));
    }

    /// <summary>
    /// Performs cleanup by detaching event listeners associated with the music volume slider and the music manager.
    /// </summary>
    private void OnDestroy()
    {
        if (this.musicVolumeSlider != null)
        {
            this.musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeSliderChanged);
        }

        if (this.sfxVolumeSlider != null)
        {
            this.sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeSliderChanged);
        }


        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.OnTrackChanged -= OnMusicTrackChanged;
        }
    }

    /// <summary>
    /// Handles changes to the music volume slider by updating the music playback volume.
    /// </summary>
    /// <param name="value">The volume level to set for music playback.</param>
    private void OnMusicVolumeSliderChanged(float value)
    {
        if (this.updatingMusicSlider)
        {
            return;
        }

        if (MusicManager.Instance == null)
        {
            return;
        }

        MusicManager.Instance.SetVolume(value);
    }

    /// <summary>
    /// Handles changes to the music track by updating the music volume slider to reflect the current volume.
    /// </summary>
    /// <param name="_">The audio clip.</param>
    private void OnMusicTrackChanged(AudioClip clip)
    {
        if (this.musicVolumeSlider == null)
        {
            return;
        }

        if (MusicManager.Instance == null)
        {
            return;
        }

        this.updatingMusicSlider = true;
        this.musicVolumeSlider.value = MusicManager.Instance.Volume;
        this.updatingMusicSlider = false;
    }

    /// <summary>
    /// Watches for key presses.
    /// </summary>
    private void Update()
    {
        // If waiting for key is false, or keyboard is null, return.
        if (!this.waitingForKey || Keyboard.current == null)
        {
            return;
        }

        // Loop through all keys and detect the first pressed key.
        foreach (KeyControl keyControl in Keyboard.current.allKeys)
        {
            if (keyControl.wasPressedThisFrame)
            {
                this.ApplyKeybind(keyControl.keyCode);
                break;
            }
        }
    }

    /// <summary>
    /// Applies the selected keybind and updates UI + settings.
    /// </summary>
    /// <param name="key">The key.</param>
    private void ApplyKeybind(Key key)
    {
        this.waitingForKey = false;

        // Checks if key player wants to use is already used.
        if (this.IsKeyAlreadyBound(key))
        {
            this.currentTextToUpdate.text = "Key already used!";
            this.currentTextToUpdate.color = Color.red;
            return;
        }

        // Checks if key isn't allowed.
        if (!this.IsAllowedKey(key))
        {
            this.currentTextToUpdate.text = "Invalid key!";
            this.currentTextToUpdate.color = Color.red;
            return;
        }

        // Apply to correct player.
        if (this.currentPlayer == "PlayerOne")
        {
            this.ApplyToPlayer(this.settingsManager.PlayerOneKeybinds, key);
        }
        else
        {
            this.ApplyToPlayer(this.settingsManager.PlayerTwoKeybinds, key);
        }

        // Restore text and color from dictionary.
        this.currentTextToUpdate.text = key.PrettyName();
        this.currentTextToUpdate.color = this.originalColors[this.currentTextToUpdate];

        // Apply changes.
        this.settingsManager.SetKeybinds(this.settingsManager.PlayerOneKeybinds, this.settingsManager.PlayerTwoKeybinds);
    }

    /// <summary>
    /// Applies a key to the correct action on a player.
    /// </summary>
    private void ApplyToPlayer(PlayerKeybinds binds, Key key)
    {
        switch (this.currentAction)
        {
            case "up": 
                binds.up = key; 
                break;
            case "down": 
                binds.down = key; 
                break;
            case "left": 
                binds.left = key; 
                break;
            case "right": 
                binds.right = key; 
                break;
        }
    }

    /// <summary>
    /// Displays current keybinds.
    /// </summary>
    private void DisplayKeybinds()
    {
        if (this.settingsManager != null) 
        {
            var p1 = this.settingsManager.PlayerOneKeybinds;
            var p2 = this.settingsManager.PlayerTwoKeybinds;

            // Set player 1 keybind text.
            this.playerOneUpText.text = p1.up.PrettyName();
            this.playerOneDownText.text = p1.down.PrettyName();
            this.playerOneLeftText.text = p1.left.PrettyName();
            this.playerOneRightText.text = p1.right.PrettyName();

            // Set player 2 keybind text.
            this.playerTwoUpText.text = p2.up.PrettyName();
            this.playerTwoDownText.text = p2.down.PrettyName();
            this.playerTwoLeftText.text = p2.left.PrettyName();
            this.playerTwoRightText.text = p2.right.PrettyName();

            // Reset colors to original.
            foreach (var kvp in this.originalColors)
            {
                kvp.Key.color = kvp.Value;
            }
        }
        else
        {
            Debug.LogError("No SettingsManager found.");
        }
    }
}
