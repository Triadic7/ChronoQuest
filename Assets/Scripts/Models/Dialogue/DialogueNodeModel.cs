using System;
using System.Collections.Generic;

/// <summary>
/// A single node in a dialogue.
/// </summary>
[Serializable]
public class DialogueNodeModel
{
    /// <summary>
    /// Gets or sets the ID of the node.
    /// </summary>
    public int NodeID { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dialogue this node belongs to.
    /// </summary>
    public int DialogueID { get; set; }

    /// <summary>
    /// The order of this node within the dialogue.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the text spoken by the NPC at this node.
    /// </summary>
    public string DialogueText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this node ends the dialogue.
    /// </summary>
    public bool IsEndNode { get; set; }

    /// <summary>
    /// Gets or sets the choices the player can make at this node.
    /// </summary>
    public List<DialogueChoiceModel> Choices { get; set; } = new List<DialogueChoiceModel>();
}
