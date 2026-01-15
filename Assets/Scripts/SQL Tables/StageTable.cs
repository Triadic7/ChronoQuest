using SQLite;

/// <summary>
/// Stage table for SQLite.
/// </summary>
[Table("stages")]
public class StageTable
{
    /// <summary>
    /// Gets or sets the stage id.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int StageId { get; set; }

    /// <summary>
    /// Gets or sets the stage name.
    /// </summary>
    public string StageName { get; set; }

    /// <summary>
    /// Gets or sets the npc id.
    /// </summary>
    public int NPCId { get; set; }

    /// <summary>
    /// Gets or sets the location id.
    /// </summary>
    public int LocationID { get; set; }

    /// <summary>
    /// Gets or sets the stages description.
    /// </summary>
    public string StageDescription { get; set; }

    /// <summary>
    /// Gets or sets the objective text.
    /// </summary>
    public string ObjectiveText { get; set; }

    /// <summary>
    /// Gets or sets the amount of progress needed for the objective.
    /// </summary>
    public int ObjectiveProgress { get; set; }

    /// <summary>
    /// Checks if the stage is the final stage.
    /// </summary>
    public bool IsFinalStage { get; set; }

    /// <summary>
    /// Gets or sets the key to the preview mp4 video.
    /// </summary>
    public string VideoPreviewKey { get; set; }

}
