using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Video;

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
    private Toggle toggleCoop;

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
    /// Video player that displays the preview of the stage.
    /// </summary>
    [SerializeField]
    private VideoPlayer previewVideoPlayer;

    /// <summary>
    /// The preview image that will have the mp4 shown.
    /// </summary>
    [SerializeField]
    private RawImage previewRawImage;

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
            this.objectiveText.text = save.Stage.StageDescription;
            this.coopText.text = save.IsCoop ? "Coop" : "Singleplayer";

            // Set isCoop for UI toggle.
            this.isCoop = save.IsCoop;

            // Addressables key.
            string key = save.Stage.VideoPreviewKey;

            // Tries to load mp4 from the key.
            Addressables.LoadAssetAsync<VideoClip>(key).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.previewVideoPlayer.clip = handle.Result;
                    this.previewVideoPlayer.isLooping = true;
                    this.previewVideoPlayer.Play();
                }
                else
                {
                    Debug.LogWarning($"Failed to load preview video {key}.");
                }
            };
        }
        else
        {
            this.locationText.text = "New Game";
            this.objectiveText.text = "Start a new adventure and attempt to save the timeline from forces beyond comprehension.";
            this.coopText.text = isCoop ? "Coop" : "Singleplayer";

            // Set default stage 1 key.
            string key = GameManager.Instance.SaveManager.DataContext.GetStageById(1).VideoPreviewKey;

            // Tries to load mp4 from the key.
            Addressables.LoadAssetAsync<VideoClip>(key).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.previewVideoPlayer.clip = handle.Result;
                    this.previewVideoPlayer.isLooping = true;
                    this.previewVideoPlayer.Play();
                }
                else
                {
                    Debug.LogWarning($"Failed to load preview video {key}.");
                }
            };
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

        save.IsCoop = isCoop;
        this.OnPlayButtonHit?.Invoke(this.save);
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
        this.coopText = toggleCoop.GetComponentInChildren<TMP_Text>();

        if(this.coopText == null)
        {
            Debug.LogError("No coop text found under button.");
            return;
        }

        // Add event listeners.
        this.playButton.onClick.AddListener(this.PlaySave);

        // Toggle for coop.
        toggleCoop.onValueChanged.AddListener((value) =>
        {
            this.isCoop = value;
            this.coopText.text = this.isCoop ? "Coop" : "Singleplayer";
            Debug.Log("Coop toggled to: " + this.isCoop);
        });

        this.deleteButton.onClick.AddListener(this.ShowDeleteConfirm);
        this.cancelButton.onClick.AddListener(this.CancelDeleteButton);
        this.confirmDeleteButton.onClick.AddListener(this.DeleteSave);
    }
}
