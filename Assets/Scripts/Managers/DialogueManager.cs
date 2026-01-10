using System;
using UnityEngine;

/// <summary>
/// Handles NPC dialogues, managing dialogue nodes and player choices.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    /// <summary>
    /// Fired whenever the current dialogue node is updated.
    /// </summary>
    public event Action<DialogueNodeModel> OnNodeUpdated;

    /// <summary>
    /// Fired when the dialogue ends.
    /// </summary>
    public event Action OnDialogueEnded;

    /// <summary>
    /// Fired when dialogue is triggered.
    /// </summary>
    public event Action<string> OnDialogueActionTriggered;

    /// <summary>
    /// Fired whenever the player selects a choice, passing the text of that choice.
    /// </summary>
    public event Action<string> OnPlayerChoiceSelected;

    /// <summary>
    /// Fired when starting dialogue.
    /// </summary>
    public event Action<NPCModel, StageModel> OnNPCUpdated;

    /// <summary>
    /// The current dialogue.
    /// </summary>
    public DialogueModel CurrentDialogue => currentDialogue;

    /// <summary>
    /// The current node index.
    /// </summary>
    public int CurrentNodeIndex => this.currentNodeIndex;

    /// <summary>
    /// The currently active dialogue.
    /// </summary>
    private DialogueModel currentDialogue;

    /// <summary>
    /// The index of the current node within the dialogue.
    /// </summary>
    private int currentNodeIndex;

    /// <summary>
    /// Has dialogue ended.
    /// </summary>
    private bool dialogueEnded = false;

    /// <summary>
    /// Starts a dialogue with the specified NPC.
    /// </summary>
    /// <param name="npc">The NPC whose dialogue to start.</param>
    /// <param name="stage">The stage the npc is in.</param>
    public void StartDialogue(NPCModel npc, StageModel stage)
    {
        if (npc == null || npc.Dialogues.Count == 0)
        {
            Debug.LogWarning("No dialogue for this NPC.");
            return;
        }

        this.dialogueEnded = false;

        // Fire event.
        this.OnNPCUpdated?.Invoke(npc, stage);

        // Get intro dialogue.
        DialogueModel introDialogue = npc.Dialogues.Find(d => !d.IsOutro);

        if (introDialogue == null)
        {
            Debug.LogWarning($"No intro dialogue found for NPC {npc.Name}");
            return;
        }

        this.StartDialogue(introDialogue);
    }

    /// <summary>
    /// Starts a specific dialogue directly, such as an outro.
    /// </summary>
    /// <param name="dialogue">The dialogue to start.</param>
    public void StartDialogue(DialogueModel dialogue)
    {
        if (dialogue == null || dialogue.Nodes.Count == 0)
        {
            Debug.LogWarning("Dialogue is null or empty.");
            return;
        }

        // Display dialogue.
        this.dialogueEnded = false;
        this.currentDialogue = dialogue;
        this.currentNodeIndex = 0;
        this.ShowNode();
    }

    /// <summary>
    /// Selects a dialogue choice at the current node and advances the dialogue.
    /// </summary>
    /// <param name="choiceIndex">The index of the chosen option.</param>
    public void SelectChoice(int choiceIndex)
    {
        if (this.dialogueEnded)
        {
            return;
        }

        // Get the currently active dialogue node using the index.
        var node = this.currentDialogue.Nodes[currentNodeIndex];

        // Ensure the selected choice index is valid.
        if (choiceIndex < 0 || choiceIndex >= node.Choices.Count)
        {
            return;
        }

        // Retrieve the chosen dialogue option.
        var choice = node.Choices[choiceIndex];

        // Fire event to for selected text.
        this.OnPlayerChoiceSelected?.Invoke(choice.ChoiceText);

        // If this choice has an associated action, trigger it.
        if (!string.IsNullOrEmpty(choice.ActionName))
        {
            Debug.Log($"Firing dialogue action {choice.ActionName}");
            this.OnDialogueActionTriggered?.Invoke(choice.ActionName);
        }

        // A NextNodeID of -1 means this choice explicitly ends the dialogue.
        if (choice.NextNodeID == -1)
        {
            this.dialogueEnded = true;
            this.OnDialogueEnded?.Invoke();
            return;
        }

        // Find the index of the next dialogue node by matching NodeID.
        var nextNodeIndex = this.currentDialogue.Nodes
            .FindIndex(n => n.NodeID == choice.NextNodeID);

        // If the next node could not be found, log a warning and safely end dialogue.
        if (nextNodeIndex == -1)
        {
            this.dialogueEnded = true;
            this.OnDialogueEnded?.Invoke();
            return;
        }

        // Update the current node index to the resolved next node.
        this.currentNodeIndex = nextNodeIndex;

        // Display the newly selected dialogue node.
        this.ShowNode();
    }

    /// <summary>
    /// Displays the current dialogue node and invokes the update event.
    /// </summary>
    private void ShowNode()
    {
        DialogueNodeModel node = this.currentDialogue.Nodes[currentNodeIndex];
        this.OnNodeUpdated?.Invoke(node);
    }
}
