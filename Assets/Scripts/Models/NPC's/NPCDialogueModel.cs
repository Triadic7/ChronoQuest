
/// <summary>
/// Every npc will have a list of this class for talking.
/// </summary>
public class NPCDialogueModel
{
    /// <summary>
    /// Gets or sets the dialogue id.
    /// </summary>
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

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="table">The sql table.</param>
    /// <param name="dialogues">The npc dialogues.</param>
    public NPCDialogueModel(NPCDialogueTable table)
    {
        this.DialogueId = table.DialogueId;
        this.NPCId = table.NPCId;
        this.Text = table.Text;
        this.Order = table.Order;
    }
}
