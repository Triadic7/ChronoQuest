using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditorInternal.ReorderableList;

/// <summary>
/// Manages player settings like keybinds and music volume.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    /// <summary>
    /// Event that fires whenever a player updates keybinds.
    /// </summary>
    public event Action<PlayerKeybinds, PlayerKeybinds> OnKeybindsUpdated;

    /// <summary>
    /// Player one's keybinds.
    /// </summary>
    private PlayerKeybinds playerOneKeybinds;

    /// <summary>
    /// Player two's keybinds.
    /// </summary>
    private PlayerKeybinds playerTwoKeybinds;

    /// <summary>
    /// Returns player one keybinds.
    /// </summary>
    public PlayerKeybinds PlayerOneKeybinds => playerOneKeybinds;

    /// <summary>
    /// Returns player two keybinds.
    /// </summary>
    public PlayerKeybinds PlayerTwoKeybinds => playerTwoKeybinds;

    /// <summary>
    /// Sets the players keybinds.
    /// </summary>
    /// <param name="p1">Player one keybinds.</param>
    /// <param name="p2">Player two keybinds.</param>
    public void SetKeybinds(PlayerKeybinds p1, PlayerKeybinds p2)
    {
        this.playerOneKeybinds = p1;
        this.playerTwoKeybinds = p2;

        // Save keybinds to db.
        SaveKeybinds();

        // Fire event.
        OnKeybindsUpdated?.Invoke(p1, p2);
    }

    // Resets keybinds to default.
    public void ResetKeybinds()
    {
        // Deletes player prefs for keybinds.
        PlayerPrefs.DeleteAll();

        // Sets default keybinds.
        playerOneKeybinds = new PlayerKeybinds
        {
            up = Key.W,
            down = Key.S,
            left = Key.A,
            right = Key.D
        };

        playerTwoKeybinds = new PlayerKeybinds
        {
            up = Key.UpArrow,
            down = Key.DownArrow,
            left = Key.LeftArrow,
            right = Key.RightArrow
        };

        // Save keybinds.
        SaveKeybinds();

        // Fire event.
        OnKeybindsUpdated?.Invoke(playerOneKeybinds, playerTwoKeybinds);
    }

    /// <summary>
    /// Loads keybinds from db.
    /// </summary>
    private void Awake()
    {
        // Loads player one keybinds.
        playerOneKeybinds = LoadPlayerKeybinds(1, new PlayerKeybinds 
        { 
            up = Key.W, 
            down = Key.S, 
            left = Key.A, 
            right = Key.D 
        });

        // Loads player two keybinds.
        playerTwoKeybinds = LoadPlayerKeybinds(2, new PlayerKeybinds 
        { 
            up = Key.UpArrow, 
            down = Key.DownArrow, 
            left = Key.LeftArrow, 
            right = Key.RightArrow 
        });

        // Fire event.
        OnKeybindsUpdated?.Invoke(this.playerOneKeybinds, this.playerTwoKeybinds);
    }

    /// <summary>
    /// Tries to parse key from input, otherwise returns default key.
    /// </summary>
    /// <param name="keyStr">The input string.</param>
    /// <param name="defaultKey">The defualt key.</param>
    /// <returns>Returns key.</returns>
    private Key ParseOrDefault(string keyStr, Key defaultKey)
    {
        // If empty, return default key.
        if (string.IsNullOrEmpty(keyStr))
        {
            return defaultKey;
        }

        // Parses and returns key. If key is none, return default key.
        return Enum.TryParse<Key>(keyStr, out Key key) && key != Key.None ? key : defaultKey;
    }

    /// <summary>
    /// Saves both players' keybinds to the database.
    /// </summary>
    private void SaveKeybinds()
    {
        SavePlayer(1, playerOneKeybinds);
        SavePlayer(2, playerTwoKeybinds);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves player keybinds.
    /// </summary>
    /// <param name="playerId">Player id.</param>
    /// <param name="binds">The keybinds.</param>
    private void SavePlayer(int playerId, PlayerKeybinds binds)
    {
        // Prefix.
        string prefix = $"P{playerId}_";

        PlayerPrefs.SetString(prefix + "Up", binds.up.ToString());
        PlayerPrefs.SetString(prefix + "Down", binds.down.ToString());
        PlayerPrefs.SetString(prefix + "Left", binds.left.ToString());
        PlayerPrefs.SetString(prefix + "Right", binds.right.ToString());
    }

    /// <summary>
    /// Loads player keybinds based on player id.
    /// </summary>
    /// <param name="playerId">The players id.</param>
    /// <param name="defaults">Default keybinds for player.</param>
    private PlayerKeybinds LoadPlayerKeybinds(int playerId, PlayerKeybinds defaults)
    {
        string prefix = $"P{playerId}_";

        return new PlayerKeybinds
        {
            up = ParseOrDefault(PlayerPrefs.GetString(prefix + "Up"), defaults.up),
            down = ParseOrDefault(PlayerPrefs.GetString(prefix + "Down"), defaults.down),
            left = ParseOrDefault(PlayerPrefs.GetString(prefix + "Left"), defaults.left),
            right = ParseOrDefault(PlayerPrefs.GetString(prefix + "Right"), defaults.right)
        };
    }

}
