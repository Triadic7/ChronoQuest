
using SQLite;
using System;

/// <summary>
/// Save table for SQLite.
/// </summary>
[Table("saves")]
public class SaveTable
{
    /// <summary>
    /// The id of the save.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the stage id.
    /// </summary>
    public int StageId { get; set; }

    /// <summary>
    /// Gets or sets the playtime of the save.
    /// </summary>
    public float PlayTime { get; set; }

    /// <summary>
    /// Gets or sets whether the game is coop or not.
    /// </summary>
    public bool IsCoop { get; set; }

    /// <summary>
    /// The data last played.
    /// </summary>
    public DateTime LastPlayed { get; set; }
}
