using System;

/// <summary>
/// A dialogue option that gets displayed on the dialogue manager.
/// </summary>
[Serializable]
public class DialogueChoiceModel
{
    /// <summary>
    /// Gets or sets the text shown on the choice button.
    /// </summary>
    public string ChoiceText { get; set; }

    /// <summary>
    /// Gets or sets the index of the next node. -1 indicates the end of the dialogue.
    /// </summary>
    public int NextNodeID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the name of the action that gets fired when this choice is selected.
    /// </summary>
    public string ActionName { get; set; }
}
