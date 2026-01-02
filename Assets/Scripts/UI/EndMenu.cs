using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The panel at the end of the game showing the story result.
/// </summary>
public class EndMenu : Menu
{
    /// <summary>
    /// The button to end the game.
    /// </summary>
    private Button continueButton;

    private void Awake()
    {
        this.continueButton = GetComponentInChildren<Button>();
        if(this.continueButton == null)
        {
            Debug.LogError("No continue button found.");
            return;
        }

        GameManager gm = GameManager.Instance;
        if(gm == null)
        {
            Debug.LogError("No GameManager");
            return;
        }

        // Go to main menu.
        this.continueButton.onClick.AddListener(() => gm.UIManager.OpenMenu("MainMenu"));
    }
}
