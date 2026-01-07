using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Talk menu that displays NPC dialogue and dynamically generated choice buttons.
/// </summary>
public class TalkMenu : Menu
{
    /// <summary>
    /// Displays the current location of the dialogue.
    /// </summary>
    [SerializeField] 
    private TMP_Text locationText;

    /// <summary>
    /// Displays the NPC's name.
    /// </summary>
    [SerializeField] 
    private TMP_Text npcNameText;

    /// <summary>
    /// Displays the players response.
    /// </summary>
    [SerializeField]
    private TMP_Text playerResponseText;

    /// <summary>
    /// Displays the dialogue text of the NPC.
    /// </summary>
    [SerializeField] 
    private TMP_Text npcText;

    /// <summary>
    /// The parent transform that holds the choice buttons.
    /// </summary>
    [SerializeField] 
    private Transform choicesContainer;

    /// <summary>
    /// Prefab for creating buttons representing dialogue choices.
    /// </summary>
    [SerializeField] 
    private GameObject choiceButtonPrefab;

    /// <summary>
    /// This is the panel the player response will get displayed on.
    /// </summary>
    [SerializeField]
    private GameObject playerResponsePanel;

    /// <summary>
    /// Reference to the DialogueManager in the scene.
    /// </summary>
    private DialogueManager dialogueManager;

    /// <summary>
    /// Displays the current dialogue node on the UI, including dynamic choice buttons.
    /// </summary>
    /// <param name="node">The dialogue node to display.</param>
    private void DisplayNode(DialogueNodeModel node)
    {
        npcText.text = node.DialogueText;
        npcText.ForceMeshUpdate();

        // Clear old choice buttons.
        foreach (Transform child in this.choicesContainer)
        {
            Destroy(child.gameObject);
        }

        // Generate new choice buttons.
        for (int i = 0; i < node.Choices.Count; i++)
        {
            int choiceIndex = i;

            // Instance buttons and subscribe to click.
            GameObject buttonGo = Instantiate(this.choiceButtonPrefab, this.choicesContainer);

            // Check for null.
            Button button = buttonGo.GetComponent<Button>();
            if(button == null)
            {
                Debug.LogWarning("No button found.");
                return;
            }

            // Set text and on click event.
            button.GetComponentInChildren<TMP_Text>().text = node.Choices[i].ChoiceText;

            // Unsubscribe from any events.
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                // Prevent double clicks.
                button.interactable = false;
                this.dialogueManager.SelectChoice(choiceIndex);
            });
        }
    }

    /// <summary>
    /// Called when the dialogue ends. Can be used to close the menu or trigger events.
    /// </summary>
    private void OnDialogueComplete()
    {
        Debug.Log("Dialogue ended.");
        // Close the TalkMenu.
        this.Close();
    }

    /// <summary>
    /// Subscribe to events.
    /// </summary>
    private void OnEnable()
    {
        if (this.dialogueManager == null)
        {
            this.dialogueManager = GameManager.Instance.DialogueManager;
        }

        this.dialogueManager.OnNPCUpdated += this.UpdateNPCInfo;
        this.dialogueManager.OnNodeUpdated += this.DisplayNode;
        this.dialogueManager.OnDialogueEnded += this.OnDialogueComplete;
        this.dialogueManager.OnPlayerChoiceSelected += this.DisplayPlayerResponse;

        this.DisplayPlayerResponse();
    }

    /// <summary>
    /// Unsubscribe to events.
    /// </summary>
    private void OnDisable()
    {
        this.dialogueManager.OnNPCUpdated -= this.UpdateNPCInfo;
        this.dialogueManager.OnNodeUpdated -= this.DisplayNode;
        this.dialogueManager.OnDialogueEnded -= this.OnDialogueComplete;
        this.dialogueManager.OnPlayerChoiceSelected -= this.DisplayPlayerResponse;
    }

    /// <summary>
    /// Updates npc and stage info.
    /// </summary>
    /// <param name="npc">The npc.</param>
    /// <param name="stage">The stage.</param>
    private void UpdateNPCInfo(NPCModel npc, StageModel stage)
    {
        this.npcNameText.text = npc.Name;
        this.locationText.text = stage.StageName;
    }

    /// <summary>
    /// Displays a player response
    /// </summary>
    /// <param name="response">The player response in dialogue.</param>
    private void DisplayPlayerResponse(string response = "")
    {
        // If no string is passed, hide the panel.
        if (response == string.Empty) 
        {
            this.playerResponsePanel.SetActive(false);
        }
        else
        {
            // Display text on panel if present.
            this.playerResponsePanel.SetActive(true);
            this.playerResponseText.text = "<b>You:</b> " + response;
        }
    }
}
