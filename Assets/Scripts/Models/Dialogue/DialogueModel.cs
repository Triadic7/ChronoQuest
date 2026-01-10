using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a full dialogue for an NPC.
/// </summary>
[Serializable]
public class DialogueModel
{
    /// <summary>
    /// Gets or sets the ID of the dialogue.
    /// </summary>
    public int DialogueID { get; set; }

    /// <summary>
    /// Gets or sets the npcs id.
    /// </summary>
    public int NPCID { get; set; }

    /// <summary>
    /// Gets or sets the NPC that owns this dialogue.
    /// </summary>
    public string NPCName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this dialogue is an outro.
    /// </summary>
    public bool IsOutro { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this dialogue is for a winning outcome.
    /// </summary>
    public bool IsWin { get; set; }

    /// <summary>
    /// Gets or sets all nodes for this dialogue.
    /// </summary>
    public List<DialogueNodeModel> Nodes { get; set; } = new List<DialogueNodeModel>();

    /// <summary>
    /// Constuctor.
    /// </summary>
    /// <param name="table">The sqlite table.</param>
    /// <param name="choices">The choices.</param>
    /// <param name="nodes">The nodes.</param>
    public DialogueModel(DialogueTable table, List<DialogueNodeTable> nodes, List<DialogueChoiceTable> choices) 
    {
        this.DialogueID = table.DialogueID;
        this.NPCID = table.NPCID;
        this.NPCName = table.NPCName;

        this.IsOutro = table.IsOutro;
        this.IsWin = table.IsWin;

        // Build nodes.
        this.Nodes = nodes
            // Only include nodes that belong to this dialogue.
            .Where(n => n.DialogueID == table.DialogueID)
            // Ensure nodes are in the correct order.
            .OrderBy(n => n.Order)
            // Transform each node table entry into a DialogueNodeModel.
            .Select(n => new DialogueNodeModel
            {
                NodeID = n.NodeID,
                DialogueID = n.DialogueID,
                Order = n.Order,
                DialogueText = n.DialogueText,
                IsEndNode = n.IsEndNode,

                // Build the list of choices for this node.
                Choices = choices
                // Only include choices that belong to the current node.
                .Where(c => c.NodeID == n.NodeID)
                // Transform each choice table entry into a DialogueChoiceModel.
                .Select(c => new DialogueChoiceModel
                {
                    ChoiceText = c.ChoiceText,
                    NextNodeID = c.NextNodeID,
                    ActionName = c.ActionName
                })
                // Convert the choice models to a list.
                .ToList()
            })
            // Convert the node models to a list.
            .ToList();
    }
}
