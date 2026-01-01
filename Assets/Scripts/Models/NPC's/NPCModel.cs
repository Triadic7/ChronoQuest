
using System.Collections.Generic;

/// <summary>
/// Represents an npc with dialogue.
/// </summary>
public class NPCModel
{
    /// <summary>
    /// Gets or sets the npc id.
    /// </summary>
    public int NPCId { get; set; }

    /// <summary>
    /// Gets or sets the npc name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the npc dialogues.
    /// </summary>
    public List<NPCDialogueModel> Dialogues { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="table">The sql table.</param>
    /// <param name="dialogues">The npc dialogues.</param>
    public NPCModel(NPCTable table, List<NPCDialogueModel> dialogues)
    {
        this.NPCId = table.NPCId;
        this.Name = table.Name;
        this.Dialogues = dialogues;
    }
}
