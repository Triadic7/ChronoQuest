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
    /// Displays the dialogue text of the NPC.
    /// </summary>
    [SerializeField] 
    private TMP_Text npcText;

    /// <summary>
    /// The npcs image.
    /// </summary>
    [SerializeField]
    private Image npcImage;

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
    /// Reference to the DialogueManager in the scene.
    /// </summary>
    private DialogueManager dialogueManager;

    /// <summary>
    /// The sprite provider to display stuff to UI.
    /// </summary>
    private NpcSpriteProvider npcSpriteProvider;

    /// <summary>
    /// Instance new sprite provider.
    /// </summary>
    private void Start()
    {
        this.npcSpriteProvider = new NpcSpriteProvider();
    }

    /// <summary>
    /// Displays the current dialogue node on the UI, including dynamic choice buttons.
    /// </summary>
    /// <param name="node">The dialogue node to display.</param>
    private void DisplayNode(DialogueNodeModel node)
    {
        this.npcText.text = node.DialogueText;
        this.npcText.ForceMeshUpdate();

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
    }

    /// <summary>
    /// Unsubscribe to events.
    /// </summary>
    private void OnDisable()
    {
        this.dialogueManager.OnNPCUpdated -= this.UpdateNPCInfo;
        this.dialogueManager.OnNodeUpdated -= this.DisplayNode;
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

        // Loads the npcs image using addressables. Passing in the sprite sheet location.
        this.npcSpriteProvider.Load(npc.ImagePath, "Assets/Art/Characters/CharactersAndLocations.png", sprite =>
        {
            this.npcImage.sprite = sprite != null ? sprite : null;
        });
    }
}
