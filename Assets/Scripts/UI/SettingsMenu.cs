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
    /// Resets keybinds to default.
    /// </summary>
    public void ResetKeybinds()
    {
        this.settingsManager.ResetKeybinds();
        this.DisplayKeybinds();
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
        settingsManager = GameManager.Instance.Settings;
        DisplayKeybinds();

        // Add listeners for player 1.
        playerOneUpButton.onClick.AddListener(() => StartRebindKey("PlayerOne", "up", playerOneUpText));
        playerOneDownButton.onClick.AddListener(() => StartRebindKey("PlayerOne", "down", playerOneDownText));
        playerOneLeftButton.onClick.AddListener(() => StartRebindKey("PlayerOne", "left", playerOneLeftText));
        playerOneRightButton.onClick.AddListener(() => StartRebindKey("PlayerOne", "right", playerOneRightText));

        // Add listeners for player 2.
        playerTwoUpButton.onClick.AddListener(() => StartRebindKey("PlayerTwo", "up", playerTwoUpText));
        playerTwoDownButton.onClick.AddListener(() => StartRebindKey("PlayerTwo", "down", playerTwoDownText));
        playerTwoLeftButton.onClick.AddListener(() => StartRebindKey("PlayerTwo", "left", playerTwoLeftText));
        playerTwoRightButton.onClick.AddListener(() => StartRebindKey("PlayerTwo", "right", playerTwoRightText));
    }

    /// <summary>
    /// Watches for key presses.
    /// </summary>
    private void Update()
    {
        // If waiting for key is false, or keyboard is null, return.
        if (!waitingForKey || Keyboard.current == null)
        {
            return;
        }

        // Loop through all keys and detect the first pressed key.
        foreach (KeyControl keyControl in Keyboard.current.allKeys)
        {
            if (keyControl.wasPressedThisFrame)
            {
                ApplyKeybind(keyControl.keyCode);
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
        waitingForKey = false;

        // Checks if key player wants to use is already used.
        if (IsKeyAlreadyBound(key))
        {
            currentTextToUpdate.text = "Key already used!";
            currentTextToUpdate.color = Color.red;
            return;
        }

        // Checks if key isn't allowed.
        if (!IsAllowedKey(key))
        {
            currentTextToUpdate.text = "Invalid key!";
            currentTextToUpdate.color = Color.red;
            return;
        }

        // Apply to correct player.
        if (currentPlayer == "PlayerOne")
        {
            ApplyToPlayer(settingsManager.PlayerOneKeybinds, key);
        }
        else
        {
            ApplyToPlayer(settingsManager.PlayerTwoKeybinds, key);
        }

        // Restore text and color from dictionary.
        currentTextToUpdate.text = key.PrettyName();
        currentTextToUpdate.color = originalColors[currentTextToUpdate];

        // Apply changes.
        settingsManager.SetKeybinds(settingsManager.PlayerOneKeybinds, settingsManager.PlayerTwoKeybinds);
    }

    /// <summary>
    /// Applies a key to the correct action on a player.
    /// </summary>
    private void ApplyToPlayer(PlayerKeybinds binds, Key key)
    {
        switch (currentAction)
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
        if (settingsManager != null) 
        {
            var p1 = settingsManager.PlayerOneKeybinds;
            var p2 = settingsManager.PlayerTwoKeybinds;

            // Set player 1 keybind text.
            playerOneUpText.text = p1.up.PrettyName();
            playerOneDownText.text = p1.down.PrettyName();
            playerOneLeftText.text = p1.left.PrettyName();
            playerOneRightText.text = p1.right.PrettyName();

            // Set player 2 keybind text.
            playerTwoUpText.text = p2.up.PrettyName();
            playerTwoDownText.text = p2.down.PrettyName();
            playerTwoLeftText.text = p2.left.PrettyName();
            playerTwoRightText.text = p2.right.PrettyName();

            // Reset colors to original.
            foreach (var kvp in originalColors)
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
