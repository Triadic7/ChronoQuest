using System;

/// <summary>
/// Save data for loading a save.
/// </summary>
[System.Serializable]
public class SaveModel
{
    /// <summary>
    /// Save Id.
    /// </summary>
    public int Id;

    /// <summary>
    /// Gets the stage id.
    /// </summary>
    public int StageId;

    /// <summary>
    /// Gets the stage.
    /// </summary>
    public StageModel Stage;

    /// <summary>
    /// Playtime on save.
    /// </summary>
    public float PlayTime;

    /// <summary>
    /// If the game is coop or not.
    /// </summary>
    public bool IsCoop;

    /// <summary>
    /// The data last played.
    /// </summary>
    public DateTime LastPlayed { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="table">The sql table.</param>
    /// <param name="stage">The stage.</param>
    public SaveModel(SaveTable table, StageModel stage)
    {
        this.Id = table.Id;
        this.StageId = table.StageId;
        this.PlayTime = table.PlayTime;
        this.IsCoop = table.IsCoop;
        this.Stage = stage;
        this.LastPlayed = table.LastPlayed;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="stageId">The stage id.</param>
    /// <param name="playtime">The time played.</param>
    /// <param name="isCoop">If game is coop.</param>
    public SaveModel(int stageId, float playtime, bool isCoop)
    {
        this.StageId = stageId;
        this.PlayTime = playtime;
        this.IsCoop = isCoop;
    }
}
