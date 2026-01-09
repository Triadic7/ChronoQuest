using SQLite;

/// <summary>
/// SQLite table for storing dialogues.
/// </summary>
[Table("dialogues")]
public class DialogueTable
{
    /// <summary>
    /// Gets or sets the dialogue id.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int DialogueID { get; set; }

    /// <summary>
    /// Gets or sets the npcs name.
    /// </summary>
    public string NPCName { get; set; }

    /// <summary>
    /// Gets or sets the npcs id.
    /// </summary>
    public int NPCID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this dialogue is an outro.
    /// </summary>
    public bool IsOutro { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this outro is for a winning outcome.
    /// </summary>
    public bool IsWin { get; set; }
}
