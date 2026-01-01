using SQLite;

/// <summary>
/// Dialogue for an npc.
/// </summary>
[Table("npcdialogues")]
public class NPCDialogueTable
{
    /// <summary>
    /// Gets or sets the dialogue id.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int DialogueId { get; set; }

    /// <summary>
    /// Gets or sets the npc id.
    /// </summary>
    public int NPCId { get; set; }

    /// <summary>
    /// Gets or sets the dialogue text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the order in which this appears.
    /// </summary>
    public int Order { get; set; }

}
