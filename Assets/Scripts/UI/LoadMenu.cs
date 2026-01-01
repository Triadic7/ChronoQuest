using System;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenu : Menu
{

    /// <summary>
    /// Action thats fired when the player presses play on save.
    /// </summary>
    public event Action<SaveModel> OnGamePlayButtonHit;

    /// <summary>
    /// Prefab of a save slot.
    /// </summary>
    [SerializeField]
    private GameObject saveSlotPrefab;

    /// <summary>
    /// The location where saves will be displayed.
    /// </summary>
    [SerializeField]
    private Transform saveSlotsLocation;

    /// <summary>
    /// The save manager to manage saves.
    /// </summary>
    private SaveManager saveManager;

    /// <summary>
    /// List of prefabs displayed in the UI.
    /// </summary>
    private List<GameObject> savePrefabs = new List<GameObject>();

    /// <summary>
    /// The panel to show a save preview.
    /// </summary>
    [SerializeField]
    private SavePreview previewPanel;

    /// <summary>
    /// Opens menu and displays saves.
    /// </summary>
    public override void Open()
    {
        base.Open();
        this.DisplayAllSaves();
        this.HidePreviewPanel();
    }

    /// <summary>
    /// Displays a more detailed save on the UI after clicking on it.
    /// </summary>
    /// <param name="id">The save data id to display on the UI.</param>
    public void DisplaySavePreview(int id = 0)
    {
        this.DisplayPreviewPanel();

        SaveModel save = saveManager.DataContext.GetSaveById(id);

        this.previewPanel.DisplaySavePreview(save);
    }

    /// <summary>
    /// Loads a save.
    /// </summary>
    /// <param name="save"></param>
    private void LoadSave(SaveModel save)
    {
        save = this.saveManager.SaveProgress(save);

        this.OnGamePlayButtonHit?.Invoke(save);
    }

    /// <summary>
    /// Subscribe to preview save event.
    /// </summary>
    private void Awake()
    {
        this.previewPanel.OnPlayButtonHit += LoadSave;
        this.previewPanel.OnDeleteSave += DeleteSaveFromDb;

        // Cache save manager.
        this.saveManager = GameManager.Instance.SaveManager;

        if (this.saveManager != null)
        {
            Debug.Log("Loaded save manager");
        }
        else
        {
            Debug.LogError("Save manager not loaded");
        }
    }

    /// <summary>
    /// Displays the preview panel.
    /// </summary>
    private void DisplayPreviewPanel()
    {
        this.previewPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the preview panel.
    /// </summary>
    private void HidePreviewPanel()
    {
        this.previewPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Deletes all save slot prefabs on UI.
    /// </summary>
    private void DeleteSavesSlotPrefabs()
    {
        // If there are prefabs present, delete them.
        foreach (GameObject prefab in savePrefabs)
        {
            Destroy(prefab);
        }

        // Clear the list so we start fresh.
        this.savePrefabs.Clear();
    }

    /// <summary>
    /// Deletes a save from the db.
    /// </summary>
    /// <param name="save"></param>
    private void DeleteSaveFromDb(SaveModel save)
    {
        if(save == null)
        {
            Debug.LogError("No save found.");
            return;
        }

        // Delete save.
        this.saveManager.DeleteSave(save);

        // Refresh the list UI.
        this.DisplayAllSaves();

        // Hide the preview panel.
        this.HidePreviewPanel();
    }

    /// <summary>
    /// Displays all saves to the UI.
    /// </summary>
    private void DisplayAllSaves()
    {
        // Delete old prefabs.
        this.DeleteSavesSlotPrefabs();

        // For each save slot, instance a new prefab and display save data.
        foreach (var save in this.saveManager.PlayerSaves)
        {
            // Instance new slot and add it to list.
            GameObject saveSlot = Instantiate(this.saveSlotPrefab, this.saveSlotsLocation.position, Quaternion.identity, this.saveSlotsLocation);
            savePrefabs.Add(saveSlot);

            // Get prefab component from gameobject.
            SaveSlotPrefab saveSlotPrefab = saveSlot.GetComponent<SaveSlotPrefab>();

            if(saveSlotPrefab == null)
            {
                Debug.LogError("SaveSlotPrefab script is null.");
                return;
            }

            // Display save on it.
            saveSlotPrefab.DisplaySave(save);

            // Add on click event to it.
            saveSlotPrefab.GetComponent<Button>().onClick.AddListener(() => DisplaySavePreview(save.Id));
        }
    }
}
