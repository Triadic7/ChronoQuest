using SQLite;

/// <summary>
/// SQLite table for storing dialogue choices.
/// </summary>
[Table("dialogue_choices")]
public class DialogueChoiceTable
{
    /// <summary>
    /// Gets or sets the choice id.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int ChoiceID { get; set; }

    /// <summary>
    /// Gets or sets the Fk to node id.
    /// </summary>
    public int NodeID { get; set; }

    /// <summary>
    /// Gets or sets the choice text.
    /// </summary>
    public string ChoiceText { get; set; }

    /// <summary>
    /// Gets or sets the next node id. 
    /// -1 = end dialogue.
    /// </summary>
    public int NextNodeID { get; set; }

    /// <summary>
    /// Optional action that gets called.
    /// </summary>
    public string ActionName { get; set; }
}
