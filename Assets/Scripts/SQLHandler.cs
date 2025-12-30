using UnityEngine;
using SQLite;
using System.IO;

/// <summary>
/// Handles interactions with the db.
/// </summary>
public class SQLHandler
{
    /// <summary>
    /// The database.
    /// </summary>
    private SQLiteConnection db;

    /// <summary>
    /// Constructor for sql.
    /// </summary>
    /// <param name="databaseName">The db file path.</param>
    public SQLHandler(string databaseName = "chronoquest.db")
    {
        string dbPath = Path.Combine(Application.persistentDataPath, databaseName);
        db = new SQLiteConnection(dbPath);
        Debug.Log($"Database path: {dbPath}");

        db.CreateTable<PlayerKeybindsSave>();
    }

    /// <summary>
    /// Saves keybinds to db.
    /// </summary>
    /// <param name="keybinds">The keybinds to save.</param>
    public void SaveKeybinds(PlayerKeybindsSave keybinds)
    {
        db.InsertOrReplace(keybinds);
    }

    /// <summary>
    /// Loads the players keybinds based on id.
    /// </summary>
    /// <param name="playerId">The players id.</param>
    /// <returns>Returns the player keybinds.</returns>
    public PlayerKeybindsSave LoadKeybinds(int playerId)
    {
        return db.Find<PlayerKeybindsSave>(playerId);
    }
}
