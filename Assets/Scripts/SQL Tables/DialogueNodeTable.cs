using SQLite;

/// <summary>
/// SQLite table for storing dialogue nodes.
/// </summary>
[Table("dialogue_nodes")]
public class DialogueNodeTable
{
    /// <summary>
    /// Gets or sets node id.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int NodeID { get; set; }

    /// <summary>
    /// Gets or sets the Fk to dialogue id.
    /// </summary>
    public int DialogueID { get; set; }

    /// <summary>
    /// The order of this node within the dialogue.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the dialogue text.
    /// </summary>
    public string DialogueText { get; set; }

    /// <summary>
    /// Gets or sets whether this node is an end node.
    /// </summary>
    public bool IsEndNode { get; set; }
}
