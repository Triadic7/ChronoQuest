using SQLite;
using System.Collections.Generic;

/// <summary>
/// Class which represents an npc.
/// </summary>
[Table("npcs")]
public class NPCTable
{
    /// <summary>
    /// Gets or sets the npc id.
    /// </summary>
    [PrimaryKey]
    public int NPCId { get; set; }

    /// <summary>
    /// Gets or sets the npc name.
    /// </summary>
    public string Name { get; set; }
}
