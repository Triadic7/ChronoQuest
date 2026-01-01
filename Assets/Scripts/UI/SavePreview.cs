using System;
using TMPro;
using Unity.VisualScripting;
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
    /// Called when the player hits delete save.
    /// </summary>
    public event Action<SaveModel> OnDeleteSave;

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
    /// The button to delete a save.
    /// </summary>
    [SerializeField]
    private Button deleteButton;

    /// <summary>
    /// The button to cancel from deleting a save.
    /// </summary>
    [SerializeField]
    private Button cancelButton;

    /// <summary>
    /// The button confirm to delete a save.
    /// </summary>
    [SerializeField]
    private Button confirmDeleteButton;

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
        // Cache the save first.
        this.save = save;

        // Display info.
        if (save != null)
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

        // Shows delete button but not confirm delete buttons.
        this.CancelDeleteButton();
    }

    /// <summary>
    /// Plays the selected save.
    /// </summary>
    private void PlaySave()
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
    private void CoopToggle()
    {
        this.isCoop = !this.isCoop;
        this.coopText.text = this.isCoop ? "Coop" : "Singleplayer";
    }

    /// <summary>
    /// Hides all delete buttons.
    /// </summary>
    private void HideDeleteButtons()
    {
        this.deleteButton.gameObject.SetActive(false);
        this.cancelButton.gameObject.SetActive(false);
        this.confirmDeleteButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows confirm delete buttons.
    /// </summary>
    private void ShowDeleteConfirm()
    {
        this.deleteButton.gameObject.SetActive(false);
        this.cancelButton.gameObject.SetActive(true);
        this.confirmDeleteButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Cancels the delete operation.
    /// </summary>
    private void CancelDeleteButton()
    {
        if(save != null)
        {
            this.deleteButton.gameObject.SetActive(true);
            this.cancelButton.gameObject.SetActive(false);
            this.confirmDeleteButton.gameObject.SetActive(false);
        }
        else
        {
            this.HideDeleteButtons();
        }
    }

    /// <summary>
    /// Deletes the save.
    /// </summary>
    public void DeleteSave()
    {
        if(this.save == null)
        {
            Debug.LogError("No save found");
            return;
        }

        this.OnDeleteSave?.Invoke(this.save);
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

    /// <summary>
    /// Add event listeners.
    /// </summary>
    private void OnEnable()
    {
        this.playButton.onClick.AddListener(this.PlaySave);
        this.toggleCoopButton.onClick.AddListener(this.CoopToggle);
        this.deleteButton.onClick.AddListener(this.ShowDeleteConfirm);
        this.cancelButton.onClick.AddListener(this.CancelDeleteButton);
        this.confirmDeleteButton.onClick.AddListener(this.DeleteSave);
    }

    /// <summary>
    /// Remove event listeners.
    /// </summary>
    private void OnDisable()
    {
        this.playButton.onClick.RemoveListener(this.PlaySave);
        this.toggleCoopButton.onClick.RemoveListener(this.CoopToggle);
        this.deleteButton.onClick.RemoveListener(this.ShowDeleteConfirm);
        this.cancelButton.onClick.RemoveListener(this.CancelDeleteButton);
        this.confirmDeleteButton.onClick.RemoveListener(this.DeleteSave);
    }
}
