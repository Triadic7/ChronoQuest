using SQLite;

/// <summary>
/// The SQLite table for save progress.
/// </summary>
public class StageProgressTable
{
    /// <summary>
    /// The stage progress id.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// The save id.
    /// </summary>
    public int SaveId { get; set; }

    /// <summary>
    /// The stage id.
    /// </summary>
    public int StageId { get; set; }

    /// <summary>
    /// Gets or sets the result.
    /// This is an enum 1, 2, 3.
    /// </summary>
    public int Result { get; set; }
}
