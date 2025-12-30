using SQLite;

/// <summary>
/// Model for keybinds.
/// </summary>
public class PlayerKeybindsSave
{
    /// <summary>
    /// Player id primary key.
    /// </summary>
    [PrimaryKey]
    public int PlayerId { get; set; }

    /// <summary>
    /// Up key.
    /// </summary>
    public string Up { get; set; }

    /// <summary>
    /// Down key.
    /// </summary>
    public string Down { get; set; }

    /// <summary>
    /// Left key.
    /// </summary>
    public string Left { get; set; }

    /// <summary>
    /// Right key.
    /// </summary>
    public string Right { get; set; }
}
