using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for showing a save preview on the load screen.
/// </summary>
public class SavePreview : MonoBehaviour
{
    /// <summary>
    /// Action thats called when the play button is hit.
    /// </summary>
    public event Action<SaveModel> OnPlayButtonHit;

    /// <summary>
    /// Displays the location title.
    /// </summary>
    [SerializeField]
    private TMP_Text locationText;

    /// <summary>
    /// Shows the objective for the location.
    /// </summary>
    [SerializeField]
    private TMP_Text objectiveText;

    /// <summary>
    /// The button to play and load the save.
    /// </summary>
    [SerializeField]
    private Button playButton;

    /// <summary>
    /// The button to toggle coop or singleplayer.
    /// </summary>
    [SerializeField]
    private Button toggleCoopButton;

    /// <summary>
    /// The text saying if it's coop or not.
    /// </summary>
    private TMP_Text coopText;

    /// <summary>
    /// If the game will be coop or not.
    /// </summary>
    private bool isCoop;

    /// <summary>
    /// The save data for loading a save.
    /// </summary>
    private SaveModel save;

    /// <summary>
    /// Displays a save preview.
    /// </summary>
    /// <param name="save">The save to show.</param>
    public void DisplaySavePreview(SaveModel save)
    {
        // If save is present, display it. Otherwise display new save.
        if(save != null)
        {
            this.locationText.text = save.Stage.StageName;
            this.objectiveText.text = save.Stage.ObjectiveText;
            this.coopText.text = save.IsCoop ? "Coop" : "Singleplayer";
        }
        else
        {
            this.locationText.text = "New Game";
            this.objectiveText.text = "Start a new adventure and attempt to save the timeline from forces beyond comprehension.";
            this.coopText.text = isCoop ? "Coop" : "Singleplayer";
        }

        this.save = save;
    }

    /// <summary>
    /// Plays the selected save.
    /// </summary>
    public void PlaySave()
    {
        if (save == null) 
        {
            save = new SaveModel(1, 0, isCoop);
        }
        this.OnPlayButtonHit?.Invoke(this.save);
    }

    /// <summary>
    /// Toggles coop button and updates text.
    /// </summary>
    public void CoopToggle()
    {
        this.isCoop = !this.isCoop;
        this.coopText.text = this.isCoop ? "Coop" : "Singleplayer";
    }

    /// <summary>
    /// Cache coop text.
    /// </summary>
    private void Awake()
    {
        // Cache coop text.
        this.coopText = toggleCoopButton.GetComponentInChildren<TMP_Text>();

        if(this.coopText == null)
        {
            Debug.LogError("No coop text found under button.");
            return;
        }
    }
}
