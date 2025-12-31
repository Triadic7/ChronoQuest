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
    }
}
