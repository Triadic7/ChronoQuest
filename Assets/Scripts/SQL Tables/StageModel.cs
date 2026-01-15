using UnityEngine;

/// <summary>
/// The class which represents a stage.
/// </summary>
public class StageModel
{
    /// <summary>
    /// Gets or sets the stage id.
    /// </summary>
    public int StageID { get; set; }

    /// <summary>
    /// Gets or sets the stage name.
    /// </summary>
    public string StageName { get; set; }

    /// <summary>
    /// Gets or sets the length of the pattern used for matching or validation operations.
    /// </summary>
    public int PatternLength { get; set; }

    /// <summary>
    /// Gets or sets the npc id.
    /// </summary>
    public int NPCId { get; set; }

    /// <summary>
    /// Gets or sets the npc.
    /// </summary>
    public NPCTable NPC { get; set; }

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
    /// Gets or sets the location.
    /// </summary>
    public LocationModel Location { get; set; }

    /// <summary>
    /// Gets or sets the location id.
    /// </summary>
    public int LocationID { get; set; }

    /// <summary>
    /// Gets or sets the key to the preview mp4 video.
    /// </summary>
    public string VideoPreviewKey { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="table">The table to convert.</param>
    /// <param name="location">The location.</param>
    public StageModel(StageTable table, LocationModel location = null)
    {
        this.StageID = table.StageId;
        this.StageName = table.StageName;
        this.ObjectiveText = table.ObjectiveText;
        this.IsFinalStage = table.IsFinalStage;
        this.StageDescription = table.StageDescription;
        this.ObjectiveProgress = table.ObjectiveProgress;
        this.LocationID = table.LocationID;
        this.Location = location;
        this.NPCId = table.NPCId;
        this.VideoPreviewKey = table.VideoPreviewKey;
    }

}
