/// <summary>
/// The model for stage progress data, like stats.
/// </summary>
public class StageProgressModel
{
    /// <summary>
    /// The stage id.
    /// </summary>
    public int StageID { get; set; }

    /// <summary>
    /// Gets or sets the stage result.
    /// </summary>
    public StageResult Result { get; set; }
}
