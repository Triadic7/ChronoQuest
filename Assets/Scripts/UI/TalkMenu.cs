using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Talk menu that allows the player to start the game.
/// </summary>
public class TalkMenu : Menu
{
    /// <summary>
    /// On dialogue finished.
    /// </summary>
    public event Action OnDialogueFinished;

    /// <summary>
    /// Displays the location title.
    /// </summary>
    [SerializeField]
    private TMP_Text locationText;

    /// <summary>
    /// Displays the npcs name.
    /// </summary>
    [SerializeField]
    private TMP_Text npcNameText;

    /// <summary>
    /// Displays the npcs dialogue.
    /// </summary>
    [SerializeField]
    private TMP_Text npcText;

    /// <summary>
    /// The current npc dialogue index.
    /// </summary>
    private int dialogueIndex;

    /// <summary>
    /// The button to press after all dialogue has been finished.
    /// </summary>
    private Button continueButton;

    private List<string> dialogues;

    public override void Open()
    {
        base.Open();
    }

    /// <summary>
    /// Displays an npc to the talk menu.
    /// </summary>
    /// <param name="npc">The npc.</param>
    /// <param name="location">The location.</param>
    public void DisplayNPC(NPCModel npc, string location)
    {
        // Reset index.
        this.dialogueIndex = 0;

        // Set texts.
        this.locationText.text = location;
        this.npcNameText.text = npc.Name;

        // Cache dialogues.
        this.dialogues = npc.Dialogues.Select(d => d.Text).ToList();

        // Display dialogue.
        this.DisplayNpcText();
    }

    /// <summary>
    /// Sets display npc to the event.
    /// </summary>
    private void Awake()
    {
        this.dialogues = new List<string>();
        GameManager.Instance.OnDialogueStart += DisplayNPC;

        this.continueButton = GetComponentInChildren<Button>();

        if(continueButton == null)
        {
            Debug.LogError("No continue button found.");
            return;
        }

        continueButton.onClick.AddListener(() => ContinueText());
    }

    /// <summary>
    /// Displays the text on the talk menu.
    /// </summary>
    /// <param name="text"></param>
    private void DisplayNpcText()
    {
        this.npcText.text = dialogues[dialogueIndex];
    }

    /// <summary>
    /// Continues the npcs text. If text is exhausted, load game.
    /// </summary>
    private void ContinueText()
    {
        if(dialogueIndex < dialogues.Count - 1)
        {
            this.dialogueIndex += 1;
            this.DisplayNpcText();
        }
        else
        {
            this.OnDialogueFinished?.Invoke();
        }
    }
}
