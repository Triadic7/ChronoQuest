using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

        // Fire event.
        OnKeybindsUpdated?.Invoke(p1, p2);
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
    /// Converts database model into keybinds.
    /// </summary>
    /// <param name="save">The database model.</param>
    /// <param name="defaultKeybinds">The default keybinds.</param>
    /// <returns>Returns keybinds.</returns>
    private PlayerKeybinds ConvertToKeybinds(PlayerKeybindsSave save, PlayerKeybinds defaultKeybinds)
    {
        // Returns either saved keys or default values.
        return new PlayerKeybinds
        {
            up = ParseOrDefault(save.Up, defaultKeybinds.up),
            down = ParseOrDefault(save.Down, defaultKeybinds.down),
            left = ParseOrDefault(save.Left, defaultKeybinds.left),
            right = ParseOrDefault(save.Right, defaultKeybinds.right)
        };
    }

    /// <summary>
    /// Loads player keybinds based on player id.
    /// </summary>
    /// <param name="playerId">The players id.</param>
    /// <param name="defaultKeys">Default keybinds for player.</param>
    private PlayerKeybinds LoadPlayerKeybinds(int playerId, PlayerKeybinds defaultKeys)
    {
        // Gets db connection and loads keybinds from player id.
        SQLHandler sql = new SQLHandler();
        PlayerKeybindsSave loaded = sql.LoadKeybinds(playerId);

        // If player keybinds arent found, use default keys.
        if (loaded == null)
        {
            loaded = new PlayerKeybindsSave
            {
                PlayerId = playerId,
                Up = defaultKeys.up.ToString(),
                Down = defaultKeys.down.ToString(),
                Left = defaultKeys.left.ToString(),
                Right = defaultKeys.right.ToString()
            };
            sql.SaveKeybinds(loaded);
        }

        // Converts and returns player keybinds.
        return ConvertToKeybinds(loaded, defaultKeys);
    }

}
